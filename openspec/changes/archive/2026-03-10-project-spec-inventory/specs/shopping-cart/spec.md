## ADDED Requirements

### Requirement: 加入購物車 (Add to Cart)
登入的使用者可以將商品加入個人購物車，並指定數量。

#### Scenario: 成功將商品加入空購物車
- **WHEN** 使用者選擇商品與數量，並點擊加入購物車
- **THEN** 系統建立 Cart 項目，與該使用者的帳戶綁定，並將商品保留於購物清單中

### Requirement: 調整購物車內容 (Modify Cart)
使用者可以修改購物車內既有商品的數量或將其移除。

#### Scenario: 成功更新商品數量
- **WHEN** 使用者在購物車頁面增加特定商品的購買數量
- **THEN** 系統更新 CartItem 的數量與總價計算

### Requirement: 清空購物車 (Clear Cart)
使用者選取特定操作，或在結帳完成後，系統可以清空對應的購物車內容。

#### Scenario: 結帳後自動清空
- **WHEN** 訂單成功建立且支付初始化完成後
- **THEN** 系統自動移除該使用者在 Cart 中的所有已結帳項目
