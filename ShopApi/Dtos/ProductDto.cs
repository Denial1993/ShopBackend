using System.ComponentModel.DataAnnotations;

namespace ShopApi.Dtos
{
    /// <summary>
    /// 產品資料
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// 產品編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 產品名稱
        /// </summary>
        /// <example>可樂</example>
        [Required]
        public string? Title { get; set; }
        /// <summary>
        /// 產品價格
        /// </summary>
        [Required]
        public decimal? Price { get; set; }
        /// <summary>
        /// 產品圖片URL
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        // 我們不回傳整個 Category 物件，只回傳分類名稱就好 (扁平化)
        /// <summary>
        /// 分類名稱
        /// </summary>
        /// <example>手機</example>
        public string CategoryName { get; set; } = string.Empty;
        /// <summary>
        /// 產品描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}