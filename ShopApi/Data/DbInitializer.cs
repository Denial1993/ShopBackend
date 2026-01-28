using ShopApi.Models;

namespace ShopApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ShopDbContext context)
        {
            // 確保資料庫已建立
            context.Database.EnsureCreated();

            // 清空舊資料，重新建立 (開發階段用)
            if (context.Products.Any())
            {
                context.Products.RemoveRange(context.Products);
                context.SaveChanges();
            }
            if (context.Categories.Any())
            {
                context.Categories.RemoveRange(context.Categories);
                context.SaveChanges();
            }
            if (context.Roles.Any())
            {
                context.Roles.RemoveRange(context.Roles);
                context.SaveChanges();
            }

            // 0. 建立角色
            var roles = new Role[]
            {
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Staff" },
                new Role { Id = 3, Name = "User" }
            };
            context.Roles.AddRange(roles);
            context.SaveChanges();

            // 1. 建立分類
            var categories = new Category[]
            {
                new Category { Name = "手機" },
                new Category { Name = "筆電" },
                new Category { Name = "配件" },
                new Category { Name = "耳機" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges(); // 先存分類，這樣才有 ID 給商品用

            // 2. 建立商品
            var products = new Product[]
            {
                // 手機
                new Product { Title = "iPhone 15", Description = "蘋果最新手機，搭載 A17 Pro 晶片，支援 Dynamic Island 動態島，拍照更強大", Price = 29900, Stock = 50, ImageUrl = "iphone.jpg", Category = categories[0] },
                new Product { Title = "Samsung S24", Description = "安卓機皇，搭載 Snapdragon 8 Gen 3 處理器，200MP 超高畫素相機", Price = 27900, Stock = 45, ImageUrl = "samsung.jpg", Category = categories[0] },
                new Product { Title = "iPhone 15 Pro Max", Description = "頂級旗艦機，鈦金屬機身，120Hz ProMotion 螢幕，超強夜拍能力", Price = 42900, Stock = 30, ImageUrl = "iphone.jpg", Category = categories[0] },
                new Product { Title = "Google Pixel 8 Pro", Description = "Google 最新旗艦，AI 相機功能超強，原生 Android 體驗", Price = 31900, Stock = 40, ImageUrl = "samsung.jpg", Category = categories[0] },
                
                // 筆電
                new Product { Title = "MacBook Air M3", Description = "輕薄好帶，搭載 M3 晶片，續航力超強，適合商務人士", Price = 35900, Stock = 25, ImageUrl = "macbook.jpg", Category = categories[1] },
                new Product { Title = "MacBook Pro 14\"", Description = "創作者首選，搭載 M3 Pro 晶片，專業級效能", Price = 59900, Stock = 20, ImageUrl = "macbook.jpg", Category = categories[1] },
                new Product { Title = "Dell XPS 15", Description = "Windows 旗艦筆電，4K OLED 螢幕，Intel Core i9 處理器", Price = 49900, Stock = 15, ImageUrl = "macbook.jpg", Category = categories[1] },
                
                // 配件
                new Product { Title = "USB-C 充電線", Description = "快充專用，支援 100W PD 快充，耐用編織線材", Price = 490, Stock = 200, ImageUrl = "cable.jpg", Category = categories[2] },
                new Product { Title = "無線充電盤", Description = "Qi 無線充電，支援 iPhone 和 Android，15W 快充", Price = 890, Stock = 150, ImageUrl = "cable.jpg", Category = categories[2] },
                new Product { Title = "MagSafe 行動電源", Description = "10000mAh 磁吸行動電源，支援 iPhone 12 以上機型", Price = 1490, Stock = 100, ImageUrl = "cable.jpg", Category = categories[2] },
                
                // 耳機 (11款)
                new Product { Title = "AirPods Pro 2", Description = "主動降噪，自適應通透模式，空間音訊，USB-C 充電盒", Price = 7990, Stock = 80, ImageUrl = "cable.jpg", Category = categories[3] },
                new Product { Title = "Sony WH-1000XM5", Description = "業界頂級降噪，40 小時續航，LDAC 高音質傳輸", Price = 10900, Stock = 60, ImageUrl = "cable.jpg", Category = categories[3] },
                new Product { Title = "Bose QuietComfort 45", Description = "舒適配戴，優異降噪，清晰通話品質", Price = 9900, Stock = 50, ImageUrl = "cable.jpg", Category = categories[3] },
                new Product { Title = "Samsung Galaxy Buds Pro", Description = "智慧降噪，IPX7 防水，360 度環繞音效", Price = 4990, Stock = 90, ImageUrl = "cable.jpg", Category = categories[3] },
                new Product { Title = "Beats Studio Pro", Description = "個性化空間音訊，USB-C 有線無損聆聽，40 小時續航", Price = 10900, Stock = 40, ImageUrl = "cable.jpg", Category = categories[3] },
                new Product { Title = "Sennheiser Momentum 4", Description = "發燒級音質，60 小時超長續航，專業調音", Price = 11900, Stock = 35, ImageUrl = "cable.jpg", Category = categories[3] },
                new Product { Title = "JBL Tune 770NC", Description = "高 CP 值降噪耳機，70 小時續航，輕量設計", Price = 3490, Stock = 100, ImageUrl = "cable.jpg", Category = categories[3] },
                new Product { Title = "Audio-Technica ATH-M50x", Description = "專業監聽耳機，錄音室級音質，可換線設計", Price = 4990, Stock = 70, ImageUrl = "cable.jpg", Category = categories[3] },
                new Product { Title = "Jabra Elite 85t", Description = "進階降噪真無線，多點連線，HearThrough 通透模式", Price = 6490, Stock = 55, ImageUrl = "cable.jpg", Category = categories[3] },
                new Product { Title = "Marshall Major IV", Description = "經典搖滾風格，80+ 小時續航，可折疊設計", Price = 3990, Stock = 65, ImageUrl = "cable.jpg", Category = categories[3] },
                new Product { Title = "Anker Soundcore Q30", Description = "平價降噪首選，40 小時續航，三種降噪模式", Price = 2490, Stock = 120, ImageUrl = "cable.jpg", Category = categories[3] },
            };
            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}