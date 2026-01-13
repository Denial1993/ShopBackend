<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router'; // 用來跳轉詳情頁

const orders = ref([]);
const router = useRouter();

// 一進來就抓訂單歷史
const fetchOrders = async () => {
  try {
    const response = await axios.get('http://localhost:5168/api/Order');
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
            <th>操作</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="order in orders" :key="order.id">
            <td>#{{ order.id }}</td>
            <td>{{ formatDate(order.createdAt) }}</td>
            <td class="fw-bold text-danger">NT$ {{ order.totalAmount }}</td>
            <td>
              <button @click="viewDetail(order.id)" class="btn btn-outline-primary btn-sm">
                查看詳情
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>