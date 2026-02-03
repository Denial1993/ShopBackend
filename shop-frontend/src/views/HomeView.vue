<script setup>
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';
import { authStore } from '../store.js';
import { useRouter } from 'vue-router';
import { formatPrice } from '../utils/format.js';

const router = useRouter();
const products = ref([]);
const categories = ref([]);
const selectedCategory = ref(null); // null = 顯示全部
const isLoading = ref(true);

const fetchProducts = async () => {
    try {
        const response = await axios.get(`${import.meta.env.VITE_API_BASE_URL}/api/Product`);
        products.value = response.data;
    } catch (error) {
        console.error(error);
    } finally {
        isLoading.value = false;
    }
};

const fetchCategories = async () => {
    try {
        const response = await axios.get(`${import.meta.env.VITE_API_BASE_URL}/api/Category`);
        categories.value = response.data;
    } catch (error) {
        console.error(error);
    }
};

const filteredProducts = computed(() => {
    if (selectedCategory.value === null) {
        return products.value; // 顯示全部
    }
    return products.value.filter(p => p.categoryName === selectedCategory.value);
});

// 分頁功能
const currentPage = ref(1);
const itemsPerPage = ref(12);

// 總頁數
const totalPages = computed(() => {
    return Math.ceil(filteredProducts.value.length / itemsPerPage.value);
});

// 分頁後的商品
const paginatedProducts = computed(() => {
    const start = (currentPage.value - 1) * itemsPerPage.value;
    const end = start + itemsPerPage.value;
    return filteredProducts.value.slice(start, end);
});

// 換頁功能
const goToPage = (page) => {
    if (page < 1 || page > totalPages.value) return;
    currentPage.value = page;
    window.scrollTo({ top: 0, behavior: 'smooth' }); // 換頁後滾動到頂部
};

// 監聽分類變化，重置到第一頁
const selectCategory = (categoryName) => {
    selectedCategory.value = categoryName;
    currentPage.value = 1; // 切換分類時重置到第一頁
};

// 點擊商品卡片跳轉到詳細頁面
const goToProduct = (productId) => {
    router.push(`/product/${productId}`);
};

// 加入購物車功能
const addToCart = async (productId, event) => {
    // 阻止事件冒泡，避免觸發卡片的點擊事件
    if (event) {
        event.stopPropagation();
    }
    
    if (!authStore.isLoggedIn) {
        if (confirm("請先登入會員才能購物，要前往登入頁嗎？")) {
            router.push('/login');
        }
        return;
    }

    try {
        await axios.post(`${import.meta.env.VITE_API_BASE_URL}/api/Cart`, {
            productId: productId,
            quantity: 1
        });
        alert("✅ 已加入購物車！");
    } catch (error) {
        console.error(error);
        alert("❌ 加入失敗 (可能已經在車內，或庫存不足)");
    }
};

onMounted(() => {
    fetchProducts();
    fetchCategories();
});
</script>

<template>
    <div class="container-fluid" style="margin-top: 80px;">
        <div class="row">
            <!-- 左側分類選單 -->
            <div class="col-md-3 col-lg-2">
                <div class="category-sidebar">
                    <h4 class="mb-3 fw-bold">商品分類</h4>
                    
                    <!-- 全部商品 -->
                    <button 
                        @click="selectCategory(null)" 
                        :class="['category-btn', { 'active': selectedCategory === null }]"
                    >
                        <i class="bi bi-grid-3x3-gap-fill me-2"></i>
                        全部商品
                    </button>

                    <!-- 各個分類 -->
                    <button 
                        v-for="cat in categories" 
                        :key="cat.id"
                        @click="selectCategory(cat.name)" 
                        :class="['category-btn', { 'active': selectedCategory === cat.name }]"
                    >
                        <i class="bi bi-tag-fill me-2"></i>
                        {{ cat.name }}
                    </button>
                </div>
            </div>

            <!-- 右側商品區域 -->
            <div class="col-md-9 col-lg-10">
                <h2 class="text-center my-4">
                    {{ selectedCategory || '熱銷商品' }}
                </h2>

                <div v-if="isLoading" class="text-center mt-5">
                    <div class="spinner-border text-primary" role="status"></div>
                    <p class="mt-2 text-muted">商品載入中...</p>
                </div>

                <div v-else class="row">
                    <div class="col-md-4 col-lg-3 mb-4" v-for="item in paginatedProducts" :key="item.id">
                        <div class="card h-100 shadow-sm border-0 product-card" 
                             @click="goToProduct(item.id)"
                             style="cursor: pointer;">
                            <div class="position-relative overflow-hidden" style="height: 200px;">
                                <img :src="item.imageUrl ? `/images/${item.imageUrl}` : 'https://placehold.co/600x400?text=No+Image'"
                                    class="card-img-top w-100 h-100" style="object-fit: contain; padding: 10px;"
                                    alt="Product Image">
                            </div>

                            <div class="card-body d-flex flex-column text-center">
                                <h5 class="card-title fs-6">{{ item.title }}</h5>
                                <p class="text-muted small mb-2">{{ item.categoryName }}</p>
                                <h5 class="fw-bold text-danger mt-auto">NT$ {{ formatPrice(item.price) }}</h5>

                                <button @click="addToCart(item.id, $event)" class="btn btn-outline-dark w-100 mt-2 rounded-0">
                                    加入購物車
                                </button>
                            </div>
                        </div>
                    </div>

                    <!-- 沒有商品時的提示 -->
                    <div v-if="paginatedProducts.length === 0 && filteredProducts.length === 0" class="col-12 text-center mt-5">
                        <i class="bi bi-inbox" style="font-size: 4rem; color: #ccc;"></i>
                        <p class="text-muted mt-3">此分類目前沒有商品</p>
                    </div>
                </div>

                <!-- 分頁導航 -->
                <nav v-if="totalPages > 1" class="mt-4" aria-label="商品分頁">
                    <ul class="pagination justify-content-center">
                        <!-- 上一頁 -->
                        <li class="page-item" :class="{ disabled: currentPage === 1 }">
                            <button class="page-link" @click="goToPage(currentPage - 1)" :disabled="currentPage === 1">
                                <i class="bi bi-chevron-left"></i> 上一頁
                            </button>
                        </li>

                        <!-- 頁碼 -->
                        <li v-for="page in totalPages" :key="page" 
                            class="page-item" 
                            :class="{ active: currentPage === page }">
                            <button class="page-link" @click="goToPage(page)">{{ page }}</button>
                        </li>

                        <!-- 下一頁 -->
                        <li class="page-item" :class="{ disabled: currentPage === totalPages }">
                            <button class="page-link" @click="goToPage(currentPage + 1)" :disabled="currentPage === totalPages">
                                下一頁 <i class="bi bi-chevron-right"></i>
                            </button>
                        </li>
                    </ul>
                </nav>
            </div>
        </div>
    </div>
</template>

<style scoped>
/* 🎮 遊戲平台首頁 - 霓虹風格 */

/* 商品卡片 - 3D 霓虹邊框 */
.product-card {
  background: var(--bg-dark-card) !important;
  border: 2px solid var(--neon-purple) !important;
  box-shadow: 0 0 15px rgba(124, 58, 237, 0.4),
              0 4px 10px rgba(0, 0, 0, 0.5) !important;
  transition: all 0.3s ease;
  position: relative;
  overflow: visible !important;
}

.product-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  border: 2px solid var(--neon-pink);
  opacity: 0;
  transition: opacity 0.3s ease;
  pointer-events: none;
}

.product-card:hover {
  border-color: var(--neon-pink) !important;
  transform: translateY(-10px) scale(1.02);
  box-shadow: 0 0 30px rgba(244, 63, 94, 0.8),
              0 0 60px rgba(124, 58, 237, 0.4),
              0 15px 40px rgba(0, 0, 0, 0.6) !important;
}

.product-card:hover::before {
  opacity: 1;
  animation: neon-pulse 1.5s ease-in-out infinite;
}

/* 商品圖片容器 */
.product-card .position-relative {
  background: var(--bg-dark-lighter);
  border-bottom: 2px solid var(--neon-purple);
}

/* 商品卡片內容 */
.card-body {
  background: transparent;
  color: var(--text-primary) !important;
}

/* 商品標題 */
.card-title {
  font-family: 'Press Start 2P', cursive !important;
  font-size: 0.75rem !important;
  color: var(--neon-purple-light) !important;
  text-shadow: 0 0 5px var(--neon-purple-light);
  line-height: 1.6 !important;
  min-height: 48px;
}

/* 分類標籤 */
.text-muted {
  font-family: 'VT323', monospace !important;
  color: var(--text-secondary) !important;
  font-size: 1rem !important;
}

/* 價格 */
.text-danger {
  color: var(--neon-pink) !important;
  text-shadow: 0 0 10px var(--neon-pink),
               0 0 20px var(--neon-pink);
  font-family: 'Press Start 2P', cursive !important;
  font-size: 1rem !important;
}

/* 加入購物車按鈕 */
.card-body .btn-outline-dark {
  font-family: 'VT323', monospace !important;
  font-size: 1.1rem !important;
  background: transparent !important;
  border: 2px solid var(--neon-purple-light) !important;
  color: var(--neon-purple-light) !important;
  border-radius: 0 !important;
  transition: all 0.3s ease;
  padding: 8px 16px !important;
}

.card-body .btn-outline-dark:hover {
  background: var(--neon-purple-light) !important;
  color: var(--bg-dark) !important;
  box-shadow: 0 0 15px var(--neon-purple-light);
  transform: scale(1.05);
}

/* 分類側邊欄 - 霓虹邊框 */
.category-sidebar {
  position: sticky;
  top: 100px;
  padding: 20px;
  background: var(--bg-dark-card) !important;
  border: 2px solid var(--neon-purple);
  border-radius: 8px;
  box-shadow: 0 0 20px rgba(124, 58, 237, 0.4),
              0 4px 10px rgba(0, 0, 0, 0.5);
}

.category-sidebar h4 {
  font-family: 'Press Start 2P', cursive !important;
  color: var(--neon-pink) !important;
  text-shadow: 0 0 10px var(--neon-pink);
  font-size: 0.9rem !important;
  border-bottom: 2px solid var(--neon-pink) !important;
  padding-bottom: 15px;
  letter-spacing: 2px;
}

/* 分類按鈕 - 霓虹效果 */
.category-btn {
  display: block;
  width: 100%;
  text-align: left;
  padding: 12px 16px;
  margin-bottom: 10px;
  border: 2px solid var(--neon-purple-light);
  background: transparent !important;
  color: var(--neon-purple-light) !important;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s ease;
  font-family: 'VT323', monospace;
  font-size: 1.1rem;
  box-shadow: 0 0 5px rgba(167, 139, 250, 0.2);
}

.category-btn:hover {
  background: var(--bg-dark-lighter) !important;
  color: var(--neon-cyan) !important;
  border-color: var(--neon-cyan);
  box-shadow: 0 0 15px rgba(6, 182, 212, 0.5);
  transform: translateX(8px) scale(1.02);
}

.category-btn.active {
  background: linear-gradient(135deg, var(--neon-purple) 0%, var(--neon-pink) 100%) !important;
  color: var(--bg-dark) !important;
  font-weight: 700;
  border-color: var(--neon-pink);
  box-shadow: 0 0 20px rgba(244, 63, 94, 0.6),
              0 4px 15px rgba(124, 58, 237, 0.4);
  text-shadow: none;
}

.category-btn i {
  font-size: 1.2rem;
  margin-right: 8px;
  filter: drop-shadow(0 0 3px currentColor);
}

/* 頁面標題 */
h2.text-center {
  font-family: 'Press Start 2P', cursive !important;
  color: var(--neon-purple) !important;
  text-shadow: 0 0 10px var(--neon-purple),
               0 0 20px var(--neon-purple),
               0 0 40px var(--neon-purple);
  font-size: 1.5rem !important;
  letter-spacing: 3px;
  margin: 2rem 0 !important;
}

/* 載入中提示 */
.spinner-border {
  border-color: var(--neon-purple) !important;
  border-right-color: transparent !important;
  box-shadow: 0 0 10px var(--neon-purple);
}

.text-muted.mt-2 {
  color: var(--text-secondary) !important;
  font-family: 'VT323', monospace;
  font-size: 1.2rem;
}

/* 空狀態圖示 */
.bi-inbox {
  filter: drop-shadow(0 0 10px var(--neon-purple));
  color: var(--neon-purple) !important;
}

/* 分頁按鈕已經在全域 CSS 中定義，這裡不需要額外樣式 */

/* 響應式調整 */
@media (max-width: 768px) {
  .category-sidebar {
    position: relative;
    top: auto;
    margin-bottom: 20px;
  }
  
  h2.text-center {
    font-size: 1.2rem !important;
  }
  
  .card-title {
    font-size: 0.65rem !important;
    min-height: auto;
  }
}
</style>