using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;
using ShopApi.Dtos;
using Microsoft.Extensions.Configuration;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminProductController(ShopDbContext context, Supabase.Client supabaseFunc, IConfiguration configuration) : ControllerBase
    {
        private readonly ShopDbContext _context = context;
        // Rename to avoid conflicts if previously used _environment
        private readonly Supabase.Client _supabase = supabaseFunc;
        private readonly IConfiguration _configuration = configuration;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProducts()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Price,
                    p.ImageUrl,
                    p.Stock,
                    p.Description,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : "無分類"
                })
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromForm] ProductUploadDto dto)
        {
            // Default image if none provided
            string imageUrl = "https://placehold.co/600x400?text=No+Image";

            if (dto.Image != null)
            {
                imageUrl = await SaveImage(dto.Image);
            }

            var product = new Product
            {
                Title = dto.Title,
                Price = dto.Price,
                Description = dto.Description ?? "",
                Stock = dto.Stock,
                CategoryId = dto.CategoryId,
                ImageUrl = imageUrl
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUploadDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            product.Title = dto.Title;
            product.Price = dto.Price;
            product.Description = dto.Description ?? "";
            product.Stock = dto.Stock;
            product.CategoryId = dto.CategoryId;

            if (dto.Image != null)
            {
                product.ImageUrl = await SaveImage(dto.Image);
            }

            await _context.SaveChangesAsync();
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";

            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            // Bucket name from config or hardcoded default
            var bucketName = "products";

            // Upload to Supabase Storage
            await _supabase.Storage
                .From(bucketName)
                .Upload(fileBytes, fileName, new Supabase.Storage.FileOptions { Upsert = false });

            // Ensure we return the full URL
            var supabaseUrl = _configuration["Supabase:Url"];

            // DEBUG LOGGING
            Console.WriteLine($"[DEBUG] Supabase:Url from config: '{supabaseUrl}'");

            // Remove trailing slash if present
            if (supabaseUrl != null && supabaseUrl.EndsWith("/"))
            {
                supabaseUrl = supabaseUrl[..^1];
            }

            var publicUrl = $"{supabaseUrl}/storage/v1/object/public/{bucketName}/{fileName}";

            return publicUrl;
        }
    }
}
