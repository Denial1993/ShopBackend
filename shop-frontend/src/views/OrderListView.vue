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
    <h2 class="mb-4 fw-bold">我的訂單歷史</h2>

    <div v-if="orders.length === 0" class="alert alert-info">
      目前還沒有訂單紀錄喔，快去買點東西吧！
    </div>

    <div v-else class="table-responsive">
      <table class="table table-hover align-middle shadow-sm bg-white rounded">
        <thead class="table-dark">
          <tr>
            <th>訂單編號</th>
            <th>下單日期</th>
            <th>總金額</th>
            <th>狀態</th>
            <th>操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="order in orders" :key="order.id">
            <td>#{{ order.id }}</td>
            <td>{{ formatDate(order.createdAt) }}</td>
            <td class="fw-bold text-danger">NT$ {{ formatPrice(order.totalAmount) }}</td>
            <td>
              <span v-if="order.status === 'Paid'" class="badge bg-success">已付款</span>
              <span v-else class="badge bg-secondary">未付款</span>
            </td>
            <td>
              <button @click="viewDetail(order.id)" class="btn btn-outline-primary btn-sm">
                查看詳情
              </button>
              <span v-if="order.status !== 'Paid'"> / </span>
              <button v-if="order.status !== 'Paid'" @click="handlePayment(order.id)" class="btn btn-success btn-sm">
                💳 前往付款
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>