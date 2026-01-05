namespace ShopApi.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        // public Order? Order { get; set; }

        public int ProductId { get; set; }
        public string ProductTitle { get; set; } = string.Empty; // 這裡要「備份」當時的商品名稱
        public decimal Price { get; set; } // 這裡要「備份」當時的價格 (因為商品以後可能會漲價)
        public int Quantity { get; set; }
    }
}