import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router' // 👈 路由
// 👇 加入這一行，引入 Bootstrap 的 CSS
import 'bootstrap/dist/css/bootstrap.min.css'
// 👇 加入這一行，引入 Bootstrap 的 JS (為了讓手機版漢堡選單會動)
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import axios from 'axios'
// 👇 2. 設定全域攔截器 (這段最重要！)
axios.interceptors.request.use(config => {
  const token = sessionStorage.getItem('shop_token');
  if (token) {
    // 如果有 Token，就加在 Header 裡：Authorization: Bearer xxxxx
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

const app = createApp(App)
app.use(router) // 👈 啟用路由
app.mount('#app')