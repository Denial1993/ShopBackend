using ShopApi.Dtos;

namespace ShopApi.Services
{
    public interface ICartService
    {
        Task<CartDto?> GetMyCartAsync(int userId);
        Task<string> AddToCartAsync(AddToCartDto request,int userId);
        Task<bool> RemoveItemAsync(int itemId,int userId);
        
        Task ClearCartAsync(int userId);
    }
}