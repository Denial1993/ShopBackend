## Why

為了提升 PawPals 的平台價值並減少客服人力成本，我們計畫在前端右下角加入一個溫暖親切的 AI 智慧客服元件。
該功能首要考量為**「極致省錢」**，因此捨棄高成本的 Web Search 與獨立的向量資料庫 (Vector DB)，全面採用基於大語言模型 (LLM) Function Calling 配合既有 SQL 資料庫的輕量化架構。

## What Changes

本變更主要涉及下列模組：
- [NEW] 新增前台 UI：在 `App.vue` 右下角加入一個聊天懸浮按鈕 (`AiChatWidget.vue`)。
- [NEW] 新增後端控制器：`AiChatController` 負責接收前端對話記錄。
- [NEW] 串接 AI 服務：在 C# 後端整合 Semantic Kernel 框架與 Gemini 1.5 Flash (或 GPT-4o-mini) 以獲得高性價比。
- [NEW] 工具呼叫 (Function Calling) 實作：實作一個供 AI 呼叫的內部函式 (如 `SearchShopProducts`)，讓 AI 能根據關鍵字直接向現有關聯式資料庫 (SQL) 索取商品資訊並回覆給顧客。
- [MODIFIED] 無現有業務模組被破壞，此為外掛型功能。

## Capabilities

### New Capabilities
- `ai-chat-widget`: 前端與使用者互動的聊天室介面。
- `ai-semantic-kernel`: 後端整合 LLM 與工具呼叫的核心邏輯處理。
- `ai-product-retrieval`: 提供給 AI 專用的內部輕量商品檢索 API。

### Modified Capabilities
<!-- 本次無既有需求變更，全為新增擴充功能 -->

## Impact

- 提升網站轉換率與使用者互動體驗。
- 初期建置只需要一個簡單的 API Key 即可啟動，幾乎沒有基礎設施 (Infrastructure) 擴增成本。
- 會增加 C# 後端的專案依賴 (如 `Microsoft.SemanticKernel`)。
