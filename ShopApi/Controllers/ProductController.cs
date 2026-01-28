using Microsoft.AspNetCore.Mvc;
using ShopApi.Dtos;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;

        // GET: api/Product
        // 參數說明：
        // keyword: 搜尋關鍵字 (預設空)
        // page: 第幾頁 (預設 1)
        // pageSize: 一頁幾筆 (預設 6)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
            [FromQuery] string? keyword,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100)
        {
            var products = await _productService.GetProductsAsync(keyword, page, pageSize);
            return Ok(products);
        }

        // GET: api/Product/5
        /// <summary>
        /// 查詢單一商品詳情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound("找不到該商品");
            return Ok(product);
        }
    }
}