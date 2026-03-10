<script setup>
</script>

<template>
  <footer class="paw-footer mt-5 pt-5 pb-4">
    <div class="container">
      <div class="row">
        <!-- 公司簡介 -->
        <div class="col-md-4 mb-4">
          <h5 class="fw-bold mb-3 footer-title">🐾 公司簡介</h5>
          <p class="text-muted">
            PawPals 致力於提供高品質的商品。不管是精選電子產品還是可愛貓咪，我們都用心嚴選，只為給您最好的購物體驗。
          </p>
        </div>

        <!-- 貓咪運送規則 -->
        <div class="col-md-4 mb-4">
          <h5 class="fw-bold mb-3 footer-title">🐱 貓咪運送規則</h5>
          <ul class="list-unstyled text-muted footer-links">
            <li><a href="#" @click.prevent>專車接送運送時間</a></li>
            <li><a href="#" @click.prevent>活體運送專業保障</a></li>
            <li><a href="#" @click.prevent>到達後安置與陪伴建議</a></li>
          </ul>
        </div>

        <!-- 電子產品退貨原則 -->
        <div class="col-md-4 mb-4">
          <h5 class="fw-bold mb-3 footer-title">💻 電子產品退貨原則</h5>
          <ul class="list-unstyled text-muted footer-links">
            <li><a href="#" @click.prevent>七天鑑賞期權益</a></li>
            <li><a href="#" @click.prevent>退換貨流程說明</a></li>
            <li><a href="#" @click.prevent>原廠保固與維修服務</a></li>
          </ul>
        </div>
      </div>
      <hr class="mt-4 mb-4 border-secondary opacity-25">
      <div class="text-center text-muted copyright-text">
        <small>© {{ new Date().getFullYear() }} PawPals. All Rights Reserved.</small>
      </div>
    </div>
  </footer>
</template>

<style scoped>
.paw-footer {
  /* 使用 var(--bg-card) 或 var(--bg-warm-white)，Dark Reader 會正確辨識並反轉 */
  background-color: var(--bg-card, #FFFFFF);
  border-top: 3px solid var(--bg-soft-pink, #ffe5e5);
  box-shadow: 0 -4px 20px rgba(174, 160, 140, 0.08);
  /* 加入 transition 讓主題切換更滑順 */
  transition: all 0.3s ease;
}

.footer-title {
  color: var(--text-dark, #2D2D2D);
  font-family: 'Fredoka One', cursive, sans-serif;
  letter-spacing: 0.5px;
}

.text-muted {
  color: var(--text-muted, #8B7E74) !important;
  font-size: 0.95rem;
  line-height: 1.6;
}

.copyright-text {
  font-size: 0.85rem;
  color: var(--text-muted, #8B7E74) !important;
}

.footer-links li {
  margin-bottom: 10px;
}

.footer-links a {
  color: var(--text-muted, #8B7E74);
  text-decoration: none;
  transition: all 0.3s ease;
  position: relative;
  display: inline-block;
  padding-bottom: 2px;
}

.footer-links a:hover {
  color: var(--coral, #FF6B6B);
  transform: translateX(5px);
}

.footer-links a::after {
  content: '';
  position: absolute;
  width: 0;
  height: 2px;
  bottom: 0;
  left: 0;
  background-color: var(--coral, #FF6B6B);
  transition: width 0.3s ease;
  border-radius: 2px;
}

.footer-links a:hover::after {
  width: 100%;
}
</style>
