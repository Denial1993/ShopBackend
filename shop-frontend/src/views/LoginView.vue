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

    <div class="card shadow-lg border-0 rounded-4" style="width: 100%; max-width: 420px;">
      <div class="card-body p-5">

        <h2 class="text-center fw-bold mb-2">🐾 PawPals</h2>
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
/* 🐾 登入頁面 - 寵物溫暖 Claymorphism 風格 */

/* 登入卡片 - Claymorphism */
.card.shadow-lg {
  background: var(--bg-card) !important;
  border: none !important;
  box-shadow: 12px 12px 30px rgba(174, 160, 140, 0.25),
              -6px -6px 16px rgba(255, 255, 255, 0.8) !important;
  border-radius: 28px !important;
}

/* 品牌標題 */
h2.text-center {
  font-family: 'Fredoka One', cursive !important;
  color: var(--coral) !important;
  font-size: 2rem !important;
  letter-spacing: 2px;
}

/* 歡迎文字 */
.text-muted {
  font-family: 'Nunito', sans-serif !important;
  color: var(--text-muted) !important;
  font-size: 0.95rem !important;
}

/* 表單輸入框焦點 */
.form-control:focus {
  box-shadow: 0 0 0 4px rgba(255, 107, 107, 0.15) !important;
  border-color: var(--coral) !important;
}

/* 浮動標籤 */
.form-floating label {
  font-family: 'Nunito', sans-serif !important;
  color: var(--text-muted) !important;
  font-size: 0.95rem;
}

/* 登入按鈕 */
.btn-primary.w-100 {
  font-family: 'Nunito', sans-serif !important;
  font-weight: 800 !important;
  font-size: 1rem !important;
  background: linear-gradient(135deg, var(--coral) 0%, var(--coral-light) 100%) !important;
  border: none !important;
  color: #FFFFFF !important;
  box-shadow: 0 6px 20px rgba(255, 107, 107, 0.35);
  border-radius: 50px !important;
  transition: all 0.35s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.btn-primary.w-100:hover {
  background: linear-gradient(135deg, var(--coral-dark) 0%, var(--coral) 100%) !important;
  color: #FFFFFF !important;
  box-shadow: 0 10px 30px rgba(255, 107, 107, 0.45);
  transform: translateY(-2px) scale(1.02);
}

/* 註冊連結 */
.text-primary.small {
  color: var(--coral) !important;
  font-family: 'Nunito', sans-serif !important;
  font-weight: 800 !important;
  font-size: 0.9rem !important;
  transition: all 0.2s ease;
}

.text-primary.small:hover {
  color: var(--coral-dark) !important;
}

.small.text-muted {
  font-family: 'Nunito', sans-serif;
  font-size: 0.9rem !important;
}
</style>