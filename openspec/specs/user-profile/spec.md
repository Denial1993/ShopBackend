## ADDED Requirements

### Requirement: 檢視個人帳號資訊 (View Profile)
系統必須允許已登入的使用者檢視自己的個人基本資料 (Profile)。

#### Scenario: 成功取得個人資訊
- **WHEN** 已登入使用者請求 `/api/Profile` 端點
- **THEN** 系統透過 JWT Token 解析出使用者 ID，並回傳該使用者的姓名、Email、聯絡電話、地址等資訊

### Requirement: 更新個人帳號資訊 (Update Profile)
系統必須允許使用者修改非關鍵性的個人聯絡資訊。

#### Scenario: 成功更新地址與電話
- **WHEN** 使用者送出更新的聯絡電話與收件地址
- **THEN** 系統將資訊更新至資料庫，並回傳更新成功的狀態
