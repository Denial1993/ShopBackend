# ShopApi (電商後端練習專案)

這是一個基於 .NET 8 Web API 開發的簡易電商後端系統。

## 🛠 技術堆疊
* **Framework**: .NET 8
* **Database**: SQL Server
* **ORM**: 
  * EF Core (負責寫入/交易處理)
  * Dapper (負責高效查詢)
* **Architecture**: Layered Architecture (Controller -> Service -> Repository/DB)
* **Docs**: Swagger / OpenAPI

## ✨ 功能特色
* JWT 身分驗證 (登入/註冊)
* 商品管理 (搜尋、分頁)
* 購物車系統 (加入、移除、清空)
* 訂單系統 (交易一致性保證)
* 完整的 DTO 與 ViewModel 分離設計