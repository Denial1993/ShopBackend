## ADDED Requirements

### Requirement: AI 客服懸浮按鈕介面 (Chat Widget UI)
前端必須提供一個獨立且美觀的聊天室元件，該元件固定於畫面右下角 (LINE 按鈕旁)。

#### Scenario: 展開與收合聊天視窗
- **WHEN** 使用者點擊右下角的 AI 客服圖示
- **THEN** 聊天視窗自底部彈出；再次點擊則收合隱藏

### Requirement: 使用者發送對話與呈現 (Message Rendering)
聊天室內必須能清楚區分「使用者」與「AI」的對話氣泡，且在 AI 回覆期間顯示載入中動畫。

#### Scenario: 發送問題並等待回應
- **WHEN** 使用者輸入文字並送出
- **THEN** 畫面顯示使用者的對話氣泡，接著出現 "Typing..." 或點點點的動畫，直到後端返回完整答覆
