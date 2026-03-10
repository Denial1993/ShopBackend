using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using ShopApi.Data;
using System.Text.Json;

namespace ShopApi.Plugins;

public class SearchShopProductsPlugin
{
    private readonly ShopDbContext _db;

    public SearchShopProductsPlugin(ShopDbContext db)
    {
        _db = db;
    }

    [KernelFunction("SearchShopProducts")]
    [Description("搜尋 PawPals 的實體商品或服務。當使用者詢問是否有賣某些商品時，請呼叫此工具。")]
    public async Task<string> SearchShopProductsAsync(
        [Description("要搜尋的商品關鍵字，例如：貓、防蚤、洗劑、推車")] string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword)) return "[]";

        try
        {
            var products = await _db.Products
                .Where(p => p.Title.Contains(keyword) || p.Description.Contains(keyword))
                .Select(p => new
                {
                    商品名稱 = p.Title,
                    價格 = p.Price,
                    描述 = p.Description
                })
                .Take(5)
                .ToListAsync();

            return JsonSerializer.Serialize(products);
        }
        catch (Exception)
        {
            return "[]";
        }
    }
}
