using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // 記得引用，才能用 Include, ToListAsync
using ShopApi.Data;
using ShopApi.Dtos;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ShopDbContext _context;

        // DI 注入：把大管家請進來
        public ProductController(ShopDbContext context)
        {
            _context = context;
        }

        // GET: api/Product
        // 參數說明：
        // keyword: 搜尋關鍵字 (預設空)
        // page: 第幾頁 (預設 1)
        // pageSize: 一頁幾筆 (預設 6)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 6)
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
                    CategoryName = p.Category != null ? p.Category.Name : "無分類"
                })
                .ToListAsync(); // 這裡才會真正去資料庫撈資料 (執行 SQL)

            return Ok(products);
        }

        // GET: api/Product/5
        // 查詢單一商品詳情
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound("找不到該商品"); // 回傳 404
            }

            // 手動轉成 DTO
            var dto = new ProductDto
            {
                Id = product.Id,
                Title = product.Title,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryName = product.Category?.Name ?? "無分類"
            };

            return Ok(dto);
        }
    }
}