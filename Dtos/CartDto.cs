namespace ShopApi.Dtos
{
    /// <summary>
    /// 購物車
    /// </summary>
    public class CartDto
    {
        /// <summary>
        /// 購物車編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 購物車裡面的明細清單
        /// </summary>
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        /// <summary>
        /// 整台車的總金額 (把裡面每個小計加起來)
        /// </summary>
        public decimal TotalAmount => Items.Sum(i => i.SubTotal);
    }
}