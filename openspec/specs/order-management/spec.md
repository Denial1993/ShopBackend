## ADDED Requirements

### Requirement: 建立訂單 (Create Order)
使用者確定購買後，系統必須將購物車內容轉換為一筆正式訂單 (Order)，並凍結商品當下價格。

#### Scenario: 成功從購物車建立訂單
- **WHEN** 使用者提交收件資訊並確認結帳
- **THEN** 系統建立一筆狀態為「待付款 (Pending)」的訂單，並記錄購買的明細 (OrderItems) 與總金額

### Requirement: 檢視個人歷史訂單 (View My Orders)
使用者必須能查看自己過去建立的所有訂單清單及詳細狀態。

#### Scenario: 成功列出個人訂單
- **WHEN** 使用者進入「我的訂單」頁面
- **THEN** 系統列出該帳號下的所有相關訂單及包含支付狀態與物流狀態

### Requirement: 後台訂單管理 (Admin Order Management)
系統管理員 (Admin/Staff) 必須能夠檢視全站訂單，並有權限變更訂單狀態 (如：已出貨、已取消)。

#### Scenario: 管理員變更訂單出貨狀態
- **WHEN** 管理員將特定訂單狀態變更為「Shipped」
- **THEN** 系統更新資料庫狀態，並保留更新日誌
