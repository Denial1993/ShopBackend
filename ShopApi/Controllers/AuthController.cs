using Microsoft.AspNetCore.Mvc;
using ShopApi.Data;
using ShopApi.Dtos;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(ShopDbContext context, IConfiguration configuration, IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 註冊
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserDto request)
        {
            var result = await _authService.RegisterAsync(request);

            if (result != "註冊成功")
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// 登入
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var token = await _authService.LoginAsync(request);
            if (token == null) // 👈 服務生看到廚師給 null
            {
                return BadRequest("帳號或密碼錯誤"); // 👈 服務生才負責說 BadRequest
            }
            return Ok(token);
        }
    }
}