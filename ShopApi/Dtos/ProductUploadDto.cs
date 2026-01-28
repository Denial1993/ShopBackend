using Microsoft.AspNetCore.Http;

namespace ShopApi.Dtos
{
    public class ProductUploadDto
    {
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int Stock { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? Image { get; set; }
    }
}
