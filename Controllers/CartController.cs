using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Dtos;
using ShopApi.Models;
using ShopApi.Services;
//測試
namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🔐 重點！掛上這個鎖，沒帶 Token 的人連門都進不來
    public class CartController(ICartService cartService) : ControllerBase
    {
        private readonly ICartService _cartService = cartService;

        // 取得我的購物車
        // GET: api/Cart
        [HttpGet]
        public async Task<ActionResult<CartDto>> GetMyCart()
        {
            var userId = GetUserId(); // 取得當前登入者的 ID

            var cart = await _cartService.GetMyCartAsync(userId);
            if (cart == null) return new CartDto();
            return Ok(cart);
        }

        // 加入購物車
        // POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<string>> AddToCart(AddToCartDto request)
        {
            var userId = GetUserId();

            var message = await _cartService.AddToCartAsync(request, userId);
            return Ok(message);
        }

        // 移除購物車某個項目
        [HttpDelete("item/{itemId}")]
        public async Task<ActionResult> RemoveItem(int itemId)
        {
            var userId = GetUserId();
            var isSuccess = await _cartService.RemoveItemAsync(itemId, userId);
            if (!isSuccess)
            {
                return NotFound("找不到該項目"); // 找不到回傳 404
            }

            return Ok("已移除");
        }

        // 清空購物車
        // DELETE: api/Cart
        [HttpDelete]
        public async Task<ActionResult> ClearCart()
        {
            var userId = GetUserId();
            await _cartService.ClearCartAsync(userId);

            return Ok("購物車已清空");
        }

        // --- 小工具 ---
        // 從 JWT Token 中解析出 User Id
        private int GetUserId()
        {
            // User.Claims 是 ASP.NET Core 自動幫我們從 Token 解密出來的資訊
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (idClaim == null)
            {
                throw new Exception("Token 裡沒有 User ID，請重新登入");
            }
            return int.Parse(idClaim.Value);
        }
    }
}