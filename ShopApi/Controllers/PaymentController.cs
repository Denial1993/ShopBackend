using Microsoft.AspNetCore.Mvc;
using ShopApi.Dtos;
using ShopApi.Services;
using System.Text;
using ShopApi.Data; // 記得引用你的 DbContext
using Microsoft.EntityFrameworkCore;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ECPayService _ecPayService;
        private readonly ShopDbContext _context; // 用來查訂單金額

        public PaymentController(ECPayService ecPayService, ShopDbContext context)
        {
            _ecPayService = ecPayService;
            _context = context;
        }

        // POST: api/Payment/Checkout
        // 前端按下「前往付款」時呼叫這支
        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
        {
            // 1. 找訂單
            var order = await _context.Orders.FindAsync(dto.OrderId);

            if (order == null) return NotFound("找不到訂單");
            if (order.TotalAmount <= 0) return BadRequest("金額錯誤");

            // 2. 呼叫 Service 產生給綠界的參數
            // 注意：商品名稱這裡先寫死或簡化，真實專案可以用 StringBuilder 把商品名稱串起來
            var paymentRequest = _ecPayService.GetPaymentRequest(
                order.Id.ToString(),
                (int)order.TotalAmount,
                "ShopDemo購物"
            );

            // 3. 產生自動送出的 HTML Form (關鍵魔法！)
            var html = GenerateHtmlForm(paymentRequest);

            // 4. 回傳 HTML (Content-Type: text/html)
            return Content(html, "text/html");
        }

        // POST: api/Payment/Callback
        // 綠界刷卡成功後，會偷偷呼叫這支 API (Server 對 Server)
        // ⚠️ 注意：這支 API 本機測不到，需要用 ngrok
        [HttpPost("Callback")]
        public async Task<IActionResult> Callback([FromForm] IFormCollection form)
        {
            // 1. 接收綠界傳來的關鍵參數
            var rtnCode = form["RtnCode"];           // 1 代表成功
            var merchantTradeNo = form["MerchantTradeNo"]; // 我們的訂單編號 (例如: 213857)
            var tradeAmt = form["TradeAmt"];         // 交易金額
            var paymentDate = form["PaymentDate"];   // 付款時間

            if (rtnCode == "1")
            {
                // 1. 解析 OrderId
                // 如果你之前是用 "純數字" 當 MerchantTradeNo，直接轉 int
                if (int.TryParse(merchantTradeNo, out int orderId))
                {
                    // 2. 查資料庫
                    var order = await _context.Orders.FindAsync(orderId);

                    if (order != null)
                    {
                        // 3. 修改狀態 (假設你有 Status 欄位)
                        // 如果你的 Entity 沒有 Status 欄位，現在是個好時機加上去 (public string Status { get; set; })
                        order.Status = "Paid";

                        // 如果沒有 Status 欄位，暫時用 Console 代表
                        // Console.WriteLine($"訂單 {orderId} 已付款，寫入 DB!"); 

                        await _context.SaveChangesAsync();
                        Console.WriteLine($"✅ 資料庫更新成功：訂單 #{orderId} -> Paid");
                    }
                }
                return Content("1|OK", "text/plain");
            }
            Console.WriteLine("❌ 付款失敗或 RtnCode != 1");
            return BadRequest("付款失敗");
        }

        // 🛠️ 小工具：把參數轉成 HTML Form
        private string GenerateHtmlForm(PaymentRequestDto request)
        {
            var sb = new StringBuilder();
            sb.Append("<html><body>");
            // 這裡的 Action 就是綠界的測試網址
            sb.Append("<form id='ecpay-form' action='https://payment-stage.ecpay.com.tw/Cashier/AioCheckOut/V5' method='POST'>");

            // 把所有參數變成 hidden input
            sb.Append($"<input type='hidden' name='MerchantID' value='{request.MerchantID}' />");
            sb.Append($"<input type='hidden' name='MerchantTradeNo' value='{request.MerchantTradeNo}' />");
            sb.Append($"<input type='hidden' name='MerchantTradeDate' value='{request.MerchantTradeDate}' />");
            sb.Append($"<input type='hidden' name='PaymentType' value='{request.PaymentType}' />");
            sb.Append($"<input type='hidden' name='TotalAmount' value='{request.TotalAmount}' />");
            sb.Append($"<input type='hidden' name='TradeDesc' value='{request.TradeDesc}' />");
            sb.Append($"<input type='hidden' name='ItemName' value='{request.ItemName}' />");
            sb.Append($"<input type='hidden' name='ReturnURL' value='{request.ReturnURL}' />");
            sb.Append($"<input type='hidden' name='ChoosePayment' value='{request.ChoosePayment}' />");
            sb.Append($"<input type='hidden' name='EncryptType' value='{request.EncryptType}' />");
            sb.Append($"<input type='hidden' name='ClientBackURL' value='{request.ClientBackURL}' />");
            sb.Append($"<input type='hidden' name='CheckMacValue' value='{request.CheckMacValue}' />"); // 檢查碼

            sb.Append("</form>");

            // 自動送出表單的 JavaScript
            sb.Append("<script>document.getElementById('ecpay-form').submit();</script>");
            sb.Append("</body></html>");

            return sb.ToString();
        }
    }

    // 簡單的 DTO，用來接前端傳來的 OrderId
    public class CheckoutDto
    {
        public int OrderId { get; set; }
    }
}