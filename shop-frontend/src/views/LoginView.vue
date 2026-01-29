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
/* 讓輸入框被點選時，外框比較好看 */
.form-control:focus {
  box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.15);
  border-color: #0d6efd;
}
</style>