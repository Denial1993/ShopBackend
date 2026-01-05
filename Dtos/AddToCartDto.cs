namespace ShopApi.Dtos
{
    public class AddToCartDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1; // 預設 1 個
    }
}