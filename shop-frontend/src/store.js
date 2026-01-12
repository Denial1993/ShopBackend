// src/store.js
import { reactive } from 'vue';

export const authStore = reactive({
  isLoggedIn: false,
  userEmail: '',

  // 初始化：一開網頁就檢查有沒有 Token
  checkLogin() {
    const token = localStorage.getItem('shop_token');
    const email = localStorage.getItem('user_email');
    if (token) {
      this.isLoggedIn = true;
      this.userEmail = email || '會員';
    } else {
      this.isLoggedIn = false;
      this.userEmail = '';
    }
  },

  // 登入動作：存 Token 並更新狀態
  login(token, email) {
    localStorage.setItem('shop_token', token);
    localStorage.setItem('user_email', email); // 把 Email 也存起來方便顯示
    this.checkLogin(); // 更新狀態
  },

  // 登出動作：清空 Token 並更新狀態
  logout() {
    localStorage.removeItem('shop_token');
    localStorage.removeItem('user_email');
    this.checkLogin(); // 更新狀態
  }
});