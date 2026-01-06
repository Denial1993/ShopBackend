namespace ShopApi.Dtos
{
    /// <summary>
    /// 加入購物車
    /// </summary>
    public class AddToCartDto
    {
        /// <summary>
        /// 產品編號
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 數量
        /// </summary>
        /// <example>1</example>
        public int Quantity { get; set; } = 1; // 預設 1 個
    }
}