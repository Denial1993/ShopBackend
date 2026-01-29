namespace ShopApi.Dtos
{
    public class AdminOrderDto : OrderDto
    {
        public string UserEmail { get; set; } = string.Empty;
        public string? UserFullName { get; set; }
    }
}
