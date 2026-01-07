using ShopApi.Dtos;

namespace ShopApi.Services
{
    public interface IOrderService
    {
        //結帳
        // 參數: userId (從 Controller 傳進來)
        // 回傳: (是否成功, 錯誤訊息, 訂單ID) -> 用 Tuple 最方便
        Task<(bool IsSuccess,string Message,int OrderId)> CheckoutAsync(int userId);
        //查列表
        Task<List<OrderDto>> GetMyOrdersAsync(int userId);
        //查單筆
        Task<OrderDto?> GetOrderByIdAsync(int orderId,int userId);
    }
}