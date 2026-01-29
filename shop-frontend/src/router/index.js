import { createRouter, createWebHistory } from "vue-router";
// .. 代表上一層資料夾，所以是從 router 資料夾跳出來，再進去 views 資料夾
import HomeView from "../views/HomeView.vue";
import LoginView from "../views/LoginView.vue";
import CartView from "../views/CartView.vue";
import OrderListView from "../views/OrderListView.vue";
import OrderDetailView from "../views/OrderDetailView.vue";
import RegisterView from "../views/RegisterView.vue";
import ProductDetailView from "../views/ProductDetailView.vue";
import ProfileView from "../views/ProfileView.vue"; // 👈 加入這一行

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: "/", name: "home", component: HomeView },
    { path: "/login", name: "login", component: LoginView },
    { path: "/cart", name: "cart", component: CartView },
    { path: "/orders", name: "orders", component: OrderListView },
    { path: "/orders/:id", name: "order-detail", component: OrderDetailView },
    { path: "/register", name: "register", component: RegisterView },
    { path: "/profile", name: "profile", component: ProfileView }, // 👈 加入這一行
    {
      path: "/product/:id",
      name: "product-detail",
      component: ProductDetailView,
    },
    {
      path: "/admin/products",
      name: "admin-products",
      component: () => import("../views/AdminProductsView.vue"),
    },
    {
      path: "/admin/orders",
      name: "admin-orders",
      component: () => import("../views/AdminOrdersView.vue"),
    },
  ],
});

export default router;
