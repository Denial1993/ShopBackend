using System.ComponentModel.DataAnnotations;

namespace ShopApi.Dtos
{
    /// <summary>
    /// 購物車明細的資料
    /// </summary>
    public class CartItemDto
    {
        /// <summary>
        /// 購物車明細編號
        /// </summary>
        public int Id { get; set; } // CartItem 的 Id (刪除用)
        /// <summary>
        /// 購物車明細_產品編號
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 購物車明細_產品名稱
        /// </summary>
        [Required]
        public string ProductTitle { get; set; } = string.Empty;
        /// <summary>
        /// 購物車明細_產品價格
        /// </summary>
        [Required]
        public decimal Price { get; set; }
        /// <summary>
        /// 購物車明細_數量
        /// </summary>
        [Required]
        public int Quantity { get; set; }
        /// <summary>
        /// 小計 (單價 * 數量)
        /// </summary>
        [Required]
        public decimal SubTotal => Price * Quantity;
    }
}