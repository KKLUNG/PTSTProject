# useCMS Composable 使用說明

> **建立日期**: 2025-11-20  
> **專案**: PTSDProject  
> **狀態**: ✅ 已完成

---

## 📋 概述

`useCMS` 是一個 Vue 3 Composition API 封裝，提供所有 CMS 相關的 API 呼叫功能。

### 功能分類
- ✅ 使用者管理 (3 個方法)
- ✅ 部門管理 (2 個方法)
- ✅ 角色權限 (4 個方法)
- ✅ 系統代碼 (2 個方法)
- ✅ 系統日誌 (3 個方法)

---

## 🚀 快速開始

### 基本使用

```typescript
import { useCMS } from '@/composables/useCMS'

// 在 setup 中使用
const { 
  loading,        // 載入狀態
  error,          // 錯誤訊息
  getUserList,    // 取得使用者清單
  updateUserProfile // 更新使用者
} = useCMS()

// 呼叫 API
const users = await getUserList({
  Keyword: 'john',
  PageSize: 50
})
```

---

## API 方法清單

### 1. 使用者管理

#### getUserProfile(userGuid: string)
取得使用者個人資料

```typescript
const user = await getUserProfile('xxx-xxx-xxx')
// 回傳: { UserGuid, UserId, UserName, UserEmail, ... }
```

---

#### getUserList(params)
取得使用者清單

```typescript
const users = await getUserList({
  DeptGuid: 'xxx-xxx-xxx',  // 可選
  Keyword: 'john',           // 可選
  PageSize: 50,              // 可選，預設 50
  PageIndex: 1               // 可選，預設 1
})
// 回傳: [ { UserGuid, UserId, UserName, ... }, ... ]
```

---

#### updateUserProfile(userData)
更新使用者資料

```typescript
const result = await updateUserProfile({
  UserGuid: 'xxx-xxx-xxx',
  UserName: 'John Doe',
  UserTitle: '工程師',     // 可選
  UserEmail: 'john@example.com', // 可選
  UserCellPhone: '0912345678'    // 可選
})
// 回傳: { success: true, message: '更新成功' }
```

---

### 2. 部門管理

#### getDeptTree()
取得部門樹狀結構

```typescript
const deptTree = await getDeptTree()
// 回傳: [ { DeptGuid, DeptName, children: [...] }, ... ]
```

**注意**: 此方法已自動組裝成樹狀結構，可直接使用。

---

#### getDeptMembers(deptGuid: string)
取得部門成員

```typescript
const members = await getDeptMembers('xxx-xxx-xxx')
// 回傳: [ { UserGuid, UserId, UserName, IsDeptAdmin, ... }, ... ]
```

---

### 3. 角色權限

#### getRoles()
取得角色清單

```typescript
const roles = await getRoles()
// 回傳: [ { RoleGuid, RoleName, RoleDesc, IsActive }, ... ]
```

---

#### getUserRoles(userGuid: string)
取得使用者的角色

```typescript
const userRoles = await getUserRoles('xxx-xxx-xxx')
// 回傳: [ { RoleGuid, RoleName, RoleDesc }, ... ]
```

---

#### assignUserRole(userGuid, roleGuid)
分配角色給使用者

```typescript
const result = await assignUserRole('user-guid', 'role-guid')
// 回傳: { success: true, message: '角色分配成功' }
```

---

#### removeUserRole(userGuid, roleGuid)
移除使用者角色

```typescript
const result = await removeUserRole('user-guid', 'role-guid')
// 回傳: { success: true, message: '角色移除成功' }
```

---

### 4. 系統代碼

#### getCMSCodes(codesType: string)
取得系統代碼

```typescript
const codes = await getCMSCodes('01') // 01 = 控件類型
// 回傳: [ { CodesGuid, CodesType, CodesID, CodesName, ... }, ... ]
```

---

#### getCodesTypes()
取得所有代碼類型

```typescript
const types = await getCodesTypes()
// 回傳: [ { CodesType: '01' }, { CodesType: 'AL' }, ... ]
```

---

### 5. 系統日誌

#### getEventLog(params)
取得事件日誌

```typescript
const logs = await getEventLog({
  StartDate: '2025-11-01',  // 可選
  EndDate: '2025-11-20',    // 可選
  EventCode: 'CMS',         // 可選
  PageSize: 50,             // 可選，預設 50
  PageIndex: 1              // 可選，預設 1
})
// 回傳: [ { EventGuid, EventCode, EventName, Parameter, ... }, ... ]
```

---

#### logEvent(eventData)
記錄事件日誌

```typescript
await logEvent({
  EventCode: 'CMS',
  EventName: 'UserLogin',
  Parameter: '192.168.1.100',     // 可選
  CreatedUserID: 'john.doe'       // 可選
})
// 回傳: { success: true, message: '日誌記錄成功' }
```

---

#### setMenuLog(menuGuid, userGuid)
記錄選單存取日誌

```typescript
await setMenuLog('menu-guid', 'user-guid')
// 注意: 此方法失敗不會 throw error
```

---

## 完整範例

### 範例 1: 使用者列表頁面

```vue
<template>
  <div>
    <DxLoadPanel :visible="loading" />
    
    <DxDataGrid :data-source="users">
      <DxColumn data-field="UserId" caption="帳號" />
      <DxColumn data-field="UserName" caption="姓名" />
    </DxDataGrid>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useCMS } from '@/composables/useCMS'
import DxLoadPanel from 'devextreme-vue/load-panel'
import DxDataGrid, { DxColumn } from 'devextreme-vue/data-grid'

const { loading, getUserList } = useCMS()
const users = ref([])

onMounted(async () => {
  users.value = await getUserList({ PageSize: 50 })
})
</script>
```

---

### 範例 2: 編輯使用者

```vue
<script setup lang="ts">
import { ref } from 'vue'
import { useCMS } from '@/composables/useCMS'

const { updateUserProfile } = useCMS()
const user = ref({
  UserGuid: 'xxx-xxx-xxx',
  UserName: 'John Doe',
  UserEmail: 'john@example.com'
})

const handleSave = async () => {
  try {
    await updateUserProfile(user.value)
    alert('更新成功')
  } catch (error) {
    alert('更新失敗')
  }
}
</script>
```

---

### 範例 3: 角色管理

```vue
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useCMS } from '@/composables/useCMS'

const { getRoles, getUserRoles, assignUserRole } = useCMS()
const allRoles = ref([])
const userRoles = ref([])
const currentUserGuid = 'xxx-xxx-xxx'

onMounted(async () => {
  // 同時載入
  const [roles, userRoleData] = await Promise.all([
    getRoles(),
    getUserRoles(currentUserGuid)
  ])
  allRoles.value = roles
  userRoles.value = userRoleData
})

const handleAssign = async (roleGuid: string) => {
  await assignUserRole(currentUserGuid, roleGuid)
  // 重新載入使用者角色
  userRoles.value = await getUserRoles(currentUserGuid)
}
</script>
```

---

### 範例 4: 部門樹狀選單

```vue
<template>
  <DxTreeView
    :items="deptTree"
    display-expr="DeptName"
    key-expr="DeptGuid"
    @item-click="handleDeptClick"
  />
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useCMS } from '@/composables/useCMS'
import DxTreeView from 'devextreme-vue/tree-view'

const { getDeptTree, getDeptMembers } = useCMS()
const deptTree = ref([])
const members = ref([])

onMounted(async () => {
  deptTree.value = await getDeptTree()
})

const handleDeptClick = async (e: any) => {
  members.value = await getDeptMembers(e.itemData.DeptGuid)
  console.log('部門成員:', members.value)
}
</script>
```

---

## 錯誤處理

### 自動錯誤處理

`useCMS` 內建錯誤處理機制：

1. **loading 狀態**: 自動管理載入狀態
2. **error 狀態**: 自動捕獲錯誤訊息
3. **錯誤拋出**: API 錯誤會 throw，需要 try-catch

### 使用範例

```typescript
const { loading, error, getUserList } = useCMS()

// 方式 1: 使用 try-catch
try {
  const users = await getUserList()
} catch (err) {
  console.error('載入失敗:', error.value)
}

// 方式 2: 監看 error
watch(error, (newError) => {
  if (newError) {
    showAlert(newError)
  }
})
```

---

## JSON 自動解析

`useCMS` 會自動處理後端回傳的 JSON 字串：

```typescript
// 後端回傳字串: "[{...}, {...}]"
const users = await getUserList()
// 已自動解析為陣列: [{...}, {...}]

// 後端回傳物件: {...}
const user = await getUserProfile('xxx')
// 直接使用: {...}
```

---

## 狀態管理整合

### 與 Vuex/Pinia 整合

```typescript
// store/user.ts (Pinia)
import { defineStore } from 'pinia'
import { useCMS } from '@/composables/useCMS'

export const useUserStore = defineStore('user', () => {
  const users = ref([])
  const { getUserList } = useCMS()

  const loadUsers = async () => {
    users.value = await getUserList()
  }

  return { users, loadUsers }
})
```

---

## 效能優化建議

### 1. 快取策略

```typescript
// 快取角色清單 (很少變動)
const rolesCache = ref<any[] | null>(null)

const getRolesCached = async () => {
  if (rolesCache.value) {
    return rolesCache.value
  }
  rolesCache.value = await getRoles()
  return rolesCache.value
}
```

### 2. 分頁載入

```typescript
// 使用分頁避免一次載入過多資料
const loadUsersPage = async (pageIndex: number) => {
  return await getUserList({
    PageSize: 20,  // 限制每頁筆數
    PageIndex: pageIndex
  })
}
```

### 3. 並行請求

```typescript
// 同時載入多個資料
const loadAllData = async () => {
  const [users, depts, roles] = await Promise.all([
    getUserList(),
    getDeptTree(),
    getRoles()
  ])
  return { users, depts, roles }
}
```

---

## 注意事項

### ⚠️ 重要提醒

1. **Token 驗證**: 所有 API 都需要 JWT Token (除了登入相關)
2. **權限檢查**: 前端應檢查使用者權限，但安全性依賴後端
3. **錯誤處理**: 務必使用 try-catch 處理錯誤
4. **分頁限制**: 建議 PageSize 不超過 100

### 💡 最佳實踐

1. ✅ 使用 TypeScript 定義介面
2. ✅ 使用 loading 狀態顯示載入動畫
3. ✅ 使用 error 狀態顯示錯誤訊息
4. ✅ 使用 try-catch 捕獲異常
5. ✅ 避免在 loop 中呼叫 API

---

## TypeScript 型別定義

建議建立型別定義檔：

```typescript
// types/cms.ts
export interface User {
  UserGuid: string
  UserId: string
  UserName: string
  UserTitle?: string
  UserEmail?: string
  UserCellPhone?: string
  LastActiveDate?: string
  IsActive: boolean
}

export interface Role {
  RoleGuid: string
  RoleName: string
  RoleDesc: string
  IsActive: boolean
}

export interface Dept {
  DeptGuid: string
  DeptPGuid: string
  DeptName: string
  DeptNameAll: string
  children?: Dept[]
}
```

---

## 📝 變更記錄

| 日期 | 版本 | 變更內容 | 作者 |
|------|------|----------|------|
| 2025-11-20 | 1.0 | 建立 useCMS Composable | AI Assistant |

---

**文檔狀態**: 🟢 Complete  
**最後更新**: 2025-11-20  
**版本**: 1.0

