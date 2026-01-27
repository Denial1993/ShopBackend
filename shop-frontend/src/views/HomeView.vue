<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { authStore } from '../store.js'; // 引入 Store 檢查登入狀態
import { useRouter } from 'vue-router';  // 引入 Router
import { formatPrice } from '../utils/format.js';

const router = useRouter();
const products = ref([]);
const isLoading = ref(true);

const fetchProducts = async () => {
    try {
        const response = await axios.get('http://localhost:5000/api/Product');
        products.value = response.data;
    } catch (error) {
        console.error(error);
    } finally {
        isLoading.value = false;
    }
};

// 👇 新增：加入購物車功能
const addToCart = async (productId) => {
    // 1. 先檢查有沒有登入
    if (!authStore.isLoggedIn) {
        if (confirm("請先登入會員才能購物，要前往登入頁嗎？")) {
            router.push('/login');
        }
        return;
    }

    // 2. 發送 API
    try {
        // 根據你的 Swagger POST /api/Cart 需要傳送 productId 和 quantity
        await axios.post('http://localhost:5000/api/Cart', {
            productId: productId,
            quantity: 1 // 預設加 1 個
        });

        alert("✅ 已加入購物車！");
    } catch (error) {
        console.error(error);
        alert("❌ 加入失敗 (可能已經在車內，或庫存不足)");
    }
};

onMounted(() => {
    fetchProducts();
});
</script>

<template>
    <div class="container" style="margin-top: 80px;">
        <h2 class="text-center my-4">熱銷商品</h2>

        <div v-if="isLoading" class="text-center mt-5">
            <div class="spinner-border text-primary" role="status"></div>
            <p class="mt-2 text-muted">商品載入中...</p>
        </div>

        <div v-else class="row">
            <div class="col-md-4 col-lg-3 mb-4" v-for="item in products" :key="item.id">
                <div class="card h-100 shadow-sm border-0 product-card">
                    <div class="position-relative overflow-hidden" style="height: 200px;">
                        <img :src="item.imageUrl ? `/images/${item.imageUrl}` : 'https://placehold.co/600x400?text=No+Image'"
                            class="card-img-top w-100 h-100" style="object-fit: contain; padding: 10px;"
                            alt="Product Image">
                    </div>

                    <div class="card-body d-flex flex-column text-center">
                        <h5 class="card-title fs-6">{{ item.title }}</h5>
                        <p class="text-muted small mb-2">{{ item.categoryName }}</p>
                        <h5 class="fw-bold text-danger mt-auto"> NT$ {{ formatPrice(item.price) }}</h5>

                        <button @click="addToCart(item.id)" class="btn btn-outline-dark w-100 mt-2 rounded-0">
                            加入購物車
                        </button>
                    </div>
                </div>
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
</style>