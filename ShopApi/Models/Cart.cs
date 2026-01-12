namespace ShopApi.Models
{
    public class Cart
    {
        public int Id { get; set; }

        // 這一台車是屬於哪位會員的
        public int UserId { get; set; } 

        // 導覽屬性：這台車裡裝了哪些東西
        public List<CartItem> Items { get; set; } = new List<CartItem>();
    }
}