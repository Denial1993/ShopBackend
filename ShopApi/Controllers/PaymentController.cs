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

        /// <summary>
        /// 前端按下「前往付款」時呼叫這支
        /// </summary>
        /// <param name="dto">訂單 ID</param>
        /// <returns></returns>
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
        /// <summary>
        /// 綠界刷卡成功後，會偷偷呼叫這支 API (Server 對 Server)
        /// ⚠️ 注意：這支 API 本機測不到，需要用 ngrok
        /// </summary>
        /// <param name="form">綠界傳來的表單</param>
        /// <returns></returns>
        [HttpPost("Callback")]
        public async Task<IActionResult> Callback([FromForm] IFormCollection form)
        {
            // 1. 接收綠界傳來的關鍵參數
            var rtnCode = form["RtnCode"];           // 1 代表成功
                                                     // var merchantTradeNo = form["MerchantTradeNo"]; // 我們的訂單編號 (例如: 213857)
                                                     // ✅ 修改：從 CustomField1 拿真正的 ID
            var targetOrderId = form["CustomField1"];
            var tradeAmt = form["TradeAmt"];         // 交易金額
            var paymentDate = form["PaymentDate"];   // 付款時間

            if (rtnCode == "1" && int.TryParse(targetOrderId, out int orderId))
            {
                // 現在 orderId 真的會是 6 了！
                var order = await _context.Orders.FindAsync(orderId);

                if (order != null)
                {
                    order.Status = "Paid";
                    await _context.SaveChangesAsync(); // EF Core 這裡會自動處理雙引號，不用擔心
                    return Content("1|OK", "text/plain");
                }
            }
            return BadRequest("查無訂單");
        }

        /// <summary>
        /// 小工具：把參數轉成 HTML Form
        /// </summary>
        /// <param name="request">綠界參數</param>
        /// <returns></returns>
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
            sb.Append($"<input type='hidden' name='CustomField1' value='{request.CustomField1}' />");
            sb.Append($"<input type='hidden' name='CheckMacValue' value='{request.CheckMacValue}' />"); // 檢查碼

            sb.Append("</form>");

            // 自動送出表單的 JavaScript
            sb.Append("<script>document.getElementById('ecpay-form').submit();</script>");
            sb.Append("</body></html>");

            return sb.ToString();
        }
    }

    /// <summary>
    /// 簡單的 DTO，用來接前端傳來的 OrderId
    /// </summary>
    public class CheckoutDto
    {
        public int OrderId { get; set; }
    }
}