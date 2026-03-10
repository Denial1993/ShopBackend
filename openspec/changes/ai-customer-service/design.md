## Context

PawPals 平台需要一個 AI 客服小幫手，能在不增加維護成本且「極度控管預算」的前提下，為顧客解答基本問題。
目前主流的 AI 方案往往包含外掛搜尋引擎 (Web Search) 或向量資料庫 (Vector DB)，這不僅帶來額外的基礎設施成本，也容易因為外部雜亂資訊導致 AI 產生幻覺 (Hallucination) 或回答偏離主題。因此，我們需要一套極致省錢且受控的架構。

## Goals / Non-Goals

**Goals:**
- 建立一個僅依賴系統內部資料對話的 AI 客服系統。
- 透過 C# Web API 與 Semantic Kernel (或直接呼叫 LLM) 將自然語言轉化為系統指令。
- 實作 Function Calling (工具呼叫) 機制，讓 AI 能夠主動去 SQL 資料庫「查貨」。
- 確保所有固定的答覆原則 (如退換貨、運送) 直接寫入 System Prompt 以利用快取降低成本。

**Non-Goals:**
- 不包含任何向量資料庫 (Vector DB, e.g., Pinecone, pgvector) 的建置。
- 不串接任何外部網路搜尋 API (如 Google Search API 或 Bing Search API)。
- 不使用昂貴的大型模型，僅限成本效益最高的模型 (如 Gemini 1.5 Flash 或 GPT-4o-mini)。

## Decisions

- **捨棄 RAG Vector DB，改用 Tool Calling**：商品型錄等變動資料不用 Embedding 事先算好，而是讓 AI 分析語意後，呼叫對應的後端函式 (`SearchShopProducts`)。由後端進行 SQL LIKE 查詢後，只返回極少量的關鍵字與價格供 AI 組裝回答，大量減少 Token 消耗。
- **System Prompt 靜態化**：將公司簡介、運費與退貨規則直接寫死在 System Roles 中。這是因為現代 LLM API (如 Gemini) 對於固定不變的前文會給予極高的 Token 折扣 (Context Caching)。
- **前端 Vue 輕量化元件**：AI 聊天窗實作為一個單獨的 `<AiChatWidget />` 元件，以 Floating Action Button 形式附著在畫面右下角，與原有的 LINE 按鈕並存。

## Risks / Trade-offs

- [Risk: 使用者詢問複雜的語意搜尋，例如「給我保暖的衣服」] → Mitigation: 由於我們沒有向量庫，標準的 SQL LIKE 查不到「保暖」。此時需依賴 AI 將「保暖」關聯為具體的「毛衣」、「厚外套」關鍵字後，再發動工具呼叫給後端。這需要在 Tool Description 妥善引導 AI 的查詞策略。
- [Risk: 惡意使用者意圖讓 AI 講出破壞品牌的話 (Prompt Injection)] → Mitigation: 後端在傳送給 AI 的 System Prompt 中，需加上嚴格的防禦指令 ("你只能回答關於 PawPals 的問題，其餘一律委婉拒絕")。
