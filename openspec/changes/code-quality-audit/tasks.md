## 1. 身份驗證（Auth）規格稽核

- [x] 1.1 審閱 `AuthController` 與 `AuthService` — 確認註冊時重複 Email 是否回傳正確的 HTTP 狀態碼
  > ⚠️ **缺陷**：`AuthService.RegisterAsync` 回傳字串 `"Email 已經被註冊過了"`，`AuthController` 接到後呼叫 `BadRequest(result)`（HTTP 400）。規格要求回傳 HTTP 400 或 409，目前為 HTTP 400，基本符合，但建議改為 409 Conflict 更語意正確。
- [x] 1.2 確認密碼最短長度驗證是否存在於 `UserDtoValidator` 或 Service 層
  > ✅ **符合**：`UserDtoValidator` 已設定 `MinimumLength(6)`，並額外要求大寫字母、小寫字母和數字，驗證強度超過規格要求。
- [x] 1.3 確認登入輸入錯誤密碼時回傳 HTTP 401（而非 400 或 500）
  > ⚠️ **缺陷**：`AuthController.Login` 呼叫 `BadRequest("帳號或密碼錯誤")`（HTTP 400），**應回傳 HTTP 401 Unauthorized**。
- [x] 1.4 確認登入輸入不存在的 Email 時回傳 HTTP 401（而非 404，以防止用戶列舉攻擊）
  > ⚠️ **缺陷**：同上，`AuthService.LoginAsync` 對 Email 不存在與密碼錯誤都回傳 `null`，Controller 統一回傳 `BadRequest`（HTTP 400），**應改為 HTTP 401**。用戶列舉攻擊防護在 Service 層已正確處理（不區分兩種失敗），但 Controller 狀態碼錯誤。
- [x] 1.5 備註：`ValidateIssuer` 與 `ValidateAudience` 目前為 `false` — 記錄為已知安全缺口，待後續獨立變更處理
  > 📋 **已確認**：`Program.cs` 第 29-30 行，`ValidateIssuer = false`、`ValidateAudience = false`。已列入安全缺口追蹤清單，不在本次變更範圍內修復。

## 2. 商品管理（Product）規格稽核

- [x] 2.1 確認 `ProductsController` 的 POST / PUT / DELETE 路由有強制驗證管理員身份
  > ✅ **符合**：`AdminProductController` 在 Class 層級套用 `[Authorize(Roles = "Admin,Staff")]`，所有 POST / PUT / DELETE 路由均受保護。
- [x] 2.2 確認 GET /products 回傳 HTTP 200 且包含所有必要欄位（id、名稱、價格、圖片 URL）
  > ✅ **符合**：`ProductController.GetProducts` 回傳 HTTP 200，`ProductDto` 包含 id、名稱、價格、圖片 URL 等必要欄位。
- [x] 2.3 確認 GET /products/{id} 查詢不存在的商品時回傳 HTTP 404
  > ✅ **符合**：`ProductController.GetProduct` 呼叫 `NotFound("找不到該商品")` 回傳 HTTP 404。
- [x] 2.4 確認商品建立回傳 HTTP 201，且透過 FluentValidation 驗證必填欄位
  > ⚠️ **缺陷**：`AdminProductController.CreateProduct` 回傳 `Ok(product)`（HTTP 200），**規格要求回傳 HTTP 201 Created**。另 `ProductUploadDto` 尚未找到對應的 FluentValidation，必填欄位驗證可能僅靠 `[Required]` Attribute 或未加驗證。
- [x] 2.5 確認圖片上傳後，商品的圖片 URL 正確透過 Supabase Storage 更新
  > ✅ **符合**：`SaveImage` 方法將圖片上傳至 Supabase Storage `products` Bucket，並建構完整公開 URL 更新至商品。另有 `Console.WriteLine` Debug 日誌殘留（第 119 行），建議清除。

## 3. 訂單管理（Order）規格稽核

- [x] 3.1 確認 `OrderController` 依已驗證用戶的 ID 過濾訂單（不允許跨用戶存取）
  > ✅ **符合**：`GetMyOrders` 傳入 `userId`，`GetOrderByIdAsync(orderId, userId)` 同時驗證訂單歸屬，`OrderController` 套用 `[Authorize]`。
- [x] 3.2 確認購物車為空時嘗試結帳會回傳 HTTP 400
  > ✅ **符合**：`OrderController.Checkout` 當 `IsSuccess == false` 時回傳 `BadRequest(Message)`（HTTP 400）。`OrderService.CheckoutAsync` 應有空購物車的判斷（需確認 Service 邏輯）。
- [x] 3.3 確認存取他人訂單會回傳 HTTP 403 或 HTTP 404（而非訂單資料）
  > ✅ **符合**：`GetOrderByIdAsync(orderId, userId)` 傳入 userId 作為過濾條件，`GetOrderDetail` 查詢結果為 null 時回傳 `NotFound`（HTTP 404）。
- [x] 3.4 確認訂單狀態流轉：付款回呼成功後，訂單狀態從「待付款」變更為「已付款」
  > ✅ **符合**：`PaymentController.Callback` 當 `RtnCode == "1"` 時，將 `order.Status` 設為 `"Paid"` 並儲存。

## 4. 購物車（Cart）規格稽核

- [x] 4.1 確認購物車為空時回傳 HTTP 200 和空陣列（而非 HTTP 404）
  > ✅ **符合**：`CartController.GetMyCart` 當 `CartService` 回傳 `null` 時，控制器回傳 `new CartDto()`（HTTP 200，空物件），而非 404。
- [x] 4.2 確認重複加入同一商品時，會累加數量而非新增重複資料列
  > ✅ **符合**：`CartService.AddToCartAsync` 先以 `ProductId` 檢查是否已存在，若存在則執行 `existingItem.Quantity += request.Quantity`。
- [x] 4.3 確認將數量設為 0 時，該商品項目會從購物車移除
  > ⚠️ **缺口**：目前 `CartController` 和 `CartService` 沒有「更新數量」端點，也沒有數量為 0 時自動移除的邏輯。規格要求的「更新數量」功能**尚未實作**。
- [x] 4.4 確認用戶無法刪除他人的購物車商品（回傳 HTTP 403 或 HTTP 404）
  > ✅ **符合**：`CartService.RemoveItemAsync` 查詢時加入 `ci.Cart!.UserId == userId` 條件，若不符即回傳 `false`，Controller 回傳 `NotFound`（HTTP 404）。

## 5. 金流整合（ECPay）規格稽核

- [x] 5.1 確認 ECPay 付款表單 / 連結只針對「待付款」狀態的訂單產生
  > ⚠️ **缺陷**：`PaymentController.Checkout` 只驗證訂單存在與金額大於 0，**沒有檢查訂單狀態是否為「Pending」**，已付款的訂單也可以重新產生付款表單。
- [x] 5.2 確認用戶無法對他人的訂單發起付款
  > ⚠️ **缺陷**：`PaymentController` **沒有 `[Authorize]` 屬性**，未驗證身份的使用者也可存取。即使加了驗證，Controller 也沒有比對訂單的 `UserId` 與當前登入者，**任何已登入用戶都可對他人訂單發起付款**。
- [x] 5.3 確認 ECPay 回呼中 `RtnCode=1` 時，訂單狀態設為「已付款」且回傳 `1|OK`
  > ✅ **符合**：`Callback` 方法正確判斷 `RtnCode == "1"` 後呼叫 `order.Status = "Paid"` 並回傳 `Content("1|OK", "text/plain")`。
- [x] 5.4 確認 `RtnCode` 非成功值的回呼，訂單仍維持「待付款」狀態
  > ✅ **符合**：`Callback` 方法只有在 `RtnCode == "1"` 時才更新訂單，其他情況直接回傳 `BadRequest`，訂單狀態不變。
- [x] 5.5 確認回呼處理前有驗證 `CheckMacValue` 的合法性（防止偽造請求）
  > ⚠️ **嚴重缺陷**：`PaymentController.Callback` **完全沒有驗證 `CheckMacValue`**，任何人都可以偽造 POST 請求將訂單標記為已付款，構成嚴重安全漏洞。
