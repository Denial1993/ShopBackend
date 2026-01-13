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
            <li class="nav-item">
              <span class="nav-link fw-bold text-primary">
                Hi, {{ authStore.userEmail }} 您好!
              </span>
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
/* 全域樣式：讓背景不要那麼死白，稍微灰一點點更有質感 */
body {
  background-color: #f8f9fa;
}
</style>