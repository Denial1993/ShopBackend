<template>
  <div class="container py-5 mt-5">
    <div class="d-flex justify-content-between align-items-center mb-5">
      <h2 class="fw-bold text-dark"><i class="bi bi-box-seam me-2"></i>產品管理</h2>
      <button @click="openModal()" class="btn btn-primary rounded-pill px-4 shadow-lg">
        <i class="bi bi-plus-lg me-2"></i>新增產品
      </button>
    </div>

    <div class="card border-0 clay-card overflow-hidden">
      <div class="table-responsive">
        <table class="table table-hover align-middle mb-0">
          <thead class="bg-light">
            <tr>
              <th class="ps-4">圖片</th>
              <th>名稱</th>
              <th>分類</th>
              <th>價格</th>
              <th>庫存</th>
              <th class="text-center">操作</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="p in products" :key="p.id">
              <td class="ps-4">
                <img :src="p.imageUrl ? (p.imageUrl.startsWith('http') ? p.imageUrl : `/images/${p.imageUrl}`) : 'https://placehold.co/50?text=No+Img'" 
                     class="rounded-3 shadow-sm clay-img" style="width: 50px; height: 50px; object-fit: cover;">
              </td>
              <td><div class="fw-bold text-dark">{{ p.title }}</div></td>
              <td><span class="badge bg-secondary rounded-pill px-3">{{ p.categoryName }}</span></td>
              <td class="text-danger fw-bold">NT$ {{ p.price }}</td>
              <td><span class="badge bg-light text-dark border">{{ p.stock }}</span></td>
              <td class="text-center">
                <button @click="openModal(p)" class="btn btn-sm btn-outline-primary me-2 rounded-pill">編輯</button>
                <button @click="deleteProduct(p.id)" class="btn btn-sm btn-outline-danger rounded-pill">刪除</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <!-- Modal -->
    <div class="modal fade" id="productModal" tabindex="-1" ref="modalRef">
      <div class="modal-dialog modal-lg modal-dialog-centered">
        <div class="modal-content border-0 shadow-lg clay-modal">
          <div class="modal-header border-0 pb-0">
            <h5 class="modal-title fw-bold text-dark ps-3 pt-3">{{ isEdit ? '✏️ 編輯產品' : '✨ 新增產品' }}</h5>
            <button type="button" class="btn-close me-3 mt-3" data-bs-dismiss="modal"></button>
          </div>
          <div class="modal-body p-4 pt-2">
            <form @submit.prevent="saveProduct" class="p-2">
              <div class="row g-3">
                <div class="col-md-8">
                  <label class="form-label">產品名稱</label>
                  <input v-model="form.title" type="text" class="form-control clay-input" required>
                </div>
                <div class="col-md-4">
                  <label class="form-label">價格</label>
                  <input v-model.number="form.price" type="number" class="form-control clay-input" required>
                </div>
                <div class="col-md-6">
                  <label class="form-label">分類</label>
                  <select v-model.number="form.categoryId" class="form-select clay-input" required>
                    <option v-for="c in categories" :key="c.id" :value="c.id">{{ c.name }}</option>
                  </select>
                </div>
                <div class="col-md-6">
                  <label class="form-label">庫存</label>
                  <input v-model.number="form.stock" type="number" class="form-control clay-input" required>
                </div>
                <div class="col-12">
                  <label class="form-label">描述</label>
                  <textarea v-model="form.description" class="form-control clay-input" rows="3"></textarea>
                </div>
                <div class="col-12">
                  <label class="form-label">產品圖片</label>
                  <input type="file" class="form-control clay-input" @change="onFileChange" accept="image/*">
                  <div v-if="previewUrl" class="mt-3 text-center p-3 clay-preview">
                    <img :src="previewUrl" class="rounded-3 shadow-sm" style="max-height: 200px;">
                  </div>
                </div>
              </div>
              <div class="text-end mt-4 pt-2 border-top">
                <button type="button" class="btn btn-light me-2 rounded-pill px-4" data-bs-dismiss="modal">取消</button>
                <button type="submit" class="btn btn-primary px-4 rounded-pill shadow-sm">儲存</button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { Modal } from 'bootstrap';

const apiClient = axios.create({
  baseURL: `${import.meta.env.VITE_API_BASE_URL}/api`,
  headers: {
    Authorization: `Bearer ${sessionStorage.getItem('shop_token')}`
  }
});

const products = ref([]);
const categories = ref([]);
const isEdit = ref(false);
const editingId = ref(null);
const modalRef = ref(null);
let modalObj = null;

const form = ref({
  title: '',
  price: 0,
  description: '',
  stock: 10,
  categoryId: 1,
  image: null
});

const previewUrl = ref(null);

const fetchProducts = async () => {
  const res = await apiClient.get('/AdminProduct');
  products.value = res.data;
};

const fetchCategories = async () => {
  const res = await axios.get(`${import.meta.env.VITE_API_BASE_URL}/api/Category`);
  categories.value = res.data;
};

const onFileChange = (e) => {
  const file = e.target.files[0];
  if (file) {
    form.value.image = file;
    previewUrl.value = URL.createObjectURL(file);
  }
};

const openModal = (p = null) => {
  if (p) {
    isEdit.value = true;
    editingId.value = p.id;
    form.value = { ...p, image: null };
    previewUrl.value = p.imageUrl ? (p.imageUrl.startsWith('http') ? p.imageUrl : `/images/${p.imageUrl}`) : null;
  } else {
    isEdit.value = false;
    editingId.value = null;
    form.value = { title: '', price: 0, description: '', stock: 10, categoryId: categories.value[0]?.id || 1, image: null };
    previewUrl.value = null;
  }
  modalObj.show();
};

const saveProduct = async () => {
  const formData = new FormData();
  formData.append('title', form.value.title);
  formData.append('price', form.value.price);
  formData.append('description', form.value.description || '');
  formData.append('stock', form.value.stock);
  formData.append('categoryId', form.value.categoryId);
  if (form.value.image) {
    formData.append('image', form.value.image);
  }

  try {
    if (isEdit.value) {
      await apiClient.put(`/AdminProduct/${editingId.value}`, formData);
    } else {
      await apiClient.post('/AdminProduct', formData);
    }
    modalObj.hide();
    fetchProducts();
    alert('儲存成功');
  } catch (err) {
    console.error(err);
    alert('儲存失敗：' + (err.response?.data?.message || err.message));
  }
};

const deleteProduct = async (id) => {
  if (!confirm('確定要刪除嗎？')) return;
  try {
    await apiClient.delete(`/AdminProduct/${id}`);
    fetchProducts();
    alert('刪除成功');
  } catch (err) {
    alert('刪除失敗');
  }
};

onMounted(() => {
  fetchProducts();
  fetchCategories();
  modalObj = new Modal(modalRef.value);
});
</script>

<style scoped>
/* 🐾 後台產品管理 - Claymorphism */

.clay-card {
  background: var(--bg-card);
  border-radius: 28px;
  box-shadow: 12px 12px 24px rgba(174, 160, 140, 0.2),
              -8px -8px 20px rgba(255, 255, 255, 0.8);
}

h2 {
  font-family: 'Fredoka One', cursive !important;
  color: var(--text-dark);
}

/* 表格樣式 */
.table thead th {
  font-family: 'Fredoka One', cursive;
  color: var(--text-dark);
  background-color: var(--bg-soft-pink) !important;
  border: none;
  font-weight: 500;
}

.clay-img {
  border: 2px solid #fff;
  box-shadow: 2px 2px 5px rgba(0,0,0,0.1);
}

.badge.bg-secondary {
  background-color: var(--text-muted) !important;
  font-family: 'Nunito', sans-serif;
  font-weight: 700;
}

.text-danger {
  color: var(--coral) !important;
  font-family: 'Fredoka One', cursive;
}

/* 按鈕 */
.btn-outline-primary {
  color: var(--mint-dark);
  border-color: var(--mint-dark);
}
.btn-outline-primary:hover {
  background-color: var(--mint-dark);
  color: #fff;
}

.btn-outline-danger {
  color: var(--coral);
  border-color: var(--coral);
}
.btn-outline-danger:hover {
  background-color: var(--coral);
  color: #fff;
}

/* Modal Claymorphism */
.clay-modal {
  border-radius: 32px;
  overflow: hidden;
  border: none;
}

.modal-header {
  background: linear-gradient(135deg, var(--bg-cream) 0%, #fff 100%);
}

.clay-input {
  background-color: var(--bg-cream) !important;
  border: 1px solid rgba(0,0,0,0.05) !important;
  border-radius: 12px !important;
  box-shadow: inset 2px 2px 5px rgba(174, 160, 140, 0.1),
              inset -2px -2px 5px rgba(255, 255, 255, 0.7) !important;
}

.clay-input:focus {
  background-color: #fff !important;
  box-shadow: 0 0 0 3px rgba(255, 107, 107, 0.2) !important;
  border-color: var(--coral) !important;
}

.clay-preview {
  background-color: var(--bg-cream);
  border-radius: 16px;
  border: 1px dashed var(--text-muted);
}
</style>
