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
      <router-link class="navbar-brand fw-bold fs-3" to="/">Ubtiv</router-link>

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
              購物車
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
/* 🎮 遊戲平台導航列 - 霓虹風格 */

/* 導航列深色主題 + 霓虹邊框 */
.navbar {
  background: var(--bg-dark-card) !important;
  border-bottom: 3px solid var(--neon-purple) !important;
  box-shadow: 0 0 20px rgba(124, 58, 237, 0.4),
              0 4px 10px rgba(0, 0, 0, 0.5) !important;
  backdrop-filter: blur(10px);
}

/* 品牌名稱 - 像素字體 + 霓虹光暈 */
.navbar-brand {
  font-family: 'Press Start 2P', cursive !important;
  font-size: 1.5rem !important;
  color: var(--neon-pink) !important;
  text-shadow: 0 0 10px var(--neon-pink),
               0 0 20px var(--neon-pink),
               0 0 40px var(--neon-pink);
  letter-spacing: 3px;
  transition: all 0.3s ease;
}

.navbar-brand:hover {
  color: var(--neon-purple) !important;
  text-shadow: 0 0 10px var(--neon-purple),
               0 0 20px var(--neon-purple),
               0 0 40px var(--neon-purple),
               0 0 80px var(--neon-purple);
  transform: scale(1.05);
  animation: glitch 0.3s ease-in-out;
}

/* 導航連結 - 霓虹效果 */
.nav-link {
  font-family: 'VT323', monospace !important;
  font-size: 1.2rem !important;
  color: var(--neon-purple-light) !important;
  transition: all 0.3s ease;
  position: relative;
  padding: 8px 16px !important;
}

.nav-link:hover {
  color: var(--neon-cyan) !important;
  text-shadow: 0 0 5px var(--neon-cyan),
               0 0 10px var(--neon-cyan);
  transform: translateY(-2px);
}

/* 導航連結底部霓虹線條效果 */
.nav-link::after {
  content: '';
  position: absolute;
  bottom: 0;
  left: 50%;
  width: 0;
  height: 2px;
  background: var(--neon-cyan);
  box-shadow: 0 0 5px var(--neon-cyan);
  transform: translateX(-50%);
  transition: width 0.3s ease;
}

.nav-link:hover::after {
  width: 80%;
}

/* 特殊角色連結（管理員/員工）*/
.nav-link.text-success {
  color: var(--neon-green) !important;
  text-shadow: 0 0 5px var(--neon-green);
  font-weight: bold;
}

.nav-link.text-info {
  color: var(--neon-cyan) !important;
  text-shadow: 0 0 5px var(--neon-cyan);
  font-weight: bold;
}

/* 歡迎訊息 */
.nav-link.fw-bold.text-primary {
  color: var(--neon-purple) !important;
  text-shadow: 0 0 5px var(--neon-purple);
}

/* 登出連結 */
.nav-link.text-danger {
  color: var(--neon-pink) !important;
  text-shadow: 0 0 5px var(--neon-pink);
}

.nav-link.text-danger:hover {
  color: #FF1744 !important;
  text-shadow: 0 0 10px #FF1744,
               0 0 20px #FF1744;
}

/* 購物車按鈕 - 霓虹粉紅 CTA */
.nav-link.btn.btn-primary {
  font-family: 'Press Start 2P', cursive !important;
  font-size: 0.7rem !important;
  background: transparent !important;
  border: 2px solid var(--neon-pink) !important;
  color: var(--neon-pink) !important;
  text-shadow: 0 0 5px var(--neon-pink);
  box-shadow: 0 0 10px rgba(244, 63, 94, 0.4);
  padding: 10px 20px !important;
  border-radius: 25px !important;
  transition: all 0.3s ease;
}

.nav-link.btn.btn-primary::after {
  display: none; /* 移除底部線條 */
}

.nav-link.btn.btn-primary:hover {
  background: var(--neon-pink) !important;
  color: var(--bg-dark) !important;
  text-shadow: none;
  box-shadow: 0 0 20px var(--neon-pink),
              0 0 40px var(--neon-pink);
  transform: scale(1.1) translateY(-2px);
}

/* 漢堡選單按鈕 - 霓虹風格 */
.navbar-toggler {
  border: 2px solid var(--neon-purple) !important;
  box-shadow: 0 0 5px var(--neon-purple);
}

.navbar-toggler-icon {
  background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 30 30'%3e%3cpath stroke='%237C3AED' stroke-linecap='round' stroke-miterlimit='10' stroke-width='2' d='M4 7h22M4 15h22M4 23h22'/%3e%3c/svg%3e") !important;
}

/* 全域背景調整 */
body {
  background-color: var(--bg-dark);
}
</style>