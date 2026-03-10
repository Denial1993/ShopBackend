using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

using ShopApi.Plugins;
using ShopApi.Data;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/Chat")]
public class AiChatController : ControllerBase
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatService;
    private readonly ShopDbContext _db;

    // 定義 System Prompt (防護與系統人設)
    private const string SystemPrompt = @"
你是 PawPals 寵物平台的專屬 AI 客服小幫手。
你的任務是提供友善、溫暖的客戶服務，並盡全力解答客人的疑問。

【公司簡介】
PawPals 致力於提供高品質的寵物食品、玩具與美容服務。

【退貨政策】
1. 一般商品享有 7 天鑑賞期。
2. 電子產品若已拆封則不接受退貨，除非有明顯瑕疵。

【運送規則】
1. 一般包裹運費為 60 元，滿千免運。
2. 活體貓咪/寵物需專車運送，運費另計，並需提前電話確認。

【安全規範】
- 如果客人詢問與 PawPals 業務無關的問題 (例如政治、機票、無關的新聞等)，請委婉並賣萌地拒絕回答。
- 你可以試著推銷店內的商品。

請使用繁體中文回覆，語氣可以可愛、親切一點。";

    public AiChatController(Kernel kernel, ShopDbContext db)
    {
        _kernel = kernel;
        _chatService = _kernel.GetRequiredService<IChatCompletionService>();
        _db = db;

        // 將撈商品的工具註冊到 Kernel 中
        _kernel.Plugins.AddFromObject(new SearchShopProductsPlugin(_db));
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        // 如果要維護多輪對話，前端可以傳遞整個 History 陣列，這裡簡化為單次觸發
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("Message cannot be empty.");

        // 初始化對話歷史紀錄，塞入 System Prompt
        var chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage(SystemPrompt);
        chatHistory.AddUserMessage(request.Message);

        // 設定觸發工具呼叫的設定 (如果有的話)
        var executionSettings = new PromptExecutionSettings
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        try
        {
            // 呼叫 LLM
            var response = await _chatService.GetChatMessageContentAsync(
                chatHistory,
                executionSettings,
                _kernel);

            return Ok(new { Reply = response.Content });
        }
        catch (Exception ex)
        {
            // 防止 API Key 沒設導致的 Crash，將友善訊息回傳前端
            return StatusCode(500, new { Reply = "不好意思，AI 腦袋當機了 (後端可能尚未設定正確的 API Key)，請稍後再試！", Error = ex.Message });
        }
    }
}
