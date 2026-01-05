namespace ShopApi.Dtos
{
    public class CartItemDto
    {
        public int Id { get; set; } // CartItem 的 Id (刪除用)
        public int ProductId { get; set; }
        public string ProductTitle { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // 小計 (單價 * 數量)
        public decimal SubTotal => Price * Quantity;
    }
}