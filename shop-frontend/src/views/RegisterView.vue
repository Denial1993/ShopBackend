<script setup>
import { ref } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router';

const router = useRouter();

// 定義表單資料
const registerData = ref({
    email: '',
    password: '',
    confirmPassword: '' // 前端自己驗證用，不傳給後端
});

const handleRegister = async () => {
    // 1. 前端先檢查兩次密碼是否一樣
    if (registerData.value.password !== registerData.value.confirmPassword) {
        alert("❌ 兩次密碼輸入不一致！");
        return;
    }

    try {
        // 2. 發送註冊請求 (只傳 email 和 password)
        // ⚠️ 注意：根據你的 Swagger，後端接受的是 { email, password }
        const payload = {
            email: registerData.value.email,
            password: registerData.value.password
        };

        await axios.post(`${import.meta.env.VITE_API_BASE_URL}/api/Auth/register`, payload);

        // 3. 註冊成功，引導去登入
        alert("🎉 註冊成功！請使用剛註冊的帳號登入。");
        router.push('/login');

    } catch (error) {
        console.error(error);

        // 定義一個預設的錯誤訊息
        let alertMessage = "註冊失敗，請稍後再試。";

        if (error.response && error.response.data) {
            const data = error.response.data;

            if (data.errors) {
                alertMessage = Object.values(data.errors).flat().join("\n");
            }
            else if (Array.isArray(data)) {
                alertMessage = data.join("\n");
            }
            else if (typeof data === 'string') {
                alertMessage = data;
            }
        }

        // 最後只顯示乾淨的文字給使用者
        alert("❌ 註冊失敗：\n" + alertMessage);
    }
};
</script>

<template>
    <div class="d-flex align-items-center justify-content-center" style="min-height: 80vh;">

        <div class="card shadow-lg border-0 rounded-4" style="width: 100%; max-width: 420px;">
            <div class="card-body p-5">

                <h2 class="text-center fw-bold mb-2">🐾 加入會員</h2>
                <p class="text-center text-muted mb-4">建立您的 PawPals 帳戶</p>

                <form @submit.prevent="handleRegister">
                    <div class="form-floating mb-3">
                        <input v-model="registerData.email" type="email" class="form-control" id="regEmail"
                            placeholder="name@example.com" required>
                        <label for="regEmail">Email 信箱</label>
                    </div>

                    <div class="form-floating mb-3">
                        <input v-model="registerData.password" type="password" class="form-control" id="regPass"
                            placeholder="Password" required>
                        <label for="regPass">設定密碼</label>
                    </div>

                    <div class="form-floating mb-4">
                        <input v-model="registerData.confirmPassword" type="password" class="form-control"
                            id="regConfirmPass" placeholder="Password" required>
                        <label for="regConfirmPass">再次輸入密碼</label>
                    </div>

                    <button type="submit" class="btn btn-primary w-100 py-3 fw-bold rounded-pill">
                        註冊帳號
                    </button>
                </form>

                <div class="text-center mt-4">
                    <span class="text-muted small">已經有帳號了？</span>
                    <router-link to="/login" class="text-primary small fw-bold text-decoration-none ms-1">
                        直接登入
                    </router-link>
                </div>

            </div>
        </div>
    </div>
</template>

<style scoped>
/* 🐾 註冊頁面 - 寵物 Claymorphism 風格 */

.card.shadow-lg {
  background: var(--bg-card) !important;
  border: none !important;
  box-shadow: 12px 12px 30px rgba(174, 160, 140, 0.25),
              -6px -6px 16px rgba(255, 255, 255, 0.8) !important;
  border-radius: 28px !important;
}

h2.text-center {
  font-family: 'Fredoka One', cursive !important;
  color: var(--coral) !important;
  font-size: 1.8rem !important;
}

.text-muted {
  font-family: 'Nunito', sans-serif !important;
  color: var(--text-muted) !important;
}

.form-control:focus {
  box-shadow: 0 0 0 4px rgba(255, 107, 107, 0.15) !important;
  border-color: var(--coral) !important;
}

.form-floating label {
  font-family: 'Nunito', sans-serif !important;
  color: var(--text-muted) !important;
}

.btn-primary.w-100 {
  font-family: 'Nunito', sans-serif !important;
  font-weight: 800 !important;
  background: linear-gradient(135deg, var(--coral) 0%, var(--coral-light) 100%) !important;
  border: none !important;
  color: #FFFFFF !important;
  box-shadow: 0 6px 20px rgba(255, 107, 107, 0.35);
  border-radius: 50px !important;
  transition: all 0.35s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.btn-primary.w-100:hover {
  background: linear-gradient(135deg, var(--coral-dark) 0%, var(--coral) 100%) !important;
  box-shadow: 0 10px 30px rgba(255, 107, 107, 0.45);
  transform: translateY(-2px) scale(1.02);
}

.text-primary.small {
  color: var(--coral) !important;
  font-family: 'Nunito', sans-serif !important;
  font-weight: 800 !important;
}

.text-primary.small:hover {
  color: var(--coral-dark) !important;
}
</style>