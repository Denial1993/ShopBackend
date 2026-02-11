<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { useRoute } from 'vue-router'; // 👈 用這個抓網址上的 ID
import { formatPrice } from '../utils/format.js'; // 👈 引入

const route = useRoute();
const order = ref(null); // 用來放訂單資料
const isLoading = ref(true);

const fetchOrderDetail = async () => {
  try {
    // 從網址抓 id (例如 /orders/5 -> id 就是 5)
    const orderId = route.params.id;
    const response = await axios.get(`${import.meta.env.VITE_API_BASE_URL}/api/Order/${orderId}`);
    order.value = response.data;
  } catch (error) {
    console.error("找不到訂單", error);
    alert("訂單不存在");
  } finally {
    isLoading.value = false;
  }
};

onMounted(() => {
  fetchOrderDetail();
});
</script>

<template>
  <div class="container" style="margin-top: 100px;">
    
    <div v-if="isLoading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="order">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="fw-bold mb-0 text-dark">📜 訂單詳情 #{{ order.id }}</h2>
        <router-link to="/orders" class="btn btn-outline-secondary rounded-pill px-4">
          <i class="bi bi-arrow-left"></i> 回列表
        </router-link>
      </div>

      <div class="card mb-5 border-0 clay-card shadow-sm">
        <div class="card-body p-4">
            <div class="row">
                <div class="col-md-6 mb-3">
                    <p class="text-muted mb-1">📅 下單時間</p>
                    <h5 class="fw-bold text-dark">{{ new Date(order.createdAt).toLocaleString() }}</h5>
                </div>
                <div class="col-md-6 mb-3">
                    <p class="text-muted mb-1">💰 訂單總額</p>
                    <h4 class="fw-bold text-danger">NT$ {{ formatPrice(order.totalAmount) }}</h4>
                </div>
            </div>
        </div>
      </div>

      <h4 class="mb-4 text-dark fw-bold">📦 購買商品</h4>
      
      <div class="clay-table-container p-4">
        <table class="table align-middle mb-0">
          <thead>
            <tr class="text-muted">
              <th scope="col" class="border-0">商品資訊</th>
              <th scope="col" class="border-0">單價</th>
              <th scope="col" class="border-0">數量</th>
              <th scope="col" class="border-0">小計</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in order.details" :key="item.id">
              <td class="border-bottom-0 py-3">
                <div class="d-flex align-items-center">
                  <img :src="item.imageUrl ? `/images/${item.imageUrl}` : 'https://placehold.co/80'" 
                       class="rounded-3 shadow-sm me-3 clay-img" 
                       style="width: 60px; height: 60px; object-fit: cover;">
                  <span class="fw-bold text-dark">{{ item.productTitle }}</span>
                </div>
              </td>
              <td class="border-bottom-0 text-muted">NT$ {{ formatPrice(item.price) }}</td>
              <td class="border-bottom-0">
                <span class="badge bg-light text-dark fs-6 px-3 py-1 rounded-pill">x {{ item.quantity }}</span>
              </td>
              <td class="border-bottom-0 fw-bold text-danger">NT$ {{ formatPrice(item.price * item.quantity) }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

  </div>
</template>

<style scoped>
/* 📜 訂單詳情 - Clay 質感 */

h2, h4 {
  font-family: 'Fredoka One', cursive !important;
  letter-spacing: 0.5px;
}

.clay-card {
  background: var(--bg-card);
  border-radius: 28px;
  box-shadow: 12px 12px 24px rgba(174, 160, 140, 0.2),
              -8px -8px 20px rgba(255, 255, 255, 0.8);
}

.text-muted {
  font-family: 'Nunito', sans-serif !important;
  color: var(--text-muted) !important;
}

.text-danger {
  color: var(--coral) !important;
  font-family: 'Fredoka One', cursive !important;
}

/* 商品表格 */
.clay-table-container {
  background: var(--bg-card);
  border-radius: 28px;
  box-shadow: inset 4px 4px 10px rgba(174, 160, 140, 0.15),
              inset -4px -4px 10px rgba(255, 255, 255, 0.8); /* 內凹效果 */
}

.table thead th {
  font-family: 'Nunito', sans-serif;
  font-weight: 700;
  color: var(--text-muted);
}

.table tbody tr {
  border-bottom: 1px solid var(--bg-soft-pink) !important;
}

.table tbody tr:last-child {
  border-bottom: none !important;
}

.clay-img {
  width: 60px;
  height: 60px;
  border-radius: 12px;
  box-shadow: 2px 2px 5px rgba(174, 160, 140, 0.2);
}

/* 按鈕 */
.btn-outline-secondary {
  border: 2px solid var(--text-muted);
  color: var(--text-muted);
  font-weight: 700;
  font-family: 'Nunito', sans-serif;
}

.btn-outline-secondary:hover {
  background-color: var(--text-muted);
  color: #fff;
}
</style>