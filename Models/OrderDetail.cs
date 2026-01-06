using System.ComponentModel.DataAnnotations.Schema; // 為了設定欄位屬性
using System.Text.Json.Serialization;             // 👈 補上這行 (給 [JsonIgnore] 用的)

namespace ShopApi.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        [JsonIgnore] // (建議加上這個，避免查詢迴圈，如果沒加也沒關係)
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public string ProductTitle { get; set; } = string.Empty; // 這裡要「備份」當時的商品名稱
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; } // 這裡要「備份」當時的價格 (因為商品以後可能會漲價)
        public int Quantity { get; set; }
    }
}