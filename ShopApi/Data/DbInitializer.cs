using ShopApi.Models;

namespace ShopApi.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ShopDbContext context)
        {
            // 確保資料庫已建立
            context.Database.EnsureCreated();

            // 如果已經有商品，就不做事 (避免重複塞)
            if (context.Products.Any())
            {
                return;
            }

            // 1. 建立分類
            var categories = new Category[]
            {
                new Category { Name = "手機" },
                new Category { Name = "筆電" },
                new Category { Name = "配件" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges(); // 先存分類，這樣才有 ID 給商品用

            // 2. 建立商品
            var products = new Product[]
            {
                new Product { Title = "iPhone 15", Description = "蘋果最新手機", Price = 29900, ImageUrl = "iphone.jpg", Category = categories[0] },
                new Product { Title = "Samsung S24", Description = "安卓機皇", Price = 27900, ImageUrl = "samsung.jpg", Category = categories[0] },
                new Product { Title = "MacBook Air", Description = "輕薄好帶", Price = 35900, ImageUrl = "macbook.jpg", Category = categories[1] },
                new Product { Title = "USB-C 充電線", Description = "快充專用", Price = 490, ImageUrl = "cable.jpg", Category = categories[2] },
                // 你可以自己多複製幾行...
            };
            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}