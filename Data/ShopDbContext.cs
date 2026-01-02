using Microsoft.EntityFrameworkCore;
using ShopApi.Models; 

namespace ShopApi.Data
{
    // 繼承 DbContext，這就是我們的「資料庫大管家」
    public class ShopDbContext : DbContext
    {
        // 建構子：這段是固定的咒語
        // 它會把外面的設定 (例如連線字串) 傳進來給父類別使用
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {
        }

        // 以後我們的資料表 (Table) 會定義在這裡
        // 例如：public DbSet<Product> Products { get; set; }
        // 目前先留空，等下一階段再來加
    }
}