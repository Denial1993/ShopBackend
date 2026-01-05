namespace ShopApi.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        // 屬於哪一台購物車
        public int CartId { get; set; }
        public Cart? Cart { get; set; }

        // 買了哪個商品
        public int ProductId { get; set; }
        public Product? Product { get; set; } // 為了之後能抓到商品名稱和價格

        // 買了幾個
        public int Quantity { get; set; }
    }
}