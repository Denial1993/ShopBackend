using ShopApi.Dtos;

namespace ShopApi.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProductsAsync(string? keyword,  int page = 1,  int pageSize = 6);
        Task<ProductDto?> GetProductByIdAsync(int id);
    }
}