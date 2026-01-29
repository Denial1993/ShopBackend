// src/store.js
import { reactive } from 'vue';

export const authStore = reactive({
  isLoggedIn: false,
  userEmail: '',
  userRole: '',
  userFullName: '', // 👈 新增姓名欄位

  // 初始化：一開網頁就檢查有沒有 Token
  checkLogin() {
    const token = localStorage.getItem('shop_token');
    if (token) {
      this.isLoggedIn = true;

      // 解析 JWT 取得資訊
      try {
        // 使用 Unicode 安全的解析方式
        const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(c =>
          '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
        ).join(''));

        const payload = JSON.parse(jsonPayload);

        this.userEmail = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || payload["unique_name"] || '';
        this.userRole = payload["role"] || payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || '';
        this.userFullName = payload["fullName"] || '';
      } catch (e) {
        console.error("Token 解析失敗", e);
        this.logout(); // 解析失敗就強制登出
      }
    } else {
      this.isLoggedIn = false;
      this.userEmail = '';
      this.userRole = '';
      this.userFullName = '';
    }
  },

  // 登入動作：存 Token 並更新狀態
  login(token) {
    localStorage.setItem('shop_token', token);
    this.checkLogin(); // 更新狀態
  },

  // 登出動作：清空 Token 並更新狀態
  logout() {
    localStorage.removeItem('shop_token');
    this.checkLogin(); // 更新狀態
  }
});