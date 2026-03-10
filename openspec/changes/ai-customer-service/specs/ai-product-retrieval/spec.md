## ADDED Requirements

### Requirement: 定義商品查詢工具 (Define Function Calling Interface)
系統必須將一個名為 `SearchShopProducts` 的原生 C# 函式定義給 LLM 知道，並清楚說明該工具接收 "keyword" 參數。

#### Scenario: AI 決定呼叫查詢工具
- **WHEN** 使用者詢問「你們有賣貓耳朵耳機嗎？」
- **THEN** LLM 分析語意後中斷文字生成，向後端發出要求執行 `SearchShopProducts`(keyword: "貓耳朵耳機") 的指令

### Requirement: 執行關聯式資料庫輕量查詢 (Execute SQL Lookup)
當收到 LLM 的工具呼叫請求時，系統不經過向量庫，直接向現有 SQL 資料庫進行輕量化的模糊查詢。

#### Scenario: 回傳 JSON 結果給 AI
- **WHEN** 後端執行完 `SearchShopProducts` 函式
- **THEN** 後端將找到的商品名稱、價格與購買連結轉為極短的 JSON 字串，再次送交給 LLM 以組裝最終的人類可讀回覆
