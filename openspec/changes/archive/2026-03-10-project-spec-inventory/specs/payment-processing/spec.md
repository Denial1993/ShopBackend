## ADDED Requirements

### Requirement: 第三方支付金流串接 (Payment Gateway Integration)
系統在訂單建立後，必須能產生供第三方支付服務 (如綠界、LinePay 等) 認證與扣款的交易 Payload。

#### Scenario: 產生支付連結
- **WHEN** 訂單成功建立並請求支付
- **THEN** 系統根據訂單總金額與編號，回傳前端導向第三方金流的 URL 或支付 Token

### Requirement: 支付狀態回調 (Payment Webhook/Callback)
系統必須提供一個 Webhook 接收外部支付服務的非同步付款結果通知。

#### Scenario: 成功接收付款成功通知
- **WHEN** 第三方支付平台呼叫 `PaymentController` 的回調端點，並帶有合法的簽章與成功狀態
- **THEN** 系統將對應訂單的狀態更新為「已付款 (PAID)」並處理後續出貨準備邏輯
