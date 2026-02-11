<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { formatPrice } from '../utils/format.js';

const orders = ref([]);
const loading = ref(true);
const selectedOrder = ref(null); // 用來存詳情

// 抓取所有訂單
const fetchAllOrders = async () => {
  try {
    const response = await axios.get(`${import.meta.env.VITE_API_BASE_URL}/api/AdminOrder`);
    orders.value = response.data;
  } catch (error) {
    console.error("無法取得訂單列表", error);
    alert("讀取訂單失敗，請確認權限");
  } finally {
    loading.value = false;
  }
};

// 抓取單筆詳情
const showDetail = async (orderId) => {
  try {
    const response = await axios.get(`${import.meta.env.VITE_API_BASE_URL}/api/AdminOrder/${orderId}`);
    selectedOrder.value = response.data;
  } catch (error) {
    alert("無法取得訂單詳情");
  }
};

const closeDetail = () => {
    selectedOrder.value = null;
};

onMounted(() => {
  fetchAllOrders();
});
</script>

<template>
  <div class="container" style="margin-top: 100px; margin-bottom: 100px;">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="fw-bold text-dark"><i class="bi bi-card-checklist me-2"></i>訂單管理 (後台)</h2>
        <button class="btn btn-outline-secondary btn-sm rounded-pill px-3" @click="fetchAllOrders">
          <i class="bi bi-arrow-clockwise me-1"></i>重新整理
        </button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
      <p class="mt-2 text-muted">正在載入訂單資料...</p>
    </div>

    <div v-else-if="orders.length === 0" class="text-center py-5 clay-card empty-state">
      <h4 class="text-muted">目前尚無任何訂單。</h4>
    </div>

    <div v-else class="row">
      <!-- 訂單列表 -->
      <div :class="selectedOrder ? 'col-lg-7' : 'col-12'">
        <div class="card border-0 clay-card overflow-hidden">
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr>
                  <th class="ps-4">編號</th>
                  <th>顧客資訊</th>
                  <th>訂單日期</th>
                  <th>總金額</th>
                  <th>狀態</th>
                  <th class="text-center">功能</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="order in orders" :key="order.id" :class="{'table-active-row': selectedOrder?.id === order.id}">
                  <td class="ps-4 fw-bold text-dark">#{{ order.id }}</td>
                  <td>
                    <div class="fw-bold text-dark">{{ order.userFullName || '未填寫姓名' }}</div>
                    <small class="text-muted">{{ order.userEmail }}</small>
                  </td>
                  <td class="text-muted">{{ new Date(order.createdAt).toLocaleString() }}</td>
                  <td class="fw-bold text-danger">NT$ {{ formatPrice(order.totalAmount) }}</td>
                  <td>
                    <span class="badge rounded-pill px-3" :class="order.status === 'Paid' ? 'bg-success' : 'bg-warning text-dark'">
                        {{ order.status === 'Paid' ? '已付款' : '未付款' }}
                    </span>
                  </td>
                  <td class="text-center">
                    <button class="btn btn-sm btn-primary rounded-pill px-3 shadow-sm" @click="showDetail(order.id)">
                      詳情
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>

      <!-- 訂單詳情 (右側顯示) -->
      <div v-if="selectedOrder" class="col-lg-5 mt-4 mt-lg-0">
        <div class="card border-0 clay-card shadow sticky-top text-dark" style="top: 100px;">
          <div class="card-header bg-gradient-coral text-white d-flex justify-content-between align-items-center p-3 border-0">
            <h5 class="mb-0 fw-bold">📜 訂單 #{{ selectedOrder.id }} 詳情</h5>
            <button type="button" class="btn-close btn-close-white" @click="closeDetail"></button>
          </div>
          <div class="card-body p-4">
            <div class="mb-4">
                <h6 class="fw-bold text-muted border-bottom pb-2 mb-3">📦 購買項目</h6>
                <div v-for="item in selectedOrder.details" :key="item.id" class="d-flex align-items-center mb-3 p-2 rounded-3 hover-bg-light">
                    <img :src="item.imageUrl ? `/images/${item.imageUrl}` : 'https://placehold.co/50'" 
                         class="rounded-3 me-3 shadow-sm" style="width: 50px; height: 50px; object-fit: cover;">
                    <div class="flex-grow-1">
                        <div class="fw-bold text-dark">{{ item.productTitle }}</div>
                        <small class="text-muted">NT$ {{ formatPrice(item.price) }} x {{ item.quantity }}</small>
                    </div>
                    <div class="fw-bold text-danger">NT$ {{ formatPrice(item.price * item.quantity) }}</div>
                </div>
            </div>

            <div class="clay-summary p-3 rounded-3 mb-3">
                <div class="d-flex justify-content-between mb-2">
                    <span class="text-muted">商品小計：</span>
                    <span class="fw-bold">NT$ {{ formatPrice(selectedOrder.totalAmount) }}</span>
                </div>
                <div class="d-flex justify-content-between fw-bold fs-5 text-danger pt-2 border-top border-white">
                    <span>實付總額：</span>
                    <span>NT$ {{ formatPrice(selectedOrder.totalAmount) }}</span>
                </div>
            </div>
            
            <div class="mt-4">
                <button class="btn btn-outline-dark w-100 rounded-pill" @click="closeDetail">關閉視窗</button>
            </div>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<style scoped>
/* 🐾 後台訂單管理 - Claymorphism */

.clay-card {
  background: var(--bg-card);
  border-radius: 28px;
  box-shadow: 12px 12px 24px rgba(174, 160, 140, 0.2),
              -8px -8px 20px rgba(255, 255, 255, 0.8);
}

h2 {
  font-family: 'Fredoka One', cursive !important;
}

/* 表格 */
.table thead th {
  font-family: 'Fredoka One', cursive;
  color: var(--text-dark);
  background-color: var(--bg-soft-pink) !important;
  border: none;
}

.table-active-row {
  background-color: var(--bg-soft-mint) !important;
  box-shadow: inset 2px 0 0 var(--mint);
}

.text-danger {
  color: var(--coral) !important;
  font-family: 'Fredoka One', cursive;
}

.badge.bg-success {
  background-color: var(--mint) !important;
  color: #fff !important;
}

.badge.bg-warning {
  background-color: var(--sunny) !important;
  color: var(--text-dark) !important;
}

/* 詳情卡片 */
.bg-gradient-coral {
  background: linear-gradient(135deg, var(--coral) 0%, var(--coral-light) 100%);
  border-radius: 28px 28px 0 0;
}

.sticky-top {
  z-index: 10;
}

.clay-summary {
  background-color: var(--bg-cream);
  box-shadow: inset 2px 2px 5px rgba(174, 160, 140, 0.1),
              inset -2px -2px 5px rgba(255, 255, 255, 0.7);
}

.hover-bg-light:hover {
  background-color: rgba(0,0,0,0.02);
}

/* 按鈕 */
.btn-primary.btn-sm {
  background: linear-gradient(135deg, var(--mint-dark) 0%, var(--mint) 100%) !important;
  border: none;
}

.btn-primary.btn-sm:hover {
  background: linear-gradient(135deg, var(--mint) 0%, var(--mint-light) 100%) !important;
  transform: translateY(-2px);
}
</style>
