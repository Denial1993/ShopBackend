using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Dtos;

namespace ShopApi.Services
{
    public class ProductService(ShopDbContext context) : IProductService
    {
        private readonly ShopDbContext _context = context;

        public async Task<IEnumerable<ProductDto>> GetProductsAsync(string? keyword, int page = 1, int pageSize = 100)
        {
            // 1. 準備查詢 (尚未對資料庫發送請求，只是在組 SQL)
            var query = _context.Products
                .Include(p => p.Category) // 重要！要把關聯的分類也抓進來
                .AsQueryable();

            // 2. 關鍵字搜尋 (如果有輸入的話)
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Title.Contains(keyword) ||
                                         p.Description.Contains(keyword));
            }

            // 3. 分頁邏輯 (Skip & Take)
            // page 1: Skip(0), Take(6)
            // page 2: Skip(6), Take(6)
            var products = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto // 4. 投影 (Projection)：把 Product 轉成 ProductDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    CategoryName = p.Category != null ? p.Category.Name : "無分類",
                    Description = p.Description
                })
                .ToListAsync(); // 這裡才會真正去資料庫撈資料 (執行 SQL)

            return products;
        }
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return null;

            // 手動轉成 DTO
            var dto = new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category?.Name ?? "無分類",
                Description = product.Description
            };
            return dto;
        }
    }
}