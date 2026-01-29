<script setup>
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router';
import { formatPrice } from '../utils/format.js'; // 👈 引入

const cartItems = ref([]);
const router = useRouter();
const checkout = async () => {
  if (cartItems.value.length === 0) {
    alert("購物車是空的，不能結帳喔！");
    return;
  }

  if (!confirm(`確定要結帳嗎？總金額 NT$ ${totalPrice.value}`)) return;

  try {
    // 呼叫你的結帳 API
    await axios.post(`${import.meta.env.VITE_API_BASE_URL}/api/Order/checkout`);
    
    alert("🎉 結帳成功！感謝您的購買！");
    
    // 結帳完購物車應該清空了，我們跳轉去「我的訂單」頁面看結果
    router.push('/orders'); 

  } catch (error) {
    console.error(error);
    alert("結帳失敗，請稍後再試");
  }
};

// 計算總金額 (Vue 的 computed 超好用，資料變了自動重算)
const totalPrice = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + (item.price * item.quantity), 0);
});

// 抓取購物車資料
const fetchCart = async () => {
  try {
    const response = await axios.get(`${import.meta.env.VITE_API_BASE_URL}/api/Cart`);
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
    await axios.delete(`${import.meta.env.VITE_API_BASE_URL}/api/Cart/item/${itemId}`);
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
                    <img :src="item.imageUrl ? `/images/${item.imageUrl}` : 'https://placehold.co/100'" 
                         class="rounded-3 shadow-sm me-3"
                         style="width: 80px; height: 80px; object-fit: cover;">
                    <div class="d-flex flex-column">
                        <span class="fw-bold fs-5">{{ item.productTitle }}</span>
                    </div>
                </div>
              </td>
              <td>NT$ {{ formatPrice(item.price) }}</td>
              <td>
                x {{ item.quantity }}
              </td>
              <td class="fw-bold text-danger">NT$ {{ formatPrice(item.price * item.quantity) }}</td>
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
          <h4 class="fw-bold mb-0">總金額： <span class="text-danger">NT$ {{ formatPrice(totalPrice) }}</span></h4>

          
          <button @click="checkout" class="btn btn-dark btn-lg px-5">前往結帳</button>
        </div>
      </div>
    </div>

  </div>
</template>