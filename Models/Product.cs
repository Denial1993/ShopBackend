using System.ComponentModel.DataAnnotations.Schema; // 為了設定欄位屬性

namespace ShopApi.Models
{
    public class Product
    {
        /// <summary>
        /// 主鍵 (PK)
        /// </summary>
        public int Id { get; set; } 
        /// <summary>
        /// 商品標題
        /// </summary>
        public string Title { get; set; } = string.Empty; 
        /// <summary>
        /// 商品描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        //使用 decimal (18,2) 來存錢是最標準的做法，不要用 float/double 會失準
        [Column(TypeName = "decimal(18,2)")] 
        public decimal Price { get; set; }
        /// <summary>
        /// 圖片網址
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        // --- 以下是關聯設定 (Foreign Key) ---
        // 這是外鍵 (FK)，對應到 Category 表的 Id
        public int CategoryId { get; set; }

        // 導覽屬性
        // 意思：這是一個「分類」物件。
        // 當我們查商品時，EF Core 可以順便幫我們把「手機」這個分類物件抓進來
        public Category? Category { get; set; }
    }
}