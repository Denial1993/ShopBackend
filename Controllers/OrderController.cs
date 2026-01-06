using System.Security.Claims;
using Dapper; // 👈 Dapper 登場
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient; // 👈 為了建立 SQL 連線
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Dtos;
using ShopApi.Models;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 只有會員能結帳和看訂單
    public class OrderController : ControllerBase
    {
        private readonly ShopDbContext _context;
        private readonly IConfiguration _configuration; // 為了拿連線字串給 Dapper 用

        public OrderController(ShopDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // 1. 結帳 (Checkout) - 使用 EF Core (處理複雜寫入)
        // POST: api/Order/checkout
        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout()
        {
            var userId = GetUserId();

            // A. 撈出購物車 (要包含商品資訊，因為要抓價格)
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.Items.Count == 0)
            {
                return BadRequest("購物車是空的，無法結帳");
            }

            // B. 建立訂單主檔
            var order = new Order
            {
                UserId = userId,
                TotalAmount = cart.Items.Sum(i => i.Product!.Price * i.Quantity)
            };

            // C. 處理明細 + 檢查庫存 + 扣庫存
            foreach (var item in cart.Items)
            {
                // 1. 檢查庫存夠不夠？
                if (item.Product!.Stock < item.Quantity)
                {
                    // 如果這行執行了，整個 Request 會直接結束，不會有任何資料寫入資料庫
                    return BadRequest($"商品 '{item.Product.Title}' 庫存不足！只剩 {item.Product.Stock} 個，你買了 {item.Quantity} 個。");
                }
                // 2. 扣庫存 (直接修改 Product 物件的屬性)
                // EF Core 會追蹤這個變化，等一下 SaveChanges 時會自動生出 UPDATE SQL
                item.Product.Stock -= item.Quantity;

                // 3. 建立訂單明細
                order.Details.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    ProductTitle = item.Product.Title,// 備份名稱
                    Price = item.Product.Price,       // 備份價格
                    Quantity = item.Quantity
                });
            }

            // D. 加入訂單到資料庫
            _context.Orders.Add(order);

            // E. 清空購物車 (這一步很重要！)
            _context.CartItems.RemoveRange(cart.Items);

            // F. 存檔 (EF Core 會自動把上述所有動作包成一個 Transaction，要嘛全成功，要嘛全失敗)
            // 這裡會同時執行：
            // 1. INSERT Order (新增訂單)
            // 2. INSERT OrderDetails (新增明細)
            // 3. UPDATE Products (扣庫存) <--- 新增的動作
            // 4. DELETE CartItems (清購物車)
            // 只要其中任何一個失敗 (例如 SQL 連線斷掉)，全部都會回滾 (Rollback)，不會發生「扣了庫存但沒訂單」的慘劇。
            await _context.SaveChangesAsync();

            return Ok(new { Message = "結帳成功", OrderId = order.Id });
        }

        // 2. 查詢我的歷史訂單 - 使用 Dapper (追求查詢效能)
        // GET: api/Order
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
        {
            var userId = GetUserId();

            // A. 準備 SQL 語法
            // Dapper 的起手式：建立 Connection
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            // B. 寫 SQL (這裡示範最簡單的查詢主檔)
            // 如果要像 EF Core 那樣一次把 Details 也抓出來，Dapper 寫法會比較複雜(Multi-Mapping)，
            // 這裡我們先練習抓主檔就好。
            string sql = @"
                SELECT * FROM Orders 
                WHERE UserId = @UserId 
                ORDER BY CreatedAt DESC";

            // C. 執行查詢
            var orders = await conn.QueryAsync<Order>(sql, new { UserId = userId });

            // 轉成 DTO
            var dtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                CreatedAt = o.CreatedAt,
                TotalAmount = o.TotalAmount,
                Details = [] // 列表頁給空陣列就好，省流量
            });

            return Ok(dtos);
        }

        // 3. 查詢單筆訂單詳情 (包含明細) - 使用 Dapper (進階練習)
        // GET: api/Order/5
        [HttpGet("{orderId}")]
        public async Task<ActionResult> GetOrderDetail(int orderId)
        {
            var userId = GetUserId();

            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            // 這裡我們用兩段 SQL 分別抓主檔和明細 (這是 Dapper 常見做法)
            string sql = @"
                SELECT * FROM Orders WHERE Id = @Id AND UserId = @UserId;
                SELECT * FROM OrderDetails WHERE OrderId = @Id;
            ";

            // QueryMultiple 可以一次執行多個查詢
            using var multi = await conn.QueryMultipleAsync(sql, new { Id = orderId, UserId = userId });

            var order = await multi.ReadFirstOrDefaultAsync<Order>();
            if (order == null) return NotFound("找不到訂單");

            var details = await multi.ReadAsync<OrderDetail>();

            // --- ⬇️ 修改重點：手動轉成 DTO (Mapping) ⬇️ ---
            var dto = new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                // 把資料庫的明細 (OrderDetail) 一筆一筆轉成 DTO (OrderDetailDto)
                Details = details.Select(d => new OrderDetailDto
                {
                    Id = d.Id,
                    OrderId = d.OrderId,
                    ProductId = d.ProductId,
                    ProductTitle = d.ProductTitle,
                    Price = d.Price,
                    Quantity = d.Quantity
                }).ToList()
            };

            return Ok(dto); // 回傳 DTO，不要直接回傳 order
        }

        // 小工具：取得 User ID
        private int GetUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idClaim == null) throw new Exception("請重新登入");
            return int.Parse(idClaim.Value);
        }
    }
}