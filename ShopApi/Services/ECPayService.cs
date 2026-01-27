using System.Security.Cryptography;
using System.Text;
using System.Web;
using ShopApi.Dtos;

namespace ShopApi.Services
{
    public class ECPayService
    {
        private readonly IConfiguration _configuration;

        public ECPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public PaymentRequestDto GetPaymentRequest(string orderId, int amount, string itemName)
        {
            var merchantId = "2000132"; // 測試帳號
            var hashKey = "5294y06JbISpM5x9"; // 測試 Key
            var hashIV = "v77hoKGq4kWxNNIS"; // 測試 IV
            var baseUrl = "https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5"; // 測試網址

            // 1. 準備基本參數
            var tradeNo = orderId + new Random().Next(0, 99999).ToString(); // 訂單編號不能重複，所以加個亂數
            var request = new PaymentRequestDto
            {
                MerchantID = merchantId,
                MerchantTradeNo = tradeNo,
                MerchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                PaymentType = "aio",
                TotalAmount = amount.ToString(),
                TradeDesc = "ShopDemo商城購物",
                ItemName = itemName, // 多個商品用 # 分隔
                ReturnURL = $"{_configuration["AppUrl"]}/api/Payment/Callback", // 綠界會 POST 結果到這裡
                ClientBackURL = "http://localhost:5173/orders", // 刷卡完跳轉回前端的網址
                ChoosePayment = "Credit", // 預設信用卡
                EncryptType = "1"
            };

            // 2. 計算 CheckMacValue (最關鍵的一步)
            request.CheckMacValue = GenerateCheckMacValue(request, hashKey, hashIV);

            return request;
        }

        // 🔐 綠界加密邏輯 (不用背，複製就好)
        private string GenerateCheckMacValue(PaymentRequestDto request, string hashKey, string hashIV)
        {
            // 1. 把所有參數依照 A-Z 排序
            var parameters = new Dictionary<string, string>
            {
                { "MerchantID", request.MerchantID },
                { "MerchantTradeNo", request.MerchantTradeNo },
                { "MerchantTradeDate", request.MerchantTradeDate },
                { "PaymentType", request.PaymentType },
                { "TotalAmount", request.TotalAmount },
                { "TradeDesc", request.TradeDesc },
                { "ItemName", request.ItemName },
                { "ReturnURL", request.ReturnURL },
                { "ChoosePayment", request.ChoosePayment },
                { "EncryptType", request.EncryptType },
                { "ClientBackURL", request.ClientBackURL }
            };

            // 2. 串接成字串: HashKey=xxx&ItemName=yyy...
            var sortedKeys = parameters.Keys.OrderBy(k => k).ToList();
            var sb = new StringBuilder();
            sb.Append($"HashKey={hashKey}");
            
            foreach (var key in sortedKeys)
            {
                sb.Append($"&{key}={parameters[key]}");
            }
            
            sb.Append($"&HashIV={hashIV}");

            // 3. URL Encode
            var raw = sb.ToString();
            var encoded = HttpUtility.UrlEncode(raw).ToLower();

            // 綠界的轉碼規則有點怪，需要手動修正一些符號
            encoded = encoded.Replace("%2d", "-")
                             .Replace("%5f", "_")
                             .Replace("%2e", ".")
                             .Replace("%21", "!")
                             .Replace("%2a", "*")
                             .Replace("%28", "(")
                             .Replace("%29", ")");

            // 4. 轉 SHA256 並轉大寫
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(encoded);
                var hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToUpper();
            }
        }
    }
}