using System.Text.Json.Serialization;

namespace ShopApi.Dtos
{
    /// <summary>
    /// 訂單明細
    /// </summary>
    public class OrderDetailDto
    {
        /// <summary>
        /// 訂單明細編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 訂單_訂單編號
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 訂單_產品編號
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 訂單_產品名稱
        /// </summary>
        public string ProductTitle { get; set; } = string.Empty; // 這裡要「備份」當時的商品名稱
        /// <summary>
        /// 訂單_產品價格
        /// </summary>
        public decimal Price { get; set; } // 這裡要「備份」當時的價格 (因為商品以後可能會漲價)
        /// <summary>
        /// 訂單數量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 產品圖片
        /// </summary>
        public string? ImageUrl { get; set; }
    }
}