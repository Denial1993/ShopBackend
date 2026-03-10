## ADDED Requirements

### Requirement: 商品展示 (Product Display)
系統必須能列出所有可供購買的商品與服務，並支援分類檢視 (例如：筆電、耳機、貓咪)。

#### Scenario: 載入首頁商品列表
- **WHEN** 使用者進入平台首頁或商品列表頁
- **THEN** 系統回傳所有上架 (Active) 的商品資料與對應的分類資訊

### Requirement: 管理員商品上架與編輯 (Admin Product Management)
具備 Admin 或 Staff 權限的使用者，可以透過後台新增、修改或刪除商品資訊 (包含上傳圖片與設定價格)。

#### Scenario: 成功新增商品
- **WHEN** 管理員送出新的商品資訊與圖片
- **THEN** 系統將商品寫入資料庫，並展示於前台

### Requirement: 商品分類管理 (Category Management)
系統需能將商品歸類於特定的類別，以便前端導覽列或過濾器使用。

#### Scenario: 取得分類清單
- **WHEN** 前端請求分類 API
- **THEN** 系統回傳所有現存的分類名稱 (Categories)
