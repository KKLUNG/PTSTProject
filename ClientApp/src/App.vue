<template>
  <div class="container">
    <h2>登入</h2>
    <div class="field">
      <label>帳號</label>
      <input v-model.trim="username" @keyup.enter="login" />
    </div>
    <div class="field">
      <label>密碼</label>
      <input type="password" v-model="password" @keyup.enter="login" />
    </div>
    <button :disabled="loading" @click="login">登入</button>
    <p v-if="error" class="error">{{ error }}</p>
    <pre v-if="me" class="success">{{ me }}</pre>
  </div>
</template>

<script setup lang="ts">
import axios from 'axios'
import { ref } from 'vue'

const username = ref('')
const password = ref('')
const loading = ref(false)
const error = ref('')
const me = ref<any>(null)

async function login() {
  error.value = ''
  me.value = null
  if (!username.value || !password.value) {
    error.value = '請輸入帳號與密碼'
    return
  }
  loading.value = true
  try {
    const res = await axios.post('/api/auth/login', {
      username: username.value,
      password: password.value
    })
    const token = res.data.access_token
    localStorage.setItem('access_token', token)
    const meRes = await axios.get('/api/auth/me', {
      headers: { Authorization: `Bearer ${token}` }
    })
    me.value = meRes.data
  } catch (e: any) {
    error.value = e?.response?.data?.message || '登入失敗'
  } finally {
    loading.value = false
  }
}
</script>

<style>
.container { max-width: 420px; margin: 40px auto; font-family: system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif; }
.field { margin: 12px 0; display: flex; flex-direction: column; }
input { padding: 8px; border: 1px solid #ddd; border-radius: 6px; }
button { padding: 8px 14px; border-radius: 6px; }
.error { color: #c00; }
.success { background: #f5f5f5; padding: 12px; border-radius: 8px; }
</style>


