namespace ShopApi.Dtos
{
    /// <summary>
    /// 訂單
    /// </summary>
    public class OrderDto
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 訂單_使用者編號
        /// </summary>
        public int UserId { get; set; } // 誰買的
        /// <summary>
        /// 訂單建立時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now; // 什麼時候買的
        /// <summary>
        /// 訂單總金額
        /// </summary>
        public decimal TotalAmount { get; set; } // 總金額 (當時結帳的錢)
        /// <summary>
        /// 訂單明細清單
        /// </summary>
        public List<OrderDetailDto> Details { get; set; } = [];
    }
}