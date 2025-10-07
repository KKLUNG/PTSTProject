<template>
  <div class="container">

    <div class="login-card dx-card">
      <h2 class="title">登入</h2>

      <div class="field">
        <label>帳號</label>
        <DxTextBox
          v-model:value="username"
          placeholder="請輸入帳號"
          stylingMode="filled"
          @keyup.enter="login"
        />
      </div>

      <div class="field">
        <label>密碼</label>
        <DxTextBox
          v-model:value="password"
          mode="password"
          placeholder="請輸入密碼"
          stylingMode="filled"
          @keyup.enter="login"
        />
      </div>


      <DxButton
        :text="'登入'"
        type="default"
        stylingMode="contained"
        :disabled="loading || !username || !password"
        @click="onLoginClick"
      />
      <p v-if="error" class="error">{{ error }}</p>
      <pre v-if="me" class="success">{{ me }}</pre>
    </div>
  </div>
</template>

<script setup lang="ts">
import axios from 'axios'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import DxTextBox from 'devextreme-vue/text-box'
import DxButton from 'devextreme-vue/button'
import auth from '../utils/auth'

const username = ref('')
const password = ref('')
const loading = ref(false)
const error = ref('')
const me = ref<any>(null)
const router = useRouter()



async function login() {
  error.value = ''
  me.value = null
  if (!username.value && !password.value) {
    error.value = '請輸入帳號與密碼'
    return
  }
  if (!username.value) {
    error.value = '請輸入帳號'
    return
  }
  if (!password.value) {
    error.value = '請輸入密碼'
    return
  }
  loading.value = true
  try {
    const res = await axios.post('/api/auth/login', {
      username: username.value,
      password: password.value
    })
    me.value = JSON.stringify(res.data, null, 2)

    // 保存 JWT 並設定 Authorization header
    const token = res.data?.token
    if (token) {
      localStorage.setItem('authToken', token)
      axios.defaults.headers.common['Authorization'] = `Bearer ${token}`
      // 記錄登入並導頁
      auth.logIn(token)
      router.push('/CMSHomePage')
    }
  } catch (e: any) {
    error.value = e.response.data.message || '登入失敗'
  } finally {
    loading.value = false
  }
}

async function onLoginClick() {
  username.value = username.value.trim()
  password.value = password.value.trim()
  await login()
}

</script>

<style>
.container { max-width: 420px; margin: 40px auto; font-family: system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif; }
.login-card { padding: 20px; }
.title { margin: 0 0 12px; font-weight: 600; }
.field { margin: 12px 0; display: flex; flex-direction: column; }
.error { color: #c00; margin-top: 12px; }
.success { background: #f5f5f5; padding: 12px; border-radius: 8px; margin-top: 12px; }
</style>


