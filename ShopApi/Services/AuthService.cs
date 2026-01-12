using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopApi.Data;
using ShopApi.Dtos;
using ShopApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ShopApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly ShopDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ShopDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> RegisterAsync(UserDto request)
        {
            // 1. 檢查 Email 是否存在
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return "Email 已經被註冊過了";
            }

            // 2. 加密 & 存檔
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                Role = "User" // 預設是一般會員
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "註冊成功";
        }
        
        public async Task<string?> LoginAsync(UserDto request)
        {            
             // 1. 找使用者
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
               return null; // 👈 直接回傳 null，不要用 BadRequest
            }
            // 驗證成功，發 Token
            return CreateToken(user);
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