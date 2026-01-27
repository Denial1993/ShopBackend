using System.ComponentModel.DataAnnotations.Schema; // 為了設定欄位屬性

namespace ShopApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; } // 誰買的
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // 什麼時候買的
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } // 總金額 (當時結帳的錢)
        public string Status { get; set; } = "Unpaid";//付款狀態
        public List<OrderDetail> Details { get; set; } = [];
    }
}