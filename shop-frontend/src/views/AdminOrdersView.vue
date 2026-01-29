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
    const response = await axios.get('http://localhost:5000/api/AdminOrder');
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
    const response = await axios.get(`http://localhost:5000/api/AdminOrder/${orderId}`);
    selectedOrder.value = response.data;
    // 使用 Bootstrap Modal 的話可以這裡開，或者簡單用 v-if 顯示在下面
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
        <h2 class="fw-bold">訂單管理 (後台)</h2>
        <button class="btn btn-outline-secondary btn-sm" @click="fetchAllOrders">重新整理</button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <div class="spinner-border text-primary" role="status"></div>
      <p class="mt-2 text-muted">正在載入訂單資料...</p>
    </div>

    <div v-else-if="orders.length === 0" class="text-center py-5 bg-light rounded-3">
      <h4 class="text-muted">目前尚無任何訂單。</h4>
    </div>

    <div v-else class="row">
      <!-- 訂單列表 -->
      <div :class="selectedOrder ? 'col-lg-7' : 'col-12'">
        <div class="card border-0 shadow-sm">
          <div class="table-responsive">
            <table class="table table-hover align-middle mb-0">
              <thead class="table-light">
                <tr>
                  <th>編號</th>
                  <th>顧客資訊</th>
                  <th>訂單日期</th>
                  <th>總金額</th>
                  <th>狀態</th>
                  <th>功能</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="order in orders" :key="order.id" :class="{'table-primary bg-opacity-10': selectedOrder?.id === order.id}">
                  <td>#{{ order.id }}</td>
                  <td>
                    <div class="fw-bold">{{ order.userFullName || '未填寫姓名' }}</div>
                    <small class="text-muted">{{ order.userEmail }}</small>
                  </td>
                  <td>{{ new Date(order.createdAt).toLocaleString() }}</td>
                  <td class="fw-bold text-danger">NT$ {{ formatPrice(order.totalAmount) }}</td>
                  <td>
                    <span class="badge" :class="order.status === 'Paid' ? 'bg-success' : 'bg-warning text-dark'">
                        {{ order.status === 'Paid' ? '已付款' : '未付款' }}
                    </span>
                  </td>
                  <td>
                    <button class="btn btn-sm btn-primary" @click="showDetail(order.id)">
                      查看明細
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
        <div class="card border-0 shadow sticky-top" style="top: 100px;">
          <div class="card-header bg-dark text-white d-flex justify-content-between align-items-center">
            <h5 class="mb-0">訂單 #{{ selectedOrder.id }} 詳情</h5>
            <button type="button" class="btn-close btn-close-white" @click="closeDetail"></button>
          </div>
          <div class="card-body">
            <div class="mb-4">
                <h6 class="fw-bold text-secondary border-bottom pb-2">購買項目</h6>
                <div v-for="item in selectedOrder.details" :key="item.id" class="d-flex align-items-center mb-3">
                    <img :src="item.imageUrl ? `/images/${item.imageUrl}` : 'https://placehold.co/50'" 
                         class="rounded me-3" style="width: 50px; height: 50px; object-fit: cover;">
                    <div class="flex-grow-1">
                        <div class="fw-bold">{{ item.productTitle }}</div>
                        <small class="text-muted">NT$ {{ formatPrice(item.price) }} x {{ item.quantity }}</small>
                    </div>
                    <div class="fw-bold">NT$ {{ formatPrice(item.price * item.quantity) }}</div>
                </div>
            </div>

            <div class="bg-light p-3 rounded-3">
                <div class="d-flex justify-content-between mb-2">
                    <span>商品小計：</span>
                    <span>NT$ {{ formatPrice(selectedOrder.totalAmount) }}</span>
                </div>
                <div class="d-flex justify-content-between fw-bold fs-5 text-danger">
                    <span>實付總額：</span>
                    <span>NT$ {{ formatPrice(selectedOrder.totalAmount) }}</span>
                </div>
            </div>
            
            <div class="mt-4">
                <button class="btn btn-outline-dark w-100" @click="closeDetail">關閉視窗</button>
            </div>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<style scoped>
.sticky-top {
    z-index: 10;
}
.table-hover tbody tr {
    cursor: pointer;
}
</style>
