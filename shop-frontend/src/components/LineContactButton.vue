<template>
  <div class="line-contact-wrapper">
    <!-- 懸浮按鈕 -->
    <div class="line-fab" @click="handleLineClick" title="加入官方 LINE 帳號">
      <svg class="line-icon" viewBox="0 0 48 48" xmlns="http://www.w3.org/2000/svg">
        <path fill="#ffffff" d="M37.113,22.417c0-5.865-5.88-10.637-13.107-10.637s-13.108,4.772-13.108,10.637c0,5.258,4.663,9.662,10.962,10.495c0.427,0.056,1.008,0.172,1.153,0.602c0.131,0.385,0.086,0.985,0.042,1.373c-0.057,0.505-0.27,1.583,1.353,0.903c1.624-0.681,8.769-5.166,11.238-8.298C36.632,25.961,37.113,24.26,37.113,22.417z M18.875,25.907h-2.604c-0.379,0-0.687-0.308-0.687-0.688V20.01c0-0.379,0.308-0.688,0.687-0.688c0.379,0,0.688,0.308,0.688,0.688v4.521h1.916c0.379,0,0.688,0.308,0.688,0.688C19.563,25.599,19.254,25.907,18.875,25.907z M21.568,25.219c0,0.379-0.308,0.688-0.687,0.688s-0.687-0.308-0.687-0.688V20.01c0-0.379,0.308-0.688,0.687-0.688s0.687,0.308,0.687,0.688V25.219z M27.838,25.219c0,0.379-0.308,0.688-0.687,0.688h-0.003c-0.198,0-0.383-0.086-0.51-0.232l-2.617-3.033v2.577c0,0.379-0.308,0.688-0.688,0.688c-0.379,0-0.688-0.308-0.688-0.688V20.01c0-0.379,0.308-0.688,0.688-0.688c0.231,0,0.443,0.113,0.573,0.303l2.558,3.753v-3.368c0-0.379,0.308-0.688,0.688-0.688c0.379,0,0.686,0.308,0.686,0.688V25.219z M32.052,21.927h-1.917v1.216h1.917c0.379,0,0.688,0.308,0.688,0.688c0,0.379-0.308,0.688-0.688,0.688h-2.604c-0.379,0-0.687-0.308-0.687-0.688V20.01c0-0.379,0.308-0.688,0.687-0.688h2.604c0.379,0,0.688,0.308,0.688,0.688c0,0.379-0.308,0.688-0.688,0.688h-1.917v1.216h1.917c0.379,0,0.688,0.308,0.688,0.688C32.74,21.619,32.431,21.927,32.052,21.927z"/>
      </svg>
    </div>

    <!-- 彈窗 (Modal) -->
    <transition name="fade">
      <div v-if="showModal" class="modal-overlay" @click="closeModal">
        <div class="modal-content" @click.stop>
          <button class="close-btn" @click="closeModal" aria-label="關閉">×</button>
          <div class="modal-header">
            <h3>掃描加我 LINE</h3>
          </div>
          <div class="modal-body">
            <!-- 替換成您自己的 QR Code 圖片路徑 -->
            <img src="/images/line-qrcode.png" alt="LINE QR Code" class="qr-code-img" />
            <p class="line-id">
              或點擊連結：<br />
              <a :href="lineLink" target="_blank" rel="noopener noreferrer">{{ lineLink }}</a>
            </p>
          </div>
        </div>
      </div>
    </transition>
  </div>
</template>

<script setup>
import { ref } from 'vue'

// LINE 加好友連結
const lineLink = 'https://line.me/ti/p/zOwllvLSvX'

// 控制彈窗顯示的狀態
const showModal = ref(false)

// 判斷是否為手機/行動裝置
const checkIsMobile = () => {
  const userAgent = navigator.userAgent || navigator.vendor || window.opera
  // 利用正則表達式檢查常見的行動裝置關鍵字
  return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini/i.test(userAgent.toLowerCase())
}

// 點擊懸浮按鈕的處理邏輯
const handleLineClick = () => {
  const isMobile = checkIsMobile()
  
  if (isMobile) {
    // 若為手機/行動裝置，直接跳轉 LINE 連結（開新分頁確保體驗）
    window.open(lineLink, '_blank', 'noopener,noreferrer')
  } else {
    // 若為電腦/桌面版，顯示 QR Code 彈窗
    showModal.value = true
  }
}

// 關閉彈窗
const closeModal = () => {
  showModal.value = false
}
</script>

<style scoped>
/* =========== 懸浮按鈕樣式 =========== */
.line-contact-wrapper {
  position: fixed;
  bottom: 30px;
  right: 30px;
  z-index: 9999;
}

.line-fab {
  display: flex;
  justify-content: center;
  align-items: center;
  width: 64px;
  height: 64px;
  background-color: #06C755; /* LINE 官方綠色 */
  border-radius: 50%;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  cursor: pointer;
  /* 帶有稍微回彈感 (cubic-bezier) 的過渡動畫 */
  transition: transform 0.3s cubic-bezier(0.175, 0.885, 0.32, 1.275), box-shadow 0.3s ease;
}

.line-fab:hover {
  transform: scale(1.1);
  box-shadow: 0 6px 16px rgba(6, 199, 85, 0.35);
}

.line-icon {
  width: 40px;
  height: 40px;
}

/* =========== 彈窗樣式 =========== */
/* 背景遮罩 (Overlay) */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  /* 半透明的深色背景 */
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 10000;
  backdrop-filter: blur(2px); /* 輕微毛玻璃效果（非必備，但增加質感） */
}

/* 彈窗主體 */
.modal-content {
  background-color: #ffffff;
  border-radius: 16px;
  padding: 32px 24px;
  width: 90%;
  max-width: 320px;
  text-align: center;
  position: relative;
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2);
  /* 彈出時的小動畫 */
  animation: modalPop 0.3s ease-out;
}

/* 關閉按鈕 */
.close-btn {
  position: absolute;
  top: 12px;
  right: 12px;
  background: none;
  border: none;
  font-size: 28px;
  line-height: 1;
  color: #999;
  cursor: pointer;
  width: 32px;
  height: 32px;
  display: flex;
  justify-content: center;
  align-items: center;
  border-radius: 50%;
  transition: color 0.2s ease, background-color 0.2s ease;
}

.close-btn:hover {
  color: #333;
  background-color: #f0f0f0;
}

/* 標題文字 */
.modal-header h3 {
  margin: 0 0 20px 0;
  color: #333;
  font-size: 20px;
  font-weight: 600;
}

/* 內容區塊 */
.modal-body {
  display: flex;
  flex-direction: column;
  align-items: center;
}

/* QR Code 圖片 */
.qr-code-img {
  width: 200px;
  height: 200px;
  object-fit: contain;
  border: 1px solid #eee;
  border-radius: 8px;
  padding: 8px;
  margin-bottom: 16px;
}

/* 連結文字 */
.line-id {
  font-size: 14px;
  color: #666;
  margin: 0;
  line-height: 1.6;
}

.line-id a {
  color: #06C755; /* 跟隨 LINE 主題色 */
  text-decoration: none;
  font-weight: 500;
  word-break: break-all;
}

.line-id a:hover {
  text-decoration: underline;
}

/* =========== Vue Transition 動畫設定 =========== */
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.3s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* 彈窗內容跳出的 Keyframe */
@keyframes modalPop {
  0% {
    opacity: 0;
    transform: scale(0.9);
  }
  100% {
    opacity: 1;
    transform: scale(1);
  }
}
</style>
