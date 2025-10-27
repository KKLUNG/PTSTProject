# API 工具函數使用說明

> **建立日期**: 2025-10-27  
> **檔案位置**: `src/utils/api-util.ts`  
> **狀態**: ✅ 已完成

---

## 📋 目錄

1. [快速開始](#快速開始)
2. [API 函數清單](#api-函數清單)
3. [使用範例](#使用範例)
4. [錯誤處理](#錯誤處理)
5. [型別定義](#型別定義)
6. [注意事項](#注意事項)

---

## 🚀 快速開始

### 匯入方式

```typescript
// 方式 1: 具名匯入 (推薦)
import { apiGet, apiPost } from '@/utils/api-util'

// 方式 2: 全部匯入
import * as api from '@/utils/api-util'

// 方式 3: 預設匯入
import apiUtil from '@/utils/api-util'
```

### 基本使用

```typescript
// GET 請求
apiGet('/api/user/list', { page: 1 })
  .then(res => console.log(res.data))
  .catch(err => console.error(err))

// POST 請求
apiPost('/api/auth/login', { userId: 'admin', password: '123' })
  .then(res => console.log(res.data))
  .catch(err => console.error(err))
```

---

## 📦 API 函數清單

### 1. `apiGet<T>(apiUrl, params?)`

**用途**: 發送 GET 請求

**參數**:
- `apiUrl` (string): API 路徑
- `params?` (object): 查詢參數 (可選)

**回傳**: `Promise<AxiosResponse<T>>`

**範例**:
```typescript
// 不帶參數
apiGet('/api/user/profile')

// 帶查詢參數
apiGet('/api/user/list', { 
  page: 1, 
  size: 10,
  keyword: 'admin'
})

// 使用型別
interface User {
  userId: string
  userName: string
}
apiGet<User[]>('/api/user/list')
  .then(res => {
    const users: User[] = res.data
    console.log(users)
  })
```

---

### 2. `apiPost<T>(apiUrl, params?)`

**用途**: 發送 POST 請求

**參數**:
- `apiUrl` (string): API 路徑
- `params?` (object): 請求資料 (可選)

**回傳**: `Promise<AxiosResponse<T>>`

**範例**:
```typescript
// 登入
apiPost('/api/auth/login', {
  userId: 'admin',
  password: '123456'
})
.then(res => {
  if (res.status === 200) {
    console.log('登入成功', res.data)
  }
})

// 新增資料
apiPost('/api/user/create', {
  userId: 'newuser',
  userName: '新使用者',
  email: 'user@example.com'
})

// 使用型別
interface LoginResponse {
  Token: string
  UserId: string
  UserName: string
}
apiPost<LoginResponse[]>('/api/auth/login', { ... })
  .then(res => {
    const token = res.data[0].Token
    auth.logIn(token)
  })
```

---

### 3. `apiSSO<T>(apiUrl, params?)`

**用途**: 單點登入請求 (不帶 Authorization header)

**參數**:
- `apiUrl` (string): 完整的 API URL
- `params?` (object): 請求資料 (可選)

**回傳**: `Promise<AxiosResponse<T>>`

**特點**:
- ⚠️ 不使用 `baseURL`，需提供完整 URL
- ⚠️ 不帶 `Authorization` header
- ✅ 用於跨系統單點登入

**範例**:
```typescript
// 跨系統 SSO
apiSSO('https://other-system.com/api/sso/login', {
  userId: 'admin',
  systemId: 'EIP'
})
.then(res => {
  console.log('SSO 成功', res.data)
})

// 取得 SSO Token
apiSSO('https://eip.example.com/api/auth/GetSSOToken', {
  UserId: 'admin',
  SystemId: 'HR'
})
```

---

### 4. `apiFile<T>(apiUrl, params)`

**用途**: 上傳檔案

**參數**:
- `apiUrl` (string): API 路徑
- `params` (FormData | object): 檔案資料

**回傳**: `Promise<AxiosResponse<T>>`

**範例**:
```typescript
// 上傳單一檔案
const formData = new FormData()
formData.append('file', file)
formData.append('description', '測試檔案')

apiFile('/api/func/FileUpload', formData)
  .then(res => {
    console.log('上傳成功', res.data)
  })

// 上傳多個檔案
const formData = new FormData()
files.forEach(file => {
  formData.append('files', file)
})

apiFile('/api/func/MultiFileUpload', formData)
```

---

### 5. `apiDeleteFile<T>(fileUrl)`

**用途**: 刪除檔案

**參數**:
- `fileUrl` (string): 檔案路徑

**回傳**: `Promise<AxiosResponse<T>>`

**範例**:
```typescript
// 刪除單一檔案
apiDeleteFile('/upload/2024/document.pdf')
  .then(res => {
    console.log('檔案已刪除')
  })

// 批次刪除
const files = ['/upload/file1.pdf', '/upload/file2.pdf']
Promise.all(files.map(file => apiDeleteFile(file)))
  .then(() => {
    console.log('所有檔案已刪除')
  })
```

---

### 6. `apiGetBlobFile(fileUrl, fileName)`

**用途**: 下載檔案 (自動觸發瀏覽器下載)

**參數**:
- `fileUrl` (string): 檔案 URL
- `fileName` (string): 儲存的檔案名稱

**回傳**: `Promise<void>`

**範例**:
```typescript
// 下載檔案
apiGetBlobFile('/upload/report.pdf', 'report.pdf')
  .then(() => {
    console.log('下載已開始')
  })

// 下載並重新命名
apiGetBlobFile(
  '/upload/document_20241027.xlsx',
  '報表_2024年10月.xlsx'
)

// 動態檔名
const today = new Date().toISOString().split('T')[0]
apiGetBlobFile(
  '/api/report/export',
  `報表_${today}.xlsx`
)
```

---

### 7. `loadExtJs(src)`

**用途**: 動態載入外部 JavaScript 檔案

**參數**:
- `src` (string): JavaScript 檔案 URL

**回傳**: `Promise<void>`

**範例**:
```typescript
// 載入外部函式庫
loadExtJs('https://cdn.example.com/library.js')
  .then(() => {
    console.log('Library loaded')
    // 現在可以使用該函式庫
  })
  .catch(err => {
    console.error('載入失敗', err)
  })

// 按需載入
if (需要特定功能) {
  await loadExtJs('https://cdn.example.com/feature.js')
}
```

---

## 💡 使用範例

### 範例 1: 登入流程 (完整)

```typescript
<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { apiPost } from '@/utils/api-util'
import auth from '@/utils/auth'

const router = useRouter()
const userId = ref('')
const password = ref('')
const loading = ref(false)

interface LoginResponse {
  Token: string
  UserId: string
  UserName: string
  UserGuid: string
}

const handleLogin = async () => {
  loading.value = true
  
  try {
    const res = await apiPost<LoginResponse[]>('/api/auth/login', {
      userId: userId.value,
      password: password.value
    })
    
    if (res.status === 200) {
      // 儲存 Token
      auth.logIn(res.data[0].Token)
      
      // 儲存使用者資訊
      appInfo.userInfo.userId = res.data[0].UserId
      appInfo.userInfo.userName = res.data[0].UserName
      appInfo.userInfo.userGuid = res.data[0].UserGuid
      
      // 跳轉到首頁
      router.push('/home')
    }
  } catch (error) {
    console.error('登入失敗', error)
    alert('登入失敗：' + error)
  } finally {
    loading.value = false
  }
}
</script>
```

---

### 範例 2: 資料列表 (含分頁)

```typescript
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { apiGet } from '@/utils/api-util'

interface User {
  UserId: string
  UserName: string
  Email: string
}

const users = ref<User[]>([])
const loading = ref(false)
const currentPage = ref(1)
const pageSize = ref(10)

const loadUsers = async () => {
  loading.value = true
  
  try {
    const res = await apiGet<User[]>('/api/user/list', {
      page: currentPage.value,
      size: pageSize.value
    })
    
    users.value = res.data
  } catch (error) {
    console.error('載入失敗', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadUsers()
})
</script>
```

---

### 範例 3: 檔案上傳

```typescript
<script setup lang="ts">
import { ref } from 'vue'
import { apiFile } from '@/utils/api-util'

const uploading = ref(false)
const uploadProgress = ref(0)

interface UploadResponse {
  success: boolean
  fileUrl: string
  fileName: string
}

const handleFileUpload = async (file: File) => {
  uploading.value = true
  
  const formData = new FormData()
  formData.append('file', file)
  formData.append('category', 'document')
  
  try {
    const res = await apiFile<UploadResponse>('/api/func/FileUpload', formData)
    
    if (res.data.success) {
      console.log('上傳成功', res.data.fileUrl)
      alert('檔案已上傳：' + res.data.fileName)
    }
  } catch (error) {
    console.error('上傳失敗', error)
    alert('上傳失敗')
  } finally {
    uploading.value = false
  }
}

// 在 template 中使用
// <input type="file" @change="e => handleFileUpload(e.target.files[0])" />
</script>
```

---

### 範例 4: 錯誤處理 (進階)

```typescript
import { apiPost } from '@/utils/api-util'
import { AxiosError } from 'axios'

const saveData = async (data: any) => {
  try {
    const res = await apiPost('/api/data/save', data)
    
    // 成功處理
    if (res.status === 200) {
      console.log('儲存成功')
      return res.data
    }
  } catch (error) {
    // 錯誤處理
    if (typeof error === 'string') {
      // 來自 formatErrorMessage 的錯誤訊息
      console.error('API 錯誤：', error)
      alert(error)
    } else {
      // 其他類型的錯誤
      console.error('未預期的錯誤：', error)
      alert('發生未預期的錯誤')
    }
    
    throw error // 重新拋出錯誤
  }
}
```

---

## ❌ 錯誤處理

### 自動錯誤格式化

API 工具會自動格式化錯誤訊息：

```typescript
// 如果 API 呼叫失敗，錯誤訊息格式：
// "{XMLName/CommandName} execute failed!!<br />Error message: {詳細錯誤}"

// 範例
apiPost('/api/data/save', { XMLName: 'SaveUser.xml', ... })
  .catch(err => {
    // err = "SaveUser.xml execute failed!!<br />Error message: User already exists"
    console.error(err)
  })
```

### 網路錯誤

```typescript
apiGet('/api/user/list')
  .catch(err => {
    if (err === 'Network Error: Please check your internet connection.') {
      // 網路連線問題
      alert('請檢查網路連線')
    } else {
      // 其他錯誤
      alert(err)
    }
  })
```

### 使用 async/await

```typescript
const loadData = async () => {
  try {
    const res = await apiGet('/api/data')
    return res.data
  } catch (error) {
    console.error('載入失敗', error)
    throw error // 重新拋出以便上層處理
  }
}
```

---

## 📘 型別定義

### ApiParams

```typescript
interface ApiParams {
  [key: string]: any
  XMLName?: string      // 用於錯誤訊息
  CommandName?: string  // 用於錯誤訊息
}
```

### 使用範例

```typescript
const params: ApiParams = {
  userId: 'admin',
  XMLName: 'GetUserList.xml'
}

apiGet('/api/user/list', params)
```

### 泛型使用

```typescript
// 定義回應型別
interface UserListResponse {
  total: number
  users: User[]
}

// 使用泛型
apiGet<UserListResponse>('/api/user/list')
  .then(res => {
    const data: UserListResponse = res.data
    console.log(`共 ${data.total} 筆資料`)
  })
```

---

## ⚠️ 注意事項

### 1. Authorization Header

所有 API 函數 (除了 `apiSSO`) 都會自動帶上 Authorization header：

```typescript
headers: {
  'Authorization': 'Bearer ' + auth.getToken()
}
```

**確保**: 使用前必須先登入並取得 Token！

---

### 2. baseURL

API 函數會自動使用 `appInfo.serverUrl` 作為 baseURL：

```typescript
// 實際請求 URL = appInfo.serverUrl + apiUrl
apiGet('/api/user/list')
// 實際請求: https://localhost:44358/api/user/list
```

**特例**: `apiSSO` 不使用 baseURL，需提供完整 URL。

---

### 3. 檔案上傳

使用 `apiFile` 時，必須使用 `FormData`：

```typescript
// ✅ 正確
const formData = new FormData()
formData.append('file', file)
apiFile('/api/upload', formData)

// ❌ 錯誤
apiFile('/api/upload', { file: file })
```

---

### 4. 錯誤訊息包含 HTML

錯誤訊息可能包含 `<br />` HTML 標籤：

```typescript
apiPost('/api/save', { XMLName: 'Save.xml' })
  .catch(err => {
    // err = "Save.xml execute failed!!<br />Error message: ..."
    
    // 如果要顯示在 HTML 中
    element.innerHTML = err
    
    // 如果要顯示純文字
    alert(err.replace(/<br \/>/g, '\n'))
  })
```

---

### 5. Promise 回傳

所有 API 函數都回傳 Promise，支援：

```typescript
// 方式 1: .then().catch()
apiGet('/api/data')
  .then(res => console.log(res.data))
  .catch(err => console.error(err))

// 方式 2: async/await
try {
  const res = await apiGet('/api/data')
  console.log(res.data)
} catch (err) {
  console.error(err)
}
```

---

## 🔗 相關檔案

- **API 工具**: `src/utils/api-util.ts`
- **認證工具**: `src/utils/auth.ts`
- **應用資訊**: `src/utils/app-Info.ts`
- **全域設定**: `src/main.ts`

---

## 📞 常見問題

### Q1: 如何處理 401 未授權錯誤？

```typescript
apiGet('/api/data')
  .catch(err => {
    if (err?.response?.status === 401) {
      // Token 過期或無效
      auth.logOut()
      router.push('/login')
    }
  })
```

### Q2: 如何取消請求？

```typescript
import axios from 'axios'

const controller = new AbortController()

axios.get('/api/data', {
  signal: controller.signal
})

// 取消請求
controller.abort()
```

### Q3: 如何上傳進度追蹤？

目前 `apiFile` 不支援進度追蹤。如需要，可以直接使用 axios：

```typescript
import axios from 'axios'

const uploadWithProgress = (file: File, onProgress: (percent: number) => void) => {
  const formData = new FormData()
  formData.append('file', file)
  
  return axios.post('/api/upload', formData, {
    onUploadProgress: (progressEvent) => {
      const percent = Math.round((progressEvent.loaded * 100) / progressEvent.total!)
      onProgress(percent)
    }
  })
}
```

---

**文檔版本**: 1.0  
**最後更新**: 2025-10-27  
**維護者**: AI Assistant

