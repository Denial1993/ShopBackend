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
    const response = await axios.get(`http://localhost:5000/api/Order/${orderId}`);
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
    
    <div v-if="isLoading" class="text-center">載入中...</div>

    <div v-else-if="order">
      <div class="d-flex justify-content-between align-items-center mb-4">
        <h2 class="fw-bold">訂單詳情 #{{ order.id }}</h2>
        <router-link to="/orders" class="btn btn-outline-secondary">回列表</router-link>
      </div>

      <div class="card mb-4 border-0 shadow-sm bg-light">
        <div class="card-body">
            <p><strong>下單時間：</strong> {{ new Date(order.createdAt).toLocaleString() }}</p>
            <p class="mb-0"><strong>訂單總額：</strong> <span class="text-danger fw-bold fs-4">NT$ {{ formatPrice(order.totalAmount) }}</span></p>
        </div>
      </div>

      <h4 class="mb-3">購買商品</h4>
      <table class="table table-bordered bg-white">
        <thead>
          <tr>
            <th>商品名稱</th>
            <th>單價</th>
            <th>數量</th>
            <th>小計</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="item in order.details" :key="item.id">
            <td>{{ item.productTitle }}</td>
            <td>{{ formatPrice(item.price) }}</td>
            <td>x {{ item.quantity }}</td>
            <td>{{ formatPrice(item.price * item.quantity) }}</td>
          </tr>
        </tbody>
      </table>
    </div>

  </div>
</template>