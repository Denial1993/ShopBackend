using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShopApi.Data;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public HealthController(ShopDbContext context)
        {
            _context = context;
        }

        // GET: api/health
        [HttpGet]
        public async Task<IActionResult> CheckHealth()
        {
            try
            {
                // 關鍵點：執行一個極輕量的 SQL 查詢 "SELECT 1"
                // 這會強迫 Render 建立與 Supabase 的連線，但幾乎不消耗資源
                await _context.Database.ExecuteSqlRawAsync("SELECT 1");

                return Ok(new { status = "Healthy", database = "Connected" });
            }
            catch (Exception ex)
            {
                // 即使資料庫連不上，至少 Render 是醒著的，但要回報錯誤
                return StatusCode(503, new { status = "Unhealthy", error = ex.Message });
            }
        }
    }
}