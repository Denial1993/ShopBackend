<script setup>
import { onMounted } from 'vue';
import { authStore } from './store.js'; // 👈 引入 store
import { useRouter } from 'vue-router'; // 引入 router 做登出跳轉
const router = useRouter();

// 網頁一打開，就檢查登入狀態
onMounted(() => {
  authStore.checkLogin();
});

// 登出按鈕的功能
const handleLogout = () => {
  if (confirm("確定要登出嗎？")) {
    authStore.logout();
    router.push('/'); // 登出後回首頁
  }
}
</script>

<template>
  <nav class="navbar navbar-expand-lg navbar-light bg-white border-bottom shadow-sm fixed-top">
    <div class="container">
      <router-link class="navbar-brand fw-bold fs-3" to="/">🐾 PawPals</router-link>

      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav ms-auto align-items-center">
          <li class="nav-item">
            <router-link class="nav-link" to="/">首頁</router-link>
          </li>
          <li class="nav-item" v-if="!authStore.isLoggedIn">
            <router-link class="nav-link" to="/login">登入 / 註冊</router-link>
          </li>

          <template v-else>
            <li class="nav-item" v-if="authStore.userRole === 'Admin' || authStore.userRole === 'Staff'">
              <router-link class="nav-link text-success fw-bold" to="/admin/products">產品管理</router-link>
            </li>
            <li class="nav-item" v-if="authStore.userRole === 'Admin' || authStore.userRole === 'Staff'">
              <router-link class="nav-link text-info fw-bold" to="/admin/orders">訂單管理</router-link>
            </li>
            <li class="nav-item">
              <span class="nav-link fw-bold text-primary">
                Hi, {{ authStore.userFullName || authStore.userEmail }} 您好!
              </span>
            </li>
            <li class="nav-item">
              <router-link class="nav-link" to="/profile">個人帳號資訊</router-link>
            </li>
            <li class="nav-item">
              <a href="#" class="nav-link text-danger" @click.prevent="handleLogout">登出</a>
            </li>
          </template>
          
          <li class="nav-item">
            <router-link to="/orders" class="nav-link">我的訂單</router-link>
          </li>

          <li class="nav-item">
            <router-link to="/cart" class="nav-link btn btn-primary text-white ms-3 rounded-pill px-4">
              🛒 購物車
            </router-link>
          </li>
        </ul>
      </div>
    </div>
  </nav>

  <div style="margin-top: 80px;">
    <router-view></router-view>
  </div>

</template>

<style>
/* 🐾 PawPals 導航列 - 溫暖寵物風格 */

/* 導航列 - 溫暖白色背景 + 柔和陰影 */
.navbar {
  background: var(--bg-warm-white) !important;
  border-bottom: 3px solid var(--bg-soft-pink) !important;
  box-shadow: 0 4px 20px rgba(174, 160, 140, 0.12) !important;
  backdrop-filter: blur(10px);
}

/* 品牌名稱 - 圓潤字體 + 珊瑚色 */
.navbar-brand {
  font-family: 'Fredoka One', cursive !important;
  font-size: 1.6rem !important;
  color: var(--coral) !important;
  letter-spacing: 1px;
  transition: all 0.3s ease;
}

.navbar-brand:hover {
  color: var(--coral-dark) !important;
  transform: scale(1.05);
}

/* 導航連結 - 圓潤可愛 */
.nav-link {
  font-family: 'Nunito', sans-serif !important;
  font-weight: 700 !important;
  font-size: 0.95rem !important;
  color: var(--text-body) !important;
  transition: all 0.3s ease;
  position: relative;
  padding: 8px 16px !important;
  border-radius: 12px;
}

.nav-link:hover {
  color: var(--coral) !important;
  background: var(--bg-soft-pink);
  transform: translateY(-1px);
}

/* 導航連結底部效果 */
.nav-link::after {
  content: '';
  position: absolute;
  bottom: 2px;
  left: 50%;
  width: 0;
  height: 3px;
  background: var(--coral);
  border-radius: 3px;
  transform: translateX(-50%);
  transition: width 0.3s ease;
}

.nav-link:hover::after {
  width: 60%;
}

/* 特殊角色連結（管理員/員工）*/
.nav-link.text-success {
  color: var(--mint-dark) !important;
  font-weight: 800 !important;
}

.nav-link.text-info {
  color: var(--lavender) !important;
  font-weight: 800 !important;
}

/* 歡迎訊息 */
.nav-link.fw-bold.text-primary {
  color: var(--coral) !important;
}

/* 登出連結 */
.nav-link.text-danger {
  color: var(--coral-dark) !important;
}

.nav-link.text-danger:hover {
  color: #FFFFFF !important;
  background: var(--coral) !important;
}

/* 購物車按鈕 - 珊瑚色膠囊形 */
.nav-link.btn.btn-primary {
  font-family: 'Nunito', sans-serif !important;
  font-weight: 800 !important;
  font-size: 0.9rem !important;
  background: linear-gradient(135deg, var(--coral) 0%, var(--coral-light) 100%) !important;
  border: none !important;
  color: #FFFFFF !important;
  box-shadow: 0 4px 15px rgba(255, 107, 107, 0.35);
  padding: 10px 24px !important;
  border-radius: 50px !important;
  transition: all 0.3s var(--transition-bounce);
}

.nav-link.btn.btn-primary::after {
  display: none;
}

.nav-link.btn.btn-primary:hover {
  background: linear-gradient(135deg, var(--coral-dark) 0%, var(--coral) 100%) !important;
  color: #FFFFFF !important;
  box-shadow: 0 8px 25px rgba(255, 107, 107, 0.45);
  transform: scale(1.05) translateY(-2px);
}

/* 漢堡選單按鈕 */
.navbar-toggler {
  border: 2px solid var(--coral) !important;
  border-radius: 12px;
}

.navbar-toggler-icon {
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 30 30'%3e%3cpath stroke='%23FF6B6B' stroke-linecap='round' stroke-miterlimit='10' stroke-width='2' d='M4 7h22M4 15h22M4 23h22'/%3e%3c/svg%3e") !important;
}

/* 全域背景調整 */
body {
  background-color: var(--bg-cream);
}
</style>