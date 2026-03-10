## ADDED Requirements

### Requirement: 整合大型語言模型 (Integrate LLM)
後端必須具備透過 Semantic Kernel 或原生 SDK 存取成本效益模型 (如 Gemini 1.5 Flash 或 GPT-4o-mini) 的能力。

#### Scenario: 成功初始化 AI 核心服務
- **WHEN** 系統啟動且合法的 API Key 已設定
- **THEN** 系統將 AI 服務註冊至 Dependency Injection，供 `AiChatController` 呼叫

### Requirement: 系統人設防護與固定知識注入 (System Prompt Injection)
後端在每次向 LLM 發送對話前，必須強制安插預設的 System Prompt，內容包含公司簡介、運送與退貨規則，以及嚴格的回覆範圍限制。

#### Scenario: 阻攔非業務相關問題
- **WHEN** 使用者詢問「現在長榮機票多少錢？」
- **THEN** LLM 基於 System Prompt 防護，不執行工具呼叫，直接回覆「我是 PawPals 客服，只能回答寵物或商品相關問題喔！」
