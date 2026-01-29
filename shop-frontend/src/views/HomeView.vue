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
/* 加一點點滑鼠移過去的特效 */
.product-card {
    transition: transform 0.2s;
}

.product-card:hover {
    transform: translateY(-5px);
    box-shadow: 0 .5rem 1rem rgba(0, 0, 0, .15) !important;
}

/* 分類側邊欄 */
.category-sidebar {
    position: sticky;
    top: 100px;
    padding: 20px;
    background: #f8f9fa;
    border-radius: 8px;
    box-shadow: 0 2px 4px rgba(0,0,0,.05);
}

.category-sidebar h4 {
    color: #333;
    font-size: 1.1rem;
    border-bottom: 2px solid #007bff;
    padding-bottom: 10px;
}

/* 分類按鈕 */
.category-btn {
    display: block;
    width: 100%;
    text-align: left;
    padding: 12px 16px;
    margin-bottom: 8px;
    border: none;
    background: white;
    color: #555;
    border-radius: 6px;
    cursor: pointer;
    transition: all 0.2s;
    font-size: 0.95rem;
    box-shadow: 0 1px 3px rgba(0,0,0,.05);
}

.category-btn:hover {
    background: #e9ecef;
    color: #000;
    transform: translateX(5px);
}

.category-btn.active {
    background: linear-gradient(135deg, #007bff 0%, #0056b3 100%);
    color: white;
    font-weight: 600;
    box-shadow: 0 4px 8px rgba(0,123,255,.3);
}

.category-btn i {
    font-size: 1rem;
}
</style>