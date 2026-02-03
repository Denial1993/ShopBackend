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
        <div class="card border-0 shadow-sm p-3">
            <img :src="product.imageUrl ? `/images/${product.imageUrl}` : 'https://placehold.co/600x600?text=No+Image'" 
                 class="img-fluid rounded" 
                 style="object-fit: contain; max-height: 500px;">
        </div>
      </div>

      <div class="col-md-6">
        <div class="ps-md-4">
            <span class="badge bg-secondary mb-2">{{ product.categoryName || '熱銷商品' }}</span>
            
            <h1 class="fw-bold mb-3">{{ product.title }}</h1>
            
            <h2 class="text-danger fw-bold mb-4">NT$ {{ formatPrice(product.price) }}</h2>
            
            <p class="text-muted mb-4" style="line-height: 1.8;">
                {{ product.description || '這個賣家很懶，沒有寫詳細介紹...' }}
            </p>

            <hr class="my-4">

            <div class="d-grid gap-2 d-md-block">
                <button @click="addToCart" class="btn btn-dark btn-lg px-5 me-md-2">
                    加入購物車
                </button>
                <router-link to="/" class="btn btn-outline-secondary btn-lg">
                    回首頁
                </router-link>
            </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* 🎮 商品詳情頁 - 遊戲霓虹風格 */

/* 商品圖片卡片 - 霓虹邊框 */
.card.border-0.shadow-sm {
  background: var(--bg-dark-card) !important;
  border: 3px solid var(--neon-purple) !important;
  box-shadow: 0 0 25px rgba(124, 58, 237, 0.6),
              0 10px 40px rgba(0, 0, 0, 0.5) !important;
  transition: all 0.3s ease;
}

.card.border-0.shadow-sm:hover {
  border-color: var(--neon-pink) !important;
  box-shadow: 0 0 40px rgba(244, 63, 94, 0.8),
              0 15px 50px rgba(0, 0, 0, 0.6) !important;
}

/* 分類標籤 */
.badge.bg-secondary {
  font-family: 'Press Start 2P', cursive !important;
  font-size: 0.6rem !important;
  background: var(--neon-purple) !important;
  color: var(--bg-dark);
  border: 1px solid var(--neon-purple);
  box-shadow: 0 0 10px var(--neon-purple);
  padding: 6px 12px;
}

/* 商品標題 */
h1.fw-bold {
  font-family: 'Press Start 2P', cursive !important;
  color: var(--neon-purple) !important;
  text-shadow: 0 0 10px var(--neon-purple),
               0 0 20px var(--neon-purple),
               0 0 40px var(--neon-purple);
  font-size: 1.8rem !important;
  line-height: 1.6;
}

/* 價格 */
h2.text-danger {
  color: var(--neon-pink) !important;
  text-shadow: 0 0 15px var(--neon-pink),
               0 0 30px var(--neon-pink);
  font-family: 'Press Start 2P', cursive !important;
  font-size: 1.5rem !important;
}

/* 商品描述 */
.text-muted {
  font-family: 'VT323', monospace !important;
  color: var(--text-secondary) !important;
  font-size: 1.2rem !important;
  line-height: 1.8;
}

/* 分隔線 */
hr {
  border-color: var(--neon-purple) !important;
  opacity: 0.5;
}

/* 加入購物車按鈕 */
.btn-dark.btn-lg {
  font-family: 'Press Start 2P', cursive !important;
  font-size: 0.8rem !important;
  background: transparent !important;
  border: 3px solid var(--neon-pink) !important;
  color: var(--neon-pink) !important;
  text-shadow: 0 0 10px var(--neon-pink);
  box-shadow: 0 0 20px rgba(244, 63, 94, 0.6);
  transition: all 0.3s ease;
  padding: 15px 40px !important;
}

.btn-dark.btn-lg:hover {
  background: var(--neon-pink) !important;
  color: var(--bg-dark) !important;
  text-shadow: none;
  box-shadow: 0 0 40px var(--neon-pink),
              0 0 80px var(--neon-pink);
  transform: scale(1.05);
}

/* 回首頁按鈕 */
.btn-outline-secondary.btn-lg {
  font-family: 'VT323', monospace !important;
  font-size: 1rem !important;
  background: transparent !important;
  border: 2px solid var(--neon-purple-light) !important;
  color: var(--neon-purple-light) !important;
  transition: all 0.3s ease;
}

.btn-outline-secondary.btn-lg:hover {
  background: var(--neon-purple-light) !important;
  color: var(--bg-dark) !important;
  box-shadow: 0 0 15px var(--neon-purple-light);
}

/* 載入中 */
.spinner-border {
  box-shadow: 0 0 15px var(--neon-purple);
}

/* 響應式調整 */
@media (max-width: 768px) {
  h1.fw-bold {
    font-size: 1.2rem !important;
  }
  
  h2.text-danger {
    font-size: 1.2rem !important;
  }
}
</style>