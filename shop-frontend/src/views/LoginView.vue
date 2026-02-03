<script setup>
import { ref } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router';
import { authStore } from '../store.js'; // 👈 1. 引入共享大腦

const router = useRouter();

const loginData = ref({
  email: '',
  password: ''
});

const handleLogin = async () => {
  try {
    const response = await axios.post(`${import.meta.env.VITE_API_BASE_URL}/api/Auth/login`, loginData.value);

    const token = response.data;
    // 👇 2. 改用 store 的方法登入 (這樣 App.vue 才會知道)
    authStore.login(token, loginData.value.email);

    alert("🎉 登入成功！");
    router.push('/');

  } catch (error) {
    console.error(error);
    alert("❌ 登入失敗：請檢查帳號密碼");
  }
};
</script>

<template>
  <div class="d-flex align-items-center justify-content-center" style="min-height: 80vh;">

    <div class="card shadow-lg border-0 rounded-4" style="width: 100%; max-width: 400px;">
      <div class="card-body p-5">

        <h2 class="text-center fw-bold mb-4">Ubtiv</h2>
        <p class="text-center text-muted mb-4">歡迎回來，請登入您的帳戶</p>

        <form @submit.prevent="handleLogin">
          <div class="form-floating mb-3">
            <input v-model="loginData.email" type="text" class="form-control" id="floatingInput"
              placeholder="name@example.com" required>
            <label for="floatingInput">Email / 帳號</label>
          </div>

          <div class="form-floating mb-4">
            <input v-model="loginData.password" type="password" class="form-control" id="floatingPassword"
              placeholder="Password" required>
            <label for="floatingPassword">密碼</label>
          </div>

          <button type="submit" class="btn btn-primary w-100 py-3 fw-bold rounded-pill">
            立即登入
          </button>
        </form>

        <div class="text-center mt-4">
          <span class="text-muted small">還沒有帳號？</span>
          <router-link to="/register" class="text-primary small fw-bold text-decoration-none ms-1">
            立即註冊
          </router-link>
        </div>

      </div>
    </div>
  </div>
</template>

<style scoped>
/* 🎮 登入頁面 - 遊戲霓虹風格 */

/* 登入卡片 - 霓虹邊框 */
.card.shadow-lg {
  background: var(--bg-dark-card) !important;
  border: 3px solid var(--neon-purple) !important;
  box-shadow: 0 0 30px rgba(124, 58, 237, 0.6),
              0 15px 50px rgba(0, 0, 0, 0.6) !important;
  border-radius: 15px !important;
}

/* 品牌標題 */
h2.text-center {
  font-family: 'Press Start 2P', cursive !important;
  color: var(--neon-pink) !important;
  text-shadow: 0 0 15px var(--neon-pink),
               0 0 30px var(--neon-pink),
               0 0 60px var(--neon-pink);
  font-size: 1.8rem !important;
  letter-spacing: 4px;
}

/* 歡迎文字 */
.text-muted {
  font-family: 'VT323', monospace !important;
  color: var(--text-secondary) !important;
  font-size: 1.1rem !important;
}

/* 表單輸入框樣式已在全域 CSS 定義 */
.form-control:focus {
  box-shadow: 0 0 15px var(--neon-pink) !important;
  border-color: var(--neon-pink) !important;
}

/* 浮動標籤 */
.form-floating label {
  font-family: 'VT323', monospace !important;
  color: var(--neon-purple-light) !important;
  font-size: 1rem;
}

/* 登入按鈕 */
.btn-primary.w-100 {
  font-family: 'Press Start 2P', cursive !important;
  font-size: 0.8rem !important;
  background: transparent !important;
  border: 3px solid var(--neon-pink) !important;
  color: var(--neon-pink) !important;
  text-shadow: 0 0 10px var(--neon-pink);
  box-shadow: 0 0 20px rgba(244, 63, 94, 0.6);
  border-radius: 25px !important;
  transition: all 0.3s ease;
}

.btn-primary.w-100:hover {
  background: var(--neon-pink) !important;
  color: var(--bg-dark) !important;
  text-shadow: none;
  box-shadow: 0 0 40px var(--neon-pink),
              0 0 80px var(--neon-pink);
  transform: scale(1.05);
}

/* 註冊連結 */
.text-primary.small {
  color: var(--neon-cyan) !important;
  font-family: 'VT323', monospace !important;
  font-size: 1rem !important;
  text-shadow: 0 0 5px var(--neon-cyan);
  transition: all 0.2s ease;
}

.text-primary.small:hover {
  color: var(--neon-purple) !important;
  text-shadow: 0 0 10px var(--neon-purple);
}

.small.text-muted {
  font-family: 'VT323', monospace;
  font-size: 0.95rem !important;
}
</style>