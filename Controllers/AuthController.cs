
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Data;
using ShopApi.Dtos;
using ShopApi.Models;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ShopDbContext _context;
        private readonly IConfiguration _configuration; // 用來讀取 User Secrets

        public AuthController(ShopDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            // 1. 檢查 Email 是否已被註冊
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Email 已經被註冊過了");
            }

            // 2. 加密密碼 (把牛肉變漢堡排)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. 建立使用者
            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = "User" // 預設是一般會員
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("註冊成功");
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            // 1. 找使用者
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return BadRequest("帳號或密碼錯誤"); // 防枚舉攻擊
            }

            // 2. 驗證密碼
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("帳號或密碼錯誤");
            }

            // 3. 製作 JWT Token (發手環)
            string token = CreateToken(user);
            return Ok(token);
        }

        /// <summary>
        /// 私人方法：製作 Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // 記住 ID 很重要！
                new Claim(ClaimTypes.Role, user.Role)
            };

            // 從 User Secrets 拿金鑰
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Key").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1), // 一天後過期
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}