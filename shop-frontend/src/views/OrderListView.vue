<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router'; // 用來跳轉詳情頁
import { formatPrice } from '../utils/format.js'; // 👈 引入
const orders = ref([]);
const router = useRouter();

// 一進來就抓訂單歷史
const fetchOrders = async () => {
  try {
    const response = await axios.get(`${import.meta.env.VITE_API_BASE_URL}/api/Order`);
    orders.value = response.data; // 根據你的截圖，這應該是一個陣列
  } catch (error) {
    console.error("無法取得訂單", error);
  }
};

// 簡單的日期格式化工具
const formatDate = (dateString) => {
  return new Date(dateString).toLocaleString();
}

// 跳轉到詳情頁
const viewDetail = (orderId) => {
  router.push(`/orders/${orderId}`);
}

// 👇 新增：處理付款邏輯
const handlePayment = async (orderId) => {
  try {
    // 1. 呼叫後端 API (傳送 OrderId)
    // 注意：後端會回傳一整段 HTML 字串
    const response = await axios.post(`${import.meta.env.VITE_API_BASE_URL}/api/Payment/Checkout`,
      { OrderId: orderId }
    );

    // 2. 這是最關鍵的一步！
    // 我們要把原本的 Vue 網頁「蓋掉」，換成後端給的綠界跳轉頁面
    document.write(response.data);

    // 3. 關閉文件流，讓瀏覽器知道寫完了，開始執行新頁面的 Script
    // 這時候瀏覽器就會執行 HTML 裡的 submit()，把你帶去綠界
    document.close();

  } catch (error) {
    console.error(error);
    alert("前往付款失敗，請稍後再試");
  }
};

onMounted(() => {
  fetchOrders();
});
</script>

<template>
  <div class="container" style="margin-top: 100px;">
    <h2 class="mb-5 fw-bold text-center">📜 我的訂單歷史</h2>

    <div v-if="orders.length === 0" class="alert clay-alert-info text-center py-5">
      <i class="bi bi-cart-x fs-1 mb-3 d-block text-muted"></i>
      <h4 class="fw-bold text-muted">目前還沒有訂單紀錄喔</h4>
      <p class="text-muted">快去逛逛，給毛小孩買點禮物吧！</p>
      <router-link to="/" class="btn btn-primary mt-3 rounded-pill px-4">前往商店</router-link>
    </div>

    <div v-else class="table-responsive clay-card p-4">
      <table class="table table-hover align-middle mb-0">
        <thead class="bg-light">
          <tr>
            <th>訂單編號</th>
            <th>下單日期</th>
            <th>總金額</th>
            <th>狀態</th>
            <th class="text-end">操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="order in orders" :key="order.id">
            <td class="fw-bold text-dark">#{{ order.id }}</td>
            <td class="text-muted">{{ formatDate(order.createdAt) }}</td>
            <td class="fw-bold text-danger">NT$ {{ formatPrice(order.totalAmount) }}</td>
            <td>
              <span v-if="order.status === 'Paid'" class="badge bg-success rounded-pill px-3 py-2">
                <i class="bi bi-check-circle-fill me-1"></i>已付款
              </span>
              <span v-else class="badge bg-secondary rounded-pill px-3 py-2">
                <i class="bi bi-hourglass-split me-1"></i>未付款
              </span>
            </td>
            <td class="text-end">
              <button @click="viewDetail(order.id)" class="btn btn-outline-dark btn-sm rounded-pill me-2">
                詳情
              </button>
              <button v-if="order.status !== 'Paid'" @click="handlePayment(order.id)" class="btn btn-primary btn-sm rounded-pill shadow-sm">
                💳 付款
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
/* 📜 訂單列表 - Claymorphism */

h2 {
  font-family: 'Fredoka One', cursive !important;
  color: var(--text-dark);
  letter-spacing: 1px;
}

.clay-card {
  background: var(--bg-card);
  border-radius: 28px;
  box-shadow: 12px 12px 24px rgba(174, 160, 140, 0.2),
              -8px -8px 20px rgba(255, 255, 255, 0.8);
  border: none;
}

/* 空狀態 */
.clay-alert-info {
  background: var(--bg-card);
  border-radius: 28px;
  box-shadow: inset 4px 4px 10px rgba(174, 160, 140, 0.15),
              inset -4px -4px 10px rgba(255, 255, 255, 0.8);
}

/* 表格樣式 */
.table thead th {
  font-family: 'Fredoka One', cursive;
  color: var(--text-dark);
  border-bottom: 2px solid var(--bg-soft-pink) !important;
  background-color: transparent !important;
}

.table tbody tr {
  transition: background-color 0.2s ease;
}

.table tbody tr:hover {
  background-color: var(--bg-soft-mint) !important;
}

.text-danger {
  color: var(--coral) !important;
  font-family: 'Fredoka One', cursive;
}

.badge.bg-success {
  background-color: var(--mint) !important;
  color: #fff !important;
  box-shadow: 0 2px 5px rgba(78, 205, 196, 0.4);
}

.badge.bg-secondary {
  background-color: var(--text-muted) !important;
}

/* 按鈕 */
.btn-primary.btn-sm {
  font-family: 'Nunito', sans-serif;
  font-weight: 700;
  background: linear-gradient(135deg, var(--mint-dark) 0%, var(--mint) 100%) !important;
  border: none;
  padding: 6px 16px;
}

.btn-primary.btn-sm:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 10px rgba(78, 205, 196, 0.4);
}

.btn-outline-dark.btn-sm {
  border: 2px solid var(--text-muted);
  color: var(--text-muted);
  font-weight: 700;
  padding: 4px 14px;
}

.btn-outline-dark.btn-sm:hover {
  background-color: var(--text-muted);
  color: #fff;
}
</style>