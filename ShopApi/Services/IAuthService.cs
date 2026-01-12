using ShopApi.Dtos;
using ShopApi.Models;

namespace ShopApi.Services
{
    public interface IAuthService
    {
        // 定義這家餐廳提供什麼服務，不用寫實作
        Task<string> RegisterAsync(UserDto request); // 回傳字串代表錯誤訊息，null 代表成功
        /// <summary>
        /// IAuthService：登入功能
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<string?> LoginAsync(UserDto request);   
    }
}