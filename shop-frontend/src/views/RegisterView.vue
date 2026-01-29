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

            // 🕵️‍♂️ 情況 A: 後端回傳的是標準錯誤物件 (包含 errors 欄位)
            // 例如: { errors: { Password: ["密碼太短", "沒大寫"], Email: ["格式錯誤"] } }
            if (data.errors) {
                // 1. 取出所有錯誤訊息陣列 (Object.values)
                // 2. 把多個陣列扁平化成一個陣列 (flat)
                // 3. 用換行符號接起來 (join)
                alertMessage = Object.values(data.errors).flat().join("\n");
            }
            // 🕵️‍♂️ 情況 B: 後端直接回傳字串陣列 (有時候 ASP.NET 會這樣)
            else if (Array.isArray(data)) {
                alertMessage = data.join("\n");
            }
            // 🕵️‍♂️ 情況 C: 後端直接回傳純文字
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

        <div class="card shadow-lg border-0 rounded-4" style="width: 100%; max-width: 400px;">
            <div class="card-body p-5">

                <h2 class="text-center fw-bold mb-4">加入會員</h2>
                <p class="text-center text-muted mb-4">建立您的 ShopDemo 帳戶</p>

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

                    <button type="submit" class="btn btn-dark w-100 py-3 fw-bold rounded-pill">
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
.form-control:focus {
    box-shadow: 0 0 0 0.25rem rgba(33, 37, 41, 0.15);
    /* 改成深色陰影搭配 Dark 按鈕 */
    border-color: #212529;
}
</style>