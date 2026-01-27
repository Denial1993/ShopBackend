using System.Security.Claims;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Dtos;
using ShopApi.Models;

namespace ShopApi.Services
{
    public class OrderService(ShopDbContext context, IConfiguration configuration) : IOrderService
    {
        private readonly ShopDbContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        public async Task<(bool IsSuccess, string Message, int OrderId)> CheckoutAsync(int userId)
        {
            // A. 撈出購物車 (要包含商品資訊，因為要抓價格)
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.Items.Count == 0)
            {
                return (false, "購物車是空的", 0);
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
                    return (false, $"商品 '{item.Product.Title}' 庫存不足! ", 0);
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
            return (true, "結帳成功", order.Id); // ✅ 回傳成功結果
        }

        public async Task<List<OrderDto>> GetMyOrdersAsync(int userId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string sql = @"SELECT * FROM Orders WHERE UserId = @UserId ORDER BY CreatedAt DESC";
            var orders = await conn.QueryAsync<Order>(sql, new { UserId = userId });

            // 直接回傳 List<OrderDto>，不需要包 Ok()
            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                CreatedAt = o.CreatedAt,
                TotalAmount = o.TotalAmount,
                Status = o.Status,                
                Details = new List<OrderDetailDto>()
            }).ToList();
        }
        public async Task<OrderDto?> GetOrderByIdAsync(int orderId, int userId)
        {
            using var conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string sql = @"
                SELECT * FROM Orders WHERE Id = @Id AND UserId = @UserId;
                SELECT * FROM OrderDetails WHERE OrderId = @Id;
            ";
            using var multi = await conn.QueryMultipleAsync(sql, new { Id = orderId, UserId = userId });
            var order = await multi.ReadFirstOrDefaultAsync<Order>();
            if (order == null) return null; // ✅ 找不到就回傳 null，不要回傳 NotFound()
            var details = await multi.ReadAsync<OrderDetail>();
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                TotalAmount = order.TotalAmount,
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
        }
    }
}