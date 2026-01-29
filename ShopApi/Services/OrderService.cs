using System.Security.Claims;
using Dapper;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Dtos;
using ShopApi.Models;
using Microsoft.Data.SqlClient;
using Npgsql;

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

            // E. 清空購物車
            _context.CartItems.RemoveRange(cart.Items);

            await _context.SaveChangesAsync();
            return (true, "結帳成功", order.Id);
        }

        public async Task<List<OrderDto>> GetMyOrdersAsync(int userId)
        {
            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string sql = @"SELECT * FROM ""Orders"" WHERE ""UserId"" = @UserId ORDER BY ""CreatedAt"" DESC";
            var orders = await conn.QueryAsync<Order>(sql, new { UserId = userId });

            return orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                CreatedAt = o.CreatedAt.ToLocalTime(),
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                Details = new List<OrderDetailDto>()
            }).ToList();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId, int userId)
        {
            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string sql = @"
                SELECT * FROM ""Orders"" WHERE ""Id"" = @Id AND ""UserId"" = @UserId;
                SELECT od.*, p.""ImageUrl"" 
                FROM ""OrderDetails"" od
                LEFT JOIN ""Products"" p ON od.""ProductId"" = p.""Id""
                WHERE od.""OrderId"" = @Id;
            ";
            using var multi = await conn.QueryMultipleAsync(sql, new { Id = orderId, UserId = userId });
            var order = await multi.ReadFirstOrDefaultAsync<Order>();
            if (order == null) return null;
            var details = await multi.ReadAsync<dynamic>();
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt.ToLocalTime(),
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Details = details.Select(d => new OrderDetailDto
                {
                    Id = d.Id,
                    OrderId = d.OrderId,
                    ProductId = d.ProductId,
                    ProductTitle = d.ProductTitle,
                    Price = d.Price,
                    Quantity = d.Quantity,
                    ImageUrl = d.ImageUrl
                }).ToList()
            };
        }

        public async Task<List<AdminOrderDto>> GetAllOrdersAsync()
        {
            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string sql = @"
                SELECT o.*, u.""Email"" as ""UserEmail"", u.""FullName"" as ""UserFullName""
                FROM ""Orders"" o
                JOIN ""Users"" u ON o.""UserId"" = u.""Id""
                ORDER BY o.""CreatedAt"" DESC";

            var orders = await conn.QueryAsync<dynamic>(sql);

            return orders.Select(o => new AdminOrderDto
            {
                Id = (int)o.Id,
                UserId = (int)o.UserId,
                CreatedAt = ((DateTime)o.CreatedAt).ToLocalTime(),
                TotalAmount = (decimal)o.TotalAmount,
                Status = (string)o.Status,
                UserEmail = (string)o.UserEmail,
                UserFullName = (string)o.UserFullName,
                Details = new List<OrderDetailDto>()
            }).ToList();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId)
        {
            using var conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            string sql = @"
                SELECT * FROM ""Orders"" WHERE ""Id"" = @Id;
                SELECT od.*, p.""ImageUrl"" 
                FROM ""OrderDetails"" od
                LEFT JOIN ""Products"" p ON od.""ProductId"" = p.""Id""
                WHERE od.""OrderId"" = @Id;
            ";
            using var multi = await conn.QueryMultipleAsync(sql, new { Id = orderId });
            var order = await multi.ReadFirstOrDefaultAsync<Order>();
            if (order == null) return null;
            var details = await multi.ReadAsync<dynamic>();
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt.ToLocalTime(),
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Details = details.Select(d => new OrderDetailDto
                {
                    Id = d.Id,
                    OrderId = d.OrderId,
                    ProductId = d.ProductId,
                    ProductTitle = d.ProductTitle,
                    Price = d.Price,
                    Quantity = d.Quantity,
                    ImageUrl = d.ImageUrl
                }).ToList()
            };
        }
    }
}