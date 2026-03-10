## 1. 後端基礎設施準備 (C# API & AI Core)

- [x] 1.1 在 ShopApi 專案中安裝 `Microsoft.SemanticKernel` 套件
- [x] 1.2 設定 `appsettings.json` 加入 LLM 模型的 API Key (OpenAI / Gemini)
- [x] 1.3 建立 `AiChatController.cs` 以接收前端對話 POST 請求

## 2. 工具呼叫實作 (Function Calling & Retrieval)

- [x] 2.1 在 C# 實作 `SearchShopProductsPlugin` 並提供 `SearchShopProducts(string keyword)` 函式
- [x] 2.2 確保 `SearchShopProducts` 僅使用簡單的 Entity Framework (LIKE 查詢) 並回傳最小化 JSON
- [x] 2.3 撰寫固定 System Prompt (包含公司規則) 並匯入至 Semantic Kernel 的 ChatHistory

## 3. 前端介面開發 (Vue 3 UI)

- [x] 3.1 在 `shop-frontend` 建立新元件 `AiChatWidget.vue`
- [x] 3.2 設計懸浮按鈕 UI 與展開後的對話氣泡介面 (套用現有 Claymorphism 風格)
- [x] 3.3 實作前端呼叫 `POST /api/Chat` 的邏輯與載入中 (Typing) 動畫狀態
- [x] 3.4 將 `<AiChatWidget />` 匯入並放置於 `App.vue` 底部
