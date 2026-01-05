using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Dtos;
using ShopApi.Models;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 🔐 重點！掛上這個鎖，沒帶 Token 的人連門都進不來
    public class CartController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public CartController(ShopDbContext context)
        {
            _context = context;
        }

        // 取得我的購物車
        // GET: api/Cart
        [HttpGet]
        public async Task<ActionResult<CartDto>> GetMyCart()
        {
            var userId = GetUserId(); // 取得當前登入者的 ID

            // 撈出這個人的購物車，順便把裡面的商品資訊 (Product) 一起抓出來
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // 如果他還沒購物車，就回傳一個空的 DTO
            if (cart == null)
            {
                return Ok(new CartDto());
            }

            // 把資料轉成 DTO
            var dto = new CartDto
            {
                Id = cart.Id,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductTitle = i.Product!.Title,
                    Price = i.Product.Price,
                    Quantity = i.Quantity
                }).ToList()
            };

            return Ok(dto);
        }

        // 加入購物車
        // POST: api/Cart
        [HttpPost]
        public async Task<ActionResult<string>> AddToCart(AddToCartDto request)
        {
            var userId = GetUserId();

            // 1. 先確認這個人有沒有購物車？沒有就幫他創一台
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync(); // 先存，才有 CartId
            }

            // 2. 檢查購物車裡是不是已經有這個商品了？
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

            if (existingItem != null)
            {
                // 如果有，就加數量
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                // 如果沒有，就新增一筆
                var newItem = new CartItem
                {
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };
                cart.Items.Add(newItem);
            }

            await _context.SaveChangesAsync();
            return Ok("加入成功");
        }

        // 移除購物車某個項目
        // DELETE: api/Cart/item/5
        [HttpDelete("item/{itemId}")]
        public async Task<ActionResult> RemoveItem(int itemId)
        {
            var userId = GetUserId();

            // 確保這個人只能刪除「自己」購物車裡的東西 (資安檢核)
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart!.UserId == userId);

            if (cartItem == null)
            {
                return NotFound("找不到該項目");
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok("已移除");
        }
        
        // 清空購物車
        // DELETE: api/Cart
        [HttpDelete] 
        public async Task<ActionResult> ClearCart()
        {
             var userId = GetUserId();
             var cart = await _context.Carts
                 .Include(c => c.Items)
                 .FirstOrDefaultAsync(c => c.UserId == userId);
             
             if (cart != null)
             {
                 _context.CartItems.RemoveRange(cart.Items); // 刪除所有明細
                 await _context.SaveChangesAsync();
             }
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