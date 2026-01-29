<template>
  <div class="container" style="margin-top: 100px; margin-bottom: 100px;">
    <div class="row justify-content-center">
      <div class="col-md-6">
        <div class="card shadow-sm border-0 rounded-4">
          <div class="card-body p-4 p-md-5">
            <div class="d-flex align-items-center mb-4">
              <div class="bg-primary bg-opacity-10 p-3 rounded-circle me-3">
                <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" class="bi bi-person-fill text-primary" viewBox="0 0 16 16">
                  <path d="M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6"/>
                </svg>
              </div>
              <h2 class="h2 fw-bold mb-0">個人帳號資訊</h2>
            </div>
            
            <form @submit.prevent="updateProfile">
              <div class="mb-3">
                <label class="form-label text-secondary small fw-bold">登入帳號 (Email)</label>
                <input type="email" class="form-control bg-light border-0 py-2" v-model="profile.email" readonly disabled>
                <div class="form-text">電子信箱為帳號唯一識別，不可修改。</div>
              </div>

              <div class="mb-3">
                <label class="form-label fw-bold">真實姓名 / 稱呼</label>
                <input type="text" class="form-control py-2" v-model="profile.fullName" placeholder="例如：曾先生" required>
              </div>

              <div class="mb-3">
                <label class="form-label fw-bold">聯絡電話</label>
                <input type="text" class="form-control py-2" v-model="profile.phone" placeholder="例如：0912345678">
              </div>

              <div class="mb-4">
                <label class="form-label fw-bold">收件地址</label>
                <textarea class="form-control" v-model="profile.address" rows="3" placeholder="請輸入預設配送地址"></textarea>
              </div>

              <div class="d-grid shadow-sm">
                <button type="submit" class="btn btn-primary py-3 rounded-3 fw-bold" :disabled="loading">
                  <span v-if="loading" class="spinner-border spinner-border-sm me-2" role="status"></span>
                  {{ loading ? '更新中...' : '儲存修改' }}
                </button>
              </div>
            </form>

            <div v-if="message" class="alert mt-4 shadow-sm border-0 animate__animated animate__fadeIn" :class="messageClass" role="alert">
                <div class="d-flex align-items-center">
                    <svg v-if="message.includes('成功')" xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-check-circle-fill me-2" viewBox="0 0 16 16">
                        <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0m-3.97-3.03a.75.75 0 0 0-1.08.022L7.477 9.417 5.384 7.323a.75.75 0 0 0-1.06 1.06L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-.01-1.05z"/>
                    </svg>
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
    
    message.value = '修改成功！您的姓名已更新，問候語將立即改變。';
    messageClass.value = 'alert-success text-success bg-success bg-opacity-10';
    
    // 同步更新全域狀態，讓導覽列立即顯示新姓名
    authStore.userFullName = profile.value.fullName;
    
  } catch (err) {
    message.value = '修改失敗，請稍後再試';
    messageClass.value = 'alert-danger text-danger bg-danger bg-opacity-10';
  } finally {
    loading.value = false;
  }
};

onMounted(fetchProfile);
</script>

<style scoped>
.form-control:focus {
    border-color: #0d6efd;
    box-shadow: 0 0 0 0.25rem rgba(13, 110, 253, 0.1);
}
.rounded-4 {
    border-radius: 1rem !important;
}
</style>
