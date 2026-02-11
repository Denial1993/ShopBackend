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

const getImageUrl = (url) => {
    if (!url) return 'https://placehold.co/600x400?text=No+Image';
    if (url.startsWith('http') || url.startsWith('data:')) return url;
    return `/images/${url}`;
};
</script>

<template>
    <div class="container-fluid" style="margin-top: 80px;">
        <div class="row">
            <!-- 左側分類選單 -->
            <div class="col-md-3 col-lg-2">
                <div class="category-sidebar">
                    <h4 class="mb-3 fw-bold">🐾 服務分類</h4>
                    
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
                    {{ selectedCategory || '🐶 熱銷商品' }}
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
                                <img :src="getImageUrl(item.imageUrl)"
                                    class="card-img-top w-100 h-100" style="object-fit: contain; padding: 10px;"
                                    alt="Product Image">
                            </div>

                            <div class="card-body d-flex flex-column text-center">
                                <h5 class="card-title fs-6">{{ item.title }}</h5>
                                <p class="text-muted small mb-2">{{ item.categoryName }}</p>
                                <h5 class="fw-bold text-danger mt-auto">NT$ {{ formatPrice(item.price) }}</h5>

                                <button @click="addToCart(item.id, $event)" class="btn btn-outline-dark w-100 mt-2 rounded-0">
                                    🛒 加入購物車
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
/* 🐾 寵物美容首頁 - Claymorphism 風格 */

/* 商品卡片 - Claymorphism 3D 黏土效果 */
.product-card {
  background: var(--bg-card) !important;
  border: none !important;
  border-radius: var(--border-radius) !important;
  box-shadow: var(--clay-shadow) !important;
  transition: all 0.35s cubic-bezier(0.34, 1.56, 0.64, 1);
  position: relative;
  overflow: hidden !important;
}

.product-card:hover {
  transform: translateY(-8px) scale(1.02);
  box-shadow: var(--clay-shadow-hover) !important;
}

/* 商品圖片容器 */
.product-card .position-relative {
  background: var(--bg-cream);
  border-bottom: 2px solid var(--bg-soft-pink);
  border-radius: var(--border-radius) var(--border-radius) 0 0;
}

/* 商品卡片內容 */
.card-body {
  background: transparent;
  color: var(--text-body) !important;
  padding: 16px !important;
}

/* 商品標題 */
.card-title {
  font-family: 'Fredoka One', cursive !important;
  font-size: 0.95rem !important;
  color: var(--text-dark) !important;
  line-height: 1.4 !important;
  min-height: 42px;
}

/* 分類標籤 */
.text-muted {
  font-family: 'Nunito', sans-serif !important;
  color: var(--text-muted) !important;
  font-size: 0.85rem !important;
}

/* 價格 */
.text-danger {
  color: var(--coral) !important;
  font-family: 'Fredoka One', cursive !important;
  font-size: 1.1rem !important;
}

/* 加入購物車按鈕 */
.card-body .btn-outline-dark {
  font-family: 'Nunito', sans-serif !important;
  font-weight: 700 !important;
  font-size: 0.9rem !important;
  background: var(--bg-card) !important;
  border: 2px solid var(--coral) !important;
  color: var(--coral) !important;
  border-radius: var(--border-radius-pill) !important;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  padding: 8px 16px !important;
}

.card-body .btn-outline-dark:hover {
  background: linear-gradient(135deg, var(--coral) 0%, var(--coral-light) 100%) !important;
  color: #FFFFFF !important;
  border-color: var(--coral) !important;
  transform: scale(1.03);
  box-shadow: 0 4px 15px rgba(255, 107, 107, 0.35);
}

/* 分類側邊欄 - Claymorphism */
.category-sidebar {
  position: sticky;
  top: 100px;
  padding: 24px;
  background: var(--bg-card) !important;
  border: none;
  border-radius: var(--border-radius);
  box-shadow: var(--clay-shadow);
}

.category-sidebar h4 {
  font-family: 'Fredoka One', cursive !important;
  color: var(--coral) !important;
  font-size: 1.1rem !important;
  border-bottom: 3px solid var(--bg-soft-pink) !important;
  padding-bottom: 15px;
  letter-spacing: 1px;
}

/* 分類按鈕 - 圓潤可愛 */
.category-btn {
  display: block;
  width: 100%;
  text-align: left;
  padding: 12px 16px;
  margin-bottom: 8px;
  border: 2px solid transparent;
  background: var(--bg-cream) !important;
  color: var(--text-body) !important;
  border-radius: 14px;
  cursor: pointer;
  transition: all 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  font-family: 'Nunito', sans-serif;
  font-weight: 700;
  font-size: 0.9rem;
  box-shadow: var(--clay-shadow-sm);
}

.category-btn:hover {
  background: var(--bg-soft-pink) !important;
  color: var(--coral) !important;
  border-color: var(--coral-light);
  transform: translateX(4px);
}

.category-btn.active {
  background: linear-gradient(135deg, var(--coral) 0%, var(--coral-light) 100%) !important;
  color: #FFFFFF !important;
  font-weight: 800;
  border-color: transparent;
  box-shadow: 0 4px 15px rgba(255, 107, 107, 0.35);
}

.category-btn i {
  font-size: 1rem;
  margin-right: 6px;
}

/* 頁面標題 */
h2.text-center {
  font-family: 'Fredoka One', cursive !important;
  color: var(--text-dark) !important;
  font-size: 1.8rem !important;
  letter-spacing: 1px;
  margin: 2rem 0 !important;
}

/* 載入中提示 */
.spinner-border {
  border-color: var(--coral) !important;
  border-right-color: transparent !important;
}

.text-muted.mt-2 {
  color: var(--text-muted) !important;
  font-family: 'Nunito', sans-serif;
  font-size: 1rem;
}

/* 空狀態圖示 */
.bi-inbox {
  color: var(--text-light) !important;
}

/* 響應式調整 */
@media (max-width: 768px) {
  .category-sidebar {
    position: relative;
    top: auto;
    margin-bottom: 20px;
  }
  
  h2.text-center {
    font-size: 1.4rem !important;
  }
  
  .card-title {
    font-size: 0.85rem !important;
    min-height: auto;
  }
}
</style>