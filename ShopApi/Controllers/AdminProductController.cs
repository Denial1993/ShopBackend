using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;
using ShopApi.Dtos;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class AdminProductController(ShopDbContext context, IWebHostEnvironment environment) : ControllerBase
    {
        private readonly ShopDbContext _context = context;
        private readonly IWebHostEnvironment _environment = environment;

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
            string imageUrl = "cable.jpg"; // Default

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
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(_environment.ContentRootPath, "..", "shop-frontend", "public", "images", fileName);

            // Ensure directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return fileName;
        }
    }
}
