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
    const response = await axios.get(`http://localhost:5000/api/Product/${productId}`);
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
    await axios.post('http://localhost:5000/api/Cart', {
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