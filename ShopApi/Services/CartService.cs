using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Dtos;
using ShopApi.Models;

namespace ShopApi.Services
{
    public class CartService(ShopDbContext context) : ICartService
    {
        private readonly ShopDbContext _context = context;
        public async Task<CartDto?> GetMyCartAsync(int userId)
        {
            // 撈出這個人的購物車，順便把裡面的商品資訊 (Product) 一起抓出來
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // 如果他還沒購物車，就回傳一個空的 DTO
            if (cart == null)
            {
                return null;
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
                    Quantity = i.Quantity,
                    ImageUrl = i.Product.ImageUrl
                }).ToList()
            };
            return dto;
        }
        public async Task<string> AddToCartAsync(AddToCartDto request, int userId)
        {
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
            return "加入購物車成功";
        }
        public async Task<bool> RemoveItemAsync(int itemId, int userId)
        {
            // 確保這個人只能刪除「自己」購物車裡的東西 (資安檢核)
            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.Cart!.UserId == userId);

            if (cartItem == null)
            {
                return false;
            }

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task ClearCartAsync(int userId)
        {
            var cart = await _context.Carts
                 .Include(c => c.Items)
                 .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.Items); // 刪除所有明細
                await _context.SaveChangesAsync();
            }
        }
    }
}