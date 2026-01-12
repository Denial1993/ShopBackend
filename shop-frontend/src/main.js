import { createApp } from 'vue'
import './style.css'
import App from './App.vue'

// 👇 加入這一行，引入 Bootstrap 的 CSS
import 'bootstrap/dist/css/bootstrap.min.css'
// 👇 加入這一行，引入 Bootstrap 的 JS (為了讓手機版漢堡選單會動)
import 'bootstrap/dist/js/bootstrap.bundle.min.js'

createApp(App).mount('#app')
