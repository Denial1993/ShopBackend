namespace ShopApi.Dtos
{
    public class PaymentRequestDto
    {
        public string MerchantID { get; set; }
        public string MerchantTradeNo { get; set; }
        public string MerchantTradeDate { get; set; }
        public string PaymentType { get; set; }
        public string TotalAmount { get; set; }
        public string TradeDesc { get; set; }
        public string ItemName { get; set; }
        public string ReturnURL { get; set; }
        public string ChoosePayment { get; set; }
        public string EncryptType { get; set; }
        public string ClientBackURL { get; set; }
        public string CheckMacValue { get; set; }
        public string CustomField1 { get; set; }
    }
}