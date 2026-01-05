namespace ShopApi.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        
        // 我們不回傳整個 Category 物件，只回傳分類名稱就好 (扁平化)
        public string CategoryName { get; set; } = string.Empty;
    }
}