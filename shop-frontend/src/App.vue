<script setup>
// 這裡是寫 JavaScript 邏輯的地方 (目前還是空的)
</script>

<template>
  <nav class="navbar navbar-expand-lg navbar-light bg-white border-bottom shadow-sm fixed-top">
    <div class="container">
      <a class="navbar-brand fw-bold fs-3" href="#">ShopDemo</a>
      
      <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
        <span class="navbar-toggler-icon"></span>
      </button>

      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav ms-auto">
          <li class="nav-item">
            <a class="nav-link active" href="#">首頁</a>
          </li>
          <li class="nav-item">
            <a class="nav-link" href="#">最新商品</a>
          </li>
          <li class="nav-item">
            <a class="nav-link" href="#">登入 / 註冊</a>
          </li>
          <li class="nav-item">
            <a class="nav-link btn btn-primary text-white ms-2 rounded-pill px-4" href="#">
              購物車 (0)
            </a>
          </li>
        </ul>
      </div>
    </div>
  </nav>

  <div class="container" style="margin-top: 80px;">
    <h2 class="text-center my-4">熱銷商品</h2>
    
    <div class="text-center">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Loading...</span>
      </div>
      <p class="mt-2">商品載入中...</p>
    </div>

  </div>
</template>

<style scoped>
/* 這裡寫額外的 CSS，目前先留空 */
</style>