## ADDED Requirements

### Requirement: 使用者註冊 (User Registration)
系統必須允許新使用者透過提供基本資料建立帳戶。

#### Scenario: 成功註冊帳戶
- **WHEN** 使用者提交包含有效 Email 與密碼的註冊表單
- **THEN** 系統建立新的 User 紀錄並回傳成功訊息

### Requirement: 使用者登入 (User Login)
系統必須使用 JWT (JSON Web Token) 進行身份驗證，並根據角色 (Role) 賦予權限。

#### Scenario: 成功登入
- **WHEN** 使用者提供正確的帳號密碼
- **THEN** 系統配發包含 User Role (Admin/Staff/User) 的 JWT Token

### Requirement: 角色存取控制 (Role-based Access Control)
系統必須限制特定端點 (如 Admin Controllers) 只能由擁有 Admin 或 Staff 權限的使用者存取。

#### Scenario: 拒絕無權限存取
- **WHEN** 一般 User 嘗試存取 `AdminProductController` 端點
- **THEN** 系統回傳 HTTP 403 Forbidden
