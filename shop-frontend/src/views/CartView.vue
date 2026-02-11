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
    <h2 class="mb-4 fw-bold">🛒 我的購物車</h2>

    <div v-if="cartItems.length === 0" class="text-center py-5 empty-cart-box">
      <h4 class="text-muted">購物車還是空的喔！</h4>
      <p class="text-muted small">快去挑選喜歡的美容服務或商品吧～</p>
      <router-link to="/" class="btn btn-primary mt-3 rounded-pill px-4">前往逛逛</router-link>
    </div>

    <div v-else>
      <div class="table-responsive clay-table-container p-4 mb-4">
        <table class="table align-middle border-0 mb-0">
          <thead>
            <tr>
              <th scope="col" class="border-0 text-muted">商品資訊</th>
              <th scope="col" class="border-0 text-muted">單價</th>
              <th scope="col" class="border-0 text-muted">數量</th>
              <th scope="col" class="border-0 text-muted">小計</th>
              <th scope="col" class="border-0 text-muted">操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="item in cartItems" :key="item.id">
              <td class="border-bottom-0 py-3">
                <div class="d-flex align-items-center">
                    <img :src="item.imageUrl ? `/images/${item.imageUrl}` : 'https://placehold.co/100'" 
                         class="rounded-4 shadow-sm me-3 clay-img"
                         style="width: 80px; height: 80px; object-fit: cover;">
                    <div class="d-flex flex-column">
                        <span class="fw-bold fs-5 text-dark">{{ item.productTitle }}</span>
                    </div>
                </div>
              </td>
              <td class="border-bottom-0 text-secondary fw-bold">NT$ {{ formatPrice(item.price) }}</td>
              <td class="border-bottom-0">
                <span class="badge bg-light text-dark fs-6 px-3 py-2 rounded-pill shadow-sm">x {{ item.quantity }}</span>
              </td>
              <td class="border-bottom-0 fw-bold text-danger fs-5">NT$ {{ formatPrice(item.price * item.quantity) }}</td>
              <td class="border-bottom-0">
                <button @click="removeItem(item.id)" class="btn btn-sm btn-outline-danger btn-remove rounded-circle shadow-sm">
                  <span class="fs-6">×</span>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div class="card border-0 mt-4 clay-card">
        <div class="card-body d-flex justify-content-between align-items-center p-4">
          <h4 class="fw-bold mb-0 text-dark">總金額： <span class="text-danger ms-2">NT$ {{ formatPrice(totalPrice) }}</span></h4>
          
          <button @click="checkout" class="btn btn-primary btn-lg px-5 rounded-pill shadow-lg hover-bounce">
            前往結帳 💳
          </button>
        </div>
      </div>
    </div>

  </div>
</template>

<style scoped>
/* 🐾 購物車頁面 - 寵物 Claymorphism 風格 */

/* 標題 */
h2.fw-bold {
  font-family: 'Fredoka One', cursive !important;
  color: var(--text-dark) !important;
  font-size: 2rem !important;
  letter-spacing: 1px;
}

/* 空購物車區塊 */
.empty-cart-box {
  background: var(--bg-card);
  border-radius: 28px;
  box-shadow: inset 4px 4px 10px rgba(174, 160, 140, 0.15),
              inset -4px -4px 10px rgba(255, 255, 255, 0.8); /* 內凹效果 */
  padding: 60px !important;
}

.text-muted {
  font-family: 'Nunito', sans-serif !important;
  color: var(--text-muted) !important;
}

/* 購物車表格容器 - Clay 卡片 */
.clay-table-container {
  background: var(--bg-card);
  border-radius: 28px;
  box-shadow: 12px 12px 24px rgba(174, 160, 140, 0.2),
              -8px -8px 20px rgba(255, 255, 255, 0.8);
}

/* 表格樣式調整 */
.table thead th {
  font-family: 'Nunito', sans-serif !important;
  font-weight: 700;
  font-size: 0.95rem;
  background-color: transparent !important;
}

.table tbody tr {
  background-color: transparent !important;
  box-shadow: none !important; /* 移除全域表格的 hover 效果 */
  border-bottom: 2px solid var(--bg-soft-pink) !important;
}

.table tbody tr:last-child {
  border-bottom: none !important;
}

.table tbody tr:hover {
  background-color: var(--bg-soft-mint) !important; /* 不一樣的 hover 色 */
  transform: none !important;
}

/* 商品圖片 - 圓潤陰影 */
.clay-img {
  border-radius: 16px !important;
  box-shadow: 4px 4px 10px rgba(174, 160, 140, 0.2) !important;
}

/* 商品名稱 */
.fs-5.text-dark {
  font-family: 'Fredoka One', cursive !important;
  color: var(--text-dark) !important;
  font-size: 1.1rem !important;
}

/* 單價 */
.text-secondary {
  color: var(--text-body) !important;
  font-family: 'Nunito', sans-serif !important;
}

/* 數量 Badge */
.badge.bg-light {
  background-color: var(--bg-cream) !important;
  color: var(--text-dark) !important;
  font-family: 'Nunito', sans-serif;
  border: 1px solid rgba(0,0,0,0.05);
}

/* 小計價格 */
.text-danger.fs-5 {
  color: var(--coral) !important;
  font-family: 'Fredoka One', cursive !important;
}

/* 移除按鈕 */
.btn-remove {
  width: 36px;
  height: 36px;
  padding: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid var(--bg-soft-pink) !important;
  color: var(--coral-light) !important;
  background: transparent !important;
  transition: all 0.3s ease;
}

.btn-remove:hover {
  background: var(--coral) !important;
  color: #fff !important;
  border-color: var(--coral) !important;
  transform: rotate(90deg);
}

/* 總金額卡片 - Clay 卡片 */
.clay-card {
  background: var(--bg-card) !important;
  border-radius: 28px !important;
  box-shadow: 12px 12px 24px rgba(174, 160, 140, 0.2),
              -8px -8px 20px rgba(255, 255, 255, 0.8) !important;
}

.card-body h4 {
  font-family: 'Fredoka One', cursive !important;
  font-size: 1.4rem !important;
}

/* 結帳按鈕 */
.btn-primary.btn-lg {
  font-family: 'Nunito', sans-serif !important;
  font-weight: 800 !important;
  font-size: 1.1rem !important;
  background: linear-gradient(135deg, var(--mint-dark) 0%, var(--mint) 100%) !important; /* 使用薄荷綠 */
  border: none !important;
  color: #FFFFFF !important;
  box-shadow: 0 6px 20px rgba(78, 205, 196, 0.4);
  transition: all 0.35s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.btn-primary.btn-lg:hover {
  background: linear-gradient(135deg, var(--mint) 0%, var(--mint-light) 100%) !important;
  box-shadow: 0 10px 30px rgba(78, 205, 196, 0.5);
  transform: translateY(-3px) scale(1.02);
}

/* 響應式調整 */
@media (max-width: 768px) {
  .card-body.d-flex {
    flex-direction: column;
    gap: 20px;
    text-align: center;
  }
  
  .btn-primary.btn-lg {
    width: 100%;
  }
}
</style>