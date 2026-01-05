namespace ShopApi.Dtos
{
    public class CartDto
    {
        public int Id { get; set; } // 購物車 Id
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();

        // 整台車的總金額 (把裡面每個小計加起來)
        public decimal TotalAmount => Items.Sum(i => i.SubTotal);
    }
}