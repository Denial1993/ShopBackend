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

<style scoped>
/* 🎮 購物車頁面 - 遊戲霓虹風格 */

/* 主標題 */
h2 {
  font-family: 'Press Start 2P', cursive !important;
  color: var(--neon-purple) !important;
  text-shadow: var(--glow-purple);
  font-size: 2rem !important;
}

/* 空購物車卡片 */
.bg-light {
  background: var(--bg-dark-card) !important;
  border: 2px solid var(--neon-purple);
  box-shadow: 0 0 15px rgba(124, 58, 237, 0.4);
}

.bg-light h4 {
  font-family: 'VT323', monospace;
  color: var(--text-secondary) !important;
}

/* 表格樣式已在全域 CSS 定義，這裡添加特定調整 */
.table {
  border: 2px solid var(--neon-purple);
}

.table thead th {
  font-size: 1rem !important;
}

/* 商品圖片 - 霓虹邊框 */
.rounded-3.shadow-sm {
  border: 2px solid var(--neon-purple-light) !important;
  box-shadow: 0 0 10px rgba(167, 139, 250, 0.3) !important;
}

/* 商品名稱 */
.fw-bold.fs-5 {
  font-family: 'Press Start 2P', cursive !important;
  font-size: 1.1rem !important;
  color: var(--neon-purple-light) !important;
  text-shadow: 0 0 5px var(--neon-purple-light);
}

/* 價格文字 */
td {
  font-family: 'VT323', monospace;
  font-size: 1.4rem;
}

/* 小計價格 */
.fw-bold.text-danger {
  color: var(--neon-pink) !important;
  text-shadow: 0 0 10px var(--neon-pink);
  font-family: 'Press Start 2P', cursive !important;
  font-size: 1.2rem !important;
}

/* 移除按鈕 */
.btn-outline-danger {
  background: transparent !important;
  border: 2px solid var(--neon-pink) !important;
  color: var(--neon-pink) !important;
  transition: all 0.3s ease;
}

.btn-outline-danger:hover {
  background: var(--neon-pink) !important;
  color: var(--bg-dark) !important;
  box-shadow: 0 0 15px var(--neon-pink);
  transform: scale(1.1);
}

/* 總金額卡片 */
.card.border-0.bg-light {
  background: var(--bg-dark-card) !important;
  border: 3px solid var(--neon-pink) !important;
  box-shadow: 0 0 25px rgba(244, 63, 94, 0.6);
}

.card-body h4 {
  font-family: 'Press Start 2P', cursive !important;
  color: var(--neon-purple-light) !important;
  text-shadow: 0 0 10px var(--neon-purple-light);
  font-size: 1.6rem !important;
}

.card-body h4 .text-danger {
  color: var(--neon-pink) !important;
  text-shadow: 0 0 10px var(--neon-pink),
               0 0 20px var(--neon-pink);
}

/* 結帳按鈕 - 大型霓虹 CTA */
.btn-dark.btn-lg {
  font-family: 'Press Start 2P', cursive !important;
  font-size: 1.2rem !important;
  background: transparent !important;
  border: 3px solid var(--neon-pink) !important;
  color: var(--neon-pink) !important;
  text-shadow: 0 0 10px var(--neon-pink);
  box-shadow: 0 0 20px rgba(244, 63, 94, 0.6);
  padding: 15px 40px !important;
  transition: all 0.3s ease;
}

.btn-dark.btn-lg:hover {
  background: var(--neon-pink) !important;
  color: var(--bg-dark) !important;
  text-shadow: none;
  box-shadow: 0 0 40px var(--neon-pink),
              0 0 80px var(--neon-pink);
  transform: scale(1.1);
}

/* 去逛逛按鈕 */
.btn-primary.mt-3 {
  font-family: 'Press Start 2P', cursive !important;
  font-size: 0.8rem !important;
}
</style>