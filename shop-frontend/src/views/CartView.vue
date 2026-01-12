<script setup>
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router';

const cartItems = ref([]);
const router = useRouter();

// 計算總金額 (Vue 的 computed 超好用，資料變了自動重算)
const totalPrice = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + (item.price * item.quantity), 0);
});

// 抓取購物車資料
const fetchCart = async () => {
  try {
    const response = await axios.get('http://localhost:5168/api/Cart');
    cartItems.value = response.data.items || [];
  } catch (error) {
    console.error("無法取得購物車", error);
    // 如果是 401 (未登入)，踢回首頁
    if (error.response && error.response.status === 401) {
        alert("請先登入");
        router.push('/login');
    }
  }
};

// 移除商品
const removeItem = async (itemId) => {
  if(!confirm("確定要移除嗎？")) return;
  try {
    await axios.delete(`http://localhost:5168/api/Cart/item/${itemId}`);
    // 移除成功後，重新抓一次資料更新畫面
    fetchCart();
  } catch (error) {
    alert("移除失敗");
  }
};

onMounted(() => {
  fetchCart();
});
</script>

<template>
  <div class="container" style="margin-top: 100px;">
    <h2 class="mb-4 fw-bold">我的購物車</h2>

    <div v-if="cartItems.length === 0" class="text-center py-5 bg-light rounded-3">
      <h4 class="text-muted">購物車還是空的喔！</h4>
      <router-link to="/" class="btn btn-primary mt-3">去逛逛</router-link>
    </div>

    <div v-else>
      <div class="table-responsive">
        <table class="table align-middle">
          <thead class="table-light">
            <tr>
              <th>商品資訊</th>
              <th>單價</th>
              <th>數量</th>
              <th>小計</th>
              <th>操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in cartItems" :key="item.id">
              <td>
                <div class="d-flex align-items-center">
                    <img :src="item.imageUrl ? `/images/${item.imageUrl}` : 'https://placehold.co/50'" 
                         width="50" class="me-3">
                    <span class="fw-bold">{{ item.productTitle }}</span>
                </div>
              </td>
              <td>NT$ {{ item.price }}</td>
              <td>
                x {{ item.quantity }}
              </td>
              <td class="fw-bold text-danger">NT$ {{ item.price * item.quantity }}</td>
              <td>
                <button @click="removeItem(item.id)" class="btn btn-sm btn-outline-danger">
                  <span class="fs-6">×</span>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="card border-0 bg-light mt-4">
        <div class="card-body d-flex justify-content-between align-items-center">
          <h4 class="fw-bold mb-0">總金額： <span class="text-danger">NT$ {{ totalPrice }}</span></h4>
          <button class="btn btn-dark btn-lg px-5">前往結帳</button>
        </div>
      </div>
    </div>

  </div>
</template>