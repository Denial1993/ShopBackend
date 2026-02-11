<template>
  <div class="container" style="margin-top: 100px; margin-bottom: 100px;">
    <div class="row justify-content-center">
      <div class="col-md-7 col-lg-6">
        <div class="card border-0 p-4 clay-card">
          <div class="card-body">
            <div class="d-flex align-items-center mb-5 justify-content-center">
              <div class="avatar-circle me-3 animate__animated animate__bounceIn">
                <i class="bi bi-person-fill text-white fs-2"></i>
              </div>
              <h2 class="h2 fw-bold mb-0 text-dark">個人帳號頁面</h2>
            </div>
            
            <form @submit.prevent="updateProfile">
              <div class="mb-4">
                <label class="form-label text-muted small fw-bold">📧 登入帳號 (Email)</label>
                <div class="input-clay-disabled">
                  <input type="email" class="form-control-plaintext" v-model="profile.email" readonly disabled>
                </div>
                <div class="form-text mt-2"><i class="bi bi-info-circle me-1"></i>電子信箱為帳號唯一識別，不可修改。</div>
              </div>

              <div class="mb-4">
                <label class="form-label fw-bold">👤 真實姓名 / 稱呼</label>
                <input type="text" class="form-control" v-model="profile.fullName" placeholder="例如：王小明" required>
              </div>

              <div class="mb-4">
                <label class="form-label fw-bold">📱 聯絡電話</label>
                <input type="text" class="form-control" v-model="profile.phone" placeholder="例如：0912345678">
              </div>

              <div class="mb-5">
                <label class="form-label fw-bold">🏠 收件地址</label>
                <textarea class="form-control" v-model="profile.address" rows="3" placeholder="請輸入預設配送地址"></textarea>
              </div>

              <div class="d-grid gap-2">
                <button type="submit" class="btn btn-primary py-3 fw-bold rounded-pill shadow-lg" :disabled="loading">
                  <span v-if="loading" class="spinner-border spinner-border-sm me-2" role="status"></span>
                  {{ loading ? '更新中...' : '💾 儲存修改' }}
                </button>
              </div>
            </form>

            <div v-if="message" class="alert mt-4 shadow-sm border-0 animate__animated animate__fadeInUp" :class="messageClass" role="alert">
                <div class="d-flex align-items-center justify-content-center">
                    <i v-if="message.includes('成功')" class="bi bi-check-circle-fill me-2 fs-5"></i>
                    <i v-else class="bi bi-exclamation-circle-fill me-2 fs-5"></i>
                    <span>{{ message }}</span>
                </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { authStore } from '../store.js';

const profile = ref({
  email: '',
  fullName: '',
  phone: '',
  address: ''
});

const loading = ref(false);
const message = ref('');
const messageClass = ref('alert-success');

const fetchProfile = async () => {
  try {
    const res = await axios.get(`${import.meta.env.VITE_API_BASE_URL}/api/profile`);
    profile.value = res.data;
  } catch (err) {
    console.error('取得資料失敗', err);
  }
};

const updateProfile = async () => {
  loading.value = true;
  message.value = '';
  try {
    await axios.put(`${import.meta.env.VITE_API_BASE_URL}/api/profile`, profile.value);
    
    message.value = '修改成功！您的資料已更新。';
    messageClass.value = 'alert-success';
    
    // 同步更新全域狀態
    authStore.userFullName = profile.value.fullName;
    
  } catch (err) {
    message.value = '修改失敗，請稍後再試';
    messageClass.value = 'alert-danger';
  } finally {
    loading.value = false;
  }
};

onMounted(fetchProfile);
</script>

<style scoped>
/* 🐾 個人資料頁 - Claymorphism */

.clay-card {
  background: var(--bg-card);
  border-radius: 32px;
  box-shadow: 12px 12px 30px rgba(174, 160, 140, 0.2),
              -8px -8px 20px rgba(255, 255, 255, 0.9);
}

/* 頭像圓圈 */
.avatar-circle {
  width: 64px;
  height: 64px;
  background: linear-gradient(135deg, var(--coral) 0%, var(--coral-light) 100%);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 4px 4px 10px rgba(255, 107, 107, 0.3);
}

/* 標題 */
h2 {
  font-family: 'Fredoka One', cursive !important;
  color: var(--text-dark);
}

/* 唯讀輸入框樣式 - 內凹 */
.input-clay-disabled {
  background: var(--bg-cream);
  border-radius: 16px;
  padding: 8px 16px;
  box-shadow: inset 2px 2px 5px rgba(174, 160, 140, 0.1),
              inset -2px -2px 5px rgba(255, 255, 255, 0.7);
  opacity: 0.8;
}

.form-control-plaintext {
  font-family: 'Nunito', sans-serif;
  color: var(--text-muted) !important;
  font-weight: 700;
  outline: none;
}

/* 按鈕 */
.btn-primary {
  font-family: 'Fredoka One', cursive;
  letter-spacing: 1px;
  font-size: 1.1rem;
}

/* 成功訊息 */
.alert-success {
  background-color: var(--bg-soft-mint) !important;
  color: var(--mint-dark) !important;
  border: 2px solid var(--mint) !important;
  border-radius: 20px !important;
  font-weight: 700;
}

.alert-danger {
  background-color: var(--bg-soft-pink) !important;
  color: var(--coral-dark) !important;
  border: 2px solid var(--coral) !important;
  border-radius: 20px !important;
  font-weight: 700;
}

/* 響應式 */
@media (max-width: 768px) {
  .clay-card {
    padding: 20px !important;
    border-radius: 24px;
  }
}
</style>
