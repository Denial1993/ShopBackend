import { createRouter, createWebHistory } from "vue-router";
// .. 代表上一層資料夾，所以是從 router 資料夾跳出來，再進去 views 資料夾
import HomeView from "../views/HomeView.vue";
import LoginView from "../views/LoginView.vue";
import CartView from "../views/CartView.vue"; // 👈 引入

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "home",
      component: HomeView,
    },
    {
      path: "/login",
      name: "login",
      component: LoginView,
    },
    { path: "/cart", 
      name: "cart", 
      component: CartView }, 
  ],
});

export default router;
