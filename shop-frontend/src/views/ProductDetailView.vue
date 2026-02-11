<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { useRoute, useRouter } from 'vue-router'; // 👈用來抓網址上的 id
import { authStore } from '../store.js'; 
import { formatPrice } from '../utils/format.js'; // 價格千分位工具

const route = useRoute();
const router = useRouter();
const product = ref(null);
const isLoading = ref(true);

// 抓取單一商品資料
const fetchProduct = async () => {
  try {
    const productId = route.params.id; // 從網址 /product/5 抓出 5
    const response = await axios.get(`${import.meta.env.VITE_API_BASE_URL}/api/Product/${productId}`);
    product.value = response.data;
  } catch (error) {
    console.error(error);
    alert("找不到該商品");
    router.push('/'); // 找不到就回首頁
  } finally {
    isLoading.value = false;
  }
};

// 加入購物車 (跟首頁邏輯一樣)
const addToCart = async () => {
  if (!authStore.isLoggedIn) {
    if(confirm("請先登入會員才能購物，要前往登入頁嗎？")) {
      router.push('/login');
    }
    return;
  }

  try {
    await axios.post(`${import.meta.env.VITE_API_BASE_URL}/api/Cart`, {
      productId: product.value.id,
      quantity: 1 // 這裡你可以之後擴充「數量選擇器」
    });
    alert("✅ 已加入購物車！");
  } catch (error) {
    console.error(error);
    alert("❌ 加入失敗");
  }
};

onMounted(() => {
  fetchProduct();
});
</script>

<template>
  <div class="container" style="margin-top: 100px;">
    
    <div v-if="isLoading" class="text-center py-5">
        <div class="spinner-border text-primary" role="status"></div>
    </div>

    <div v-else-if="product" class="row">
      <div class="col-md-6 mb-4">
        <div class="card border-0 shadow-sm p-3 clay-card">
            <img :src="product.imageUrl ? `/images/${product.imageUrl}` : 'https://placehold.co/600x600?text=No+Image'" 
                 class="img-fluid rounded" 
                 style="object-fit: contain; max-height: 500px; border-radius: 20px;">
        </div>
      </div>

      <div class="col-md-6">
        <div class="ps-md-4">
            <span class="badge bg-secondary mb-3">{{ product.categoryName || '熱銷商品' }}</span>
            
            <h1 class="fw-bold mb-3">{{ product.title }}</h1>
            
            <h2 class="text-danger fw-bold mb-4">NT$ {{ formatPrice(product.price) }}</h2>
            
            <div class="description-box p-4 mb-4">
              <p class="text-muted mb-0" style="line-height: 1.8;">
                  {{ product.description || '這個賣家很懶，沒有寫詳細介紹...' }}
              </p>
            </div>

            <div class="d-grid gap-2 d-md-block mt-4">
                <button @click="addToCart" class="btn btn-primary btn-lg px-5 me-md-2 rounded-pill">
                    🛒 加入購物車
                </button>
                <router-link to="/" class="btn btn-outline-dark btn-lg rounded-pill">
                    回首頁
                </router-link>
            </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* 🐾 商品詳情頁 - 寵物 Claymorphism 風格 */

/* 商品圖片卡片 - Clay 效果 */
.clay-card {
  background: var(--bg-card) !important;
  border: none !important;
  border-radius: 28px !important;
  box-shadow: 12px 12px 24px rgba(174, 160, 140, 0.25),
              -8px -8px 20px rgba(255, 255, 255, 0.8) !important;
}

/* 分類標籤 */
.badge.bg-secondary {
  font-family: 'Nunito', sans-serif !important;
  font-weight: 700 !important;
  font-size: 0.85rem !important;
  background: var(--bg-soft-lavender) !important;
  color: var(--lavender) !important;
  padding: 8px 16px;
  border-radius: 50px;
}

/* 商品標題 */
h1.fw-bold {
  font-family: 'Fredoka One', cursive !important;
  color: var(--text-dark) !important;
  font-size: 2.2rem !important;
  letter-spacing: 0.5px;
}

/* 價格 */
h2.text-danger {
  color: var(--coral) !important;
  font-family: 'Fredoka One', cursive !important;
  font-size: 1.8rem !important;
}

/* 商品描述區域 - 內凹 Clay 效果 */
.description-box {
  background: var(--bg-cream);
  border-radius: 20px;
  box-shadow: inset 4px 4px 8px rgba(174, 160, 140, 0.15),
              inset -2px -2px 6px rgba(255, 255, 255, 0.7);
  border: 1px solid rgba(255, 255, 255, 0.5);
}

.text-muted {
  font-family: 'Nunito', sans-serif !important;
  color: var(--text-body) !important;
  font-size: 1.1rem !important;
}

/* 加入購物車按鈕 */
.btn-primary.btn-lg {
  font-family: 'Nunito', sans-serif !important;
  font-weight: 800 !important;
  font-size: 1.1rem !important;
  background: linear-gradient(135deg, var(--coral) 0%, var(--coral-light) 100%) !important;
  border: none !important;
  color: #FFFFFF !important;
  box-shadow: 0 6px 20px rgba(255, 107, 107, 0.35);
  transition: all 0.35s cubic-bezier(0.34, 1.56, 0.64, 1);
  padding: 14px 40px !important;
}

.btn-primary.btn-lg:hover {
  background: linear-gradient(135deg, var(--coral-dark) 0%, var(--coral) 100%) !important;
  box-shadow: 0 10px 30px rgba(255, 107, 107, 0.45);
  transform: translateY(-3px) scale(1.02);
}

/* 回首頁按鈕 */
.btn-outline-dark.btn-lg {
  font-family: 'Nunito', sans-serif !important;
  font-weight: 700 !important;
  font-size: 1rem !important;
  background: var(--bg-card) !important;
  border: 2px solid var(--text-muted) !important;
  color: var(--text-muted) !important;
  transition: all 0.3s ease;
  padding: 14px 30px !important;
}

.btn-outline-dark.btn-lg:hover {
  background: var(--text-muted) !important;
  color: #FFFFFF !important;
  transform: translateY(-2px);
}

/* 載入中 */
.spinner-border {
  border-color: var(--coral) !important;
  border-right-color: transparent !important;
}

/* 響應式調整 */
@media (max-width: 768px) {
  h1.fw-bold {
    font-size: 1.6rem !important;
  }
  
  h2.text-danger {
    font-size: 1.4rem !important;
  }
}
</style>