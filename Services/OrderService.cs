using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;

namespace ShopApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly ShopDbContext _context;
        private readonly IConfiguration _configuration;

        public OrderService(ShopDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> Checkout()
        {
            var userId = GetUserId();

            // A. 撈出購物車 (要包含商品資訊，因為要抓價格)
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.Items.Count == 0)
            {
                return null;
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
                    return null;
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

        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
        {
            throw new NotImplementedException("還沒好");
        }
        
        public async Task<ActionResult> GetOrderDetail(int orderId)
        {
            throw new NotImplementedException("還沒好");
        }

        // 小工具：取得 User ID
        private Task<int> GetUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idClaim == null) throw new Exception("請重新登入");
            return int.Parse(idClaim.Value);
        }
    }
}