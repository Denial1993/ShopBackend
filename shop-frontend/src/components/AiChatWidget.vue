<script setup>
import { ref, nextTick } from 'vue';

const isOpen = ref(false);
const userInput = ref('');
const messages = ref([
  { role: 'assistant', content: '你好！我是 PawPals AI 客服小幫手 🐾，請問有什麼我可以為您服務的呢？' }
]);
const isLoading = ref(false);
const chatBodyRef = ref(null);

const toggleChat = () => {
  isOpen.value = !isOpen.value;
  if(isOpen.value) {
    setTimeout(scrollToBottom, 100);
  }
};

const getApiUrl = () => {
  // 對應本地或 VITE_API_URL (例如 Render 上的後端)
  const baseUrl = import.meta.env.VITE_API_URL || 'http://localhost:5033';
  return `${baseUrl}/api/Chat`;
};

const sendMessage = async () => {
  if (!userInput.value.trim() || isLoading.value) return;

  const msg = userInput.value.trim();
  messages.value.push({ role: 'user', content: msg });
  userInput.value = '';
  isLoading.value = true;
  await nextTick();
  scrollToBottom();

  try {
    const response = await fetch(getApiUrl(), {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ message: msg })
    });
    
    if(!response.ok) {
        throw new Error('Network response was not ok');
    }
    
    const data = await response.json();
    messages.value.push({ role: 'assistant', content: data.reply });
  } catch (err) {
    messages.value.push({ role: 'assistant', content: '非常抱歉，系統現在有點忙碌，請稍後再試喔 🐾' });
  } finally {
    isLoading.value = false;
    await nextTick();
    scrollToBottom();
  }
};

const scrollToBottom = () => {
  if (chatBodyRef.value) {
    chatBodyRef.value.scrollTop = chatBodyRef.value.scrollHeight;
  }
};
</script>

<template>
  <div class="ai-chat-container">
    <!-- 懸浮按鈕 -->
    <button class="ai-fab" @click="toggleChat" :class="{ 'is-open': isOpen }">
      <span v-if="!isOpen">🤖</span>
      <span v-else>✖</span>
    </button>

    <!-- 對話視窗 -->
    <Transition name="bounce">
      <div v-if="isOpen" class="ai-chat-window">
        <div class="ai-chat-header">
          🐾 PawPals 智能客服
        </div>
        
        <div class="ai-chat-body" ref="chatBodyRef">
          <div 
            v-for="(msg, index) in messages" 
            :key="index"
            class="chat-bubble-wrapper"
            :class="msg.role === 'user' ? 'user' : 'assistant'"
          >
            <div class="chat-bubble">
              {{ msg.content }}
            </div>
          </div>
          
          <div v-if="isLoading" class="chat-bubble-wrapper assistant">
            <div class="chat-bubble typing">
              <span></span><span></span><span></span>
            </div>
          </div>
        </div>

        <div class="ai-chat-footer">
          <input 
            v-model="userInput" 
            @keyup.enter="sendMessage" 
            type="text" 
            placeholder="請輸入您的問題..."
            :disabled="isLoading"
          />
          <button @click="sendMessage" :disabled="isLoading || !userInput.trim()">傳送</button>
        </div>
      </div>
    </Transition>
  </div>
</template>

<style scoped>
/* 容器固定在右下角，與原本的 LineContactButton 錯開 */
.ai-chat-container {
  position: fixed;
  bottom: 120px; /* 拉高一點避免跟 LINE 按鈕疊加 */
  right: 30px;
  z-index: 1050;
  display: flex;
  flex-direction: column;
  align-items: flex-end;
}

/* 懸浮按鈕 Claymorphism 風格 */
.ai-fab {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  background: linear-gradient(135deg, #ffb6b9 0%, #fae3d9 100%);
  border: none;
  box-shadow: 
    4px 4px 10px rgba(255, 182, 185, 0.4),
    -4px -4px 10px rgba(255, 255, 255, 0.8),
    inset 2px 2px 5px rgba(255, 255, 255, 0.5),
    inset -2px -2px 5px rgba(255, 182, 185, 0.5);
  font-size: 24px;
  color: #fff;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.68, -0.55, 0.265, 1.55);
  display: flex;
  align-items: center;
  justify-content: center;
}

.ai-fab:hover:not(.is-open) {
  transform: scale(1.1) translateY(-5px);
}

.ai-fab.is-open {
  background: #f8f9fa;
  color: #333;
  box-shadow: 2px 2px 5px rgba(0,0,0,0.1);
}

/* 聊天視窗 */
.ai-chat-window {
  position: absolute;
  bottom: 80px;
  right: 0;
  width: 340px;
  height: 480px;
  background: rgba(255, 255, 255, 0.95);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  border: 2px solid #ffefe0;
  box-shadow: 0 10px 30px rgba(255, 182, 185, 0.2);
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

/* 動畫設定 */
.bounce-enter-active {
  animation: bounce-in 0.4s;
}
.bounce-leave-active {
  animation: bounce-in 0.3s reverse;
}
@keyframes bounce-in {
  0% { transform: scale(0.8) translateY(20px); opacity: 0; }
  50% { transform: scale(1.02); }
  100% { transform: scale(1) translateY(0); opacity: 1; }
}

/* Header */
.ai-chat-header {
  background: linear-gradient(135deg, #ffb6b9 0%, #fae3d9 100%);
  color: #fff;
  padding: 15px;
  font-weight: bold;
  font-family: 'Fredoka One', cursive;
  text-align: center;
  letter-spacing: 1px;
}

/* Body */
.ai-chat-body {
  flex: 1;
  padding: 15px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 12px;
  background: var(--bg-cream, #fffdf8);
}

/* Bubble Wrapper */
.chat-bubble-wrapper {
  display: flex;
  width: 100%;
}
.chat-bubble-wrapper.user {
  justify-content: flex-end;
}
.chat-bubble-wrapper.assistant {
  justify-content: flex-start;
}

/* Bubbles */
.chat-bubble {
  max-width: 80%;
  padding: 10px 14px;
  border-radius: 18px;
  font-size: 0.95rem;
  line-height: 1.4;
  white-space: pre-wrap;
  word-wrap: break-word;
}
.chat-bubble-wrapper.user .chat-bubble {
  background: #ffb6b9;
  color: white;
  border-bottom-right-radius: 4px;
  box-shadow: 2px 4px 8px rgba(255,182,185, 0.2);
}
.chat-bubble-wrapper.assistant .chat-bubble {
  background: #ffffff;
  color: #333;
  border: 1px solid #fae3d9;
  border-bottom-left-radius: 4px;
  box-shadow: 2px 4px 8px rgba(0,0,0,0.04);
}

/* Typing Indicator */
.typing {
  display: flex;
  align-items: center;
  gap: 4px;
  height: 24px;
}
.typing span {
  display: inline-block;
  width: 6px;
  height: 6px;
  background-color: #ffb6b9;
  border-radius: 50%;
  animation: typing-blink 1.4s infinite ease-in-out both;
}
.typing span:nth-child(1) { animation-delay: -0.32s; }
.typing span:nth-child(2) { animation-delay: -0.16s; }
@keyframes typing-blink {
  0%, 80%, 100% { transform: scale(0); opacity: 0.3; }
  40% { transform: scale(1); opacity: 1; }
}

/* Footer */
.ai-chat-footer {
  display: flex;
  padding: 12px;
  background: #ffffff;
  border-top: 1px solid #fae3d9;
  gap: 8px;
}
.ai-chat-footer input {
  flex: 1;
  padding: 8px 12px;
  border: 2px solid #fae3d9;
  border-radius: 20px;
  outline: none;
  font-size: 0.95rem;
  transition: border-color 0.2s;
}
.ai-chat-footer input:focus {
  border-color: #ffb6b9;
}
.ai-chat-footer button {
  background: #ffb6b9;
  color: white;
  border: none;
  border-radius: 20px;
  padding: 0 16px;
  font-weight: bold;
  cursor: pointer;
  transition: background 0.2s;
}
.ai-chat-footer button:hover:not(:disabled) {
  background: #ff9a9e;
}
.ai-chat-footer button:disabled {
  background: #f0f0f0;
  color: #aaa;
  cursor: not-allowed;
}
</style>
