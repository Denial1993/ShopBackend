using System.Text.Json.Serialization; // 為了之後 API 輸出不要無限迴圈

namespace ShopApi.Models
{
    public class Category
    {
        public int Id { get; set; } // 主鍵 (PK)

        public string Name { get; set; } = string.Empty; // 分類名稱 (例如: 手機, 筆電)

        // 導覽屬性 (Navigation Property)
        // 意思：一個分類下面，會有很多 (List) 商品
        // JsonIgnore: 之後 API 查詢分類時，不要把下面幾萬個商品都吐出來，避免資料太大
        [JsonIgnore]
        public List<Product>? Products { get; set; }
    }
}