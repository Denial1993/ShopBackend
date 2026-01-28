using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public CategoryController(ShopDbContext context)
        {
            _context = context;
        }

        // GET: api/Category
        // 取得所有分類列表
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.Id)
                .ToListAsync();

            return Ok(categories);
        }
    }
}
