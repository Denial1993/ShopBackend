using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Dtos;
using ShopApi.Models;
using System.Security.Claims;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 必須登入才能存取
    public class ProfileController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public ProfileController(ShopDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取得個人資料
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null) return NotFound();

            return new UserProfileDto
            {
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Address = user.Address
            };
        }

        /// <summary>
        /// 更新個人資料
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UserProfileDto request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == int.Parse(userId));

            if (user == null) return NotFound();

            // 更新欄位 (Email 不允許修改)
            user.FullName = request.FullName;
            user.Phone = request.Phone;
            user.Address = request.Address;

            await _context.SaveChangesAsync();

            return Ok("個人資料已更新");
        }
    }
}
