namespace ShopApi.Services
{
    public interface IOrderService
    {
        Task<string> Checkout();
        Task<string> GetMyOrders();
        Task<string> GetOrderDetail(int orderId);
        Task<int> GetUserId();
    }
}