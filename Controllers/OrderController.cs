using System.Security.Claims;
using Dapper; // 👈 Dapper 登場
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient; // 👈 為了建立 SQL 連線
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Dtos;
using ShopApi.Models;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // 只有會員能結帳和看訂單
    public class OrderController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;

        // 1. 結帳 (Checkout) - 使用 EF Core (處理複雜寫入)
        [HttpPost("checkout")]
        public async Task<ActionResult> Checkout()
        {
            var userId = GetUserId();
            var (IsSuccess, Message, OrderId) = await _orderService.CheckoutAsync(userId);
            if (!IsSuccess)
            {
                return BadRequest(Message); // ✅ 服務生負責把失敗轉成 400
            }
            return Ok(new {Message, OrderId }); // ✅ 服務生負責把成功轉成 200
        }

        // 2. 查詢我的歷史訂單 - 使用 Dapper (追求查詢效能)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
        {
            var userId = GetUserId();
            var orders = await _orderService.GetMyOrdersAsync(userId);
            return Ok(orders);
        }

        // 3. 查詢單筆訂單詳情 (包含明細) - 使用 Dapper (進階練習)
        [HttpGet("{orderId}")]
        public async Task<ActionResult> GetOrderDetail(int orderId)
        {
            var userId = GetUserId();
            var order = await _orderService.GetOrderByIdAsync(orderId,userId);
            if ( order == null) return NotFound("找不到訂單");

            return Ok(order);
        }

        // 小工具：取得 User ID
        private int GetUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier) ?? throw new Exception("請重新登入");
            return int.Parse(idClaim.Value);
        }
    }
}