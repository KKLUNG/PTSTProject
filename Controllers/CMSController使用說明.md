# CMSController API 使用說明

> **建立日期**: 2025-11-20  
> **專案**: PTSDProject  
> **狀態**: ✅ 已完成核心 API (排除推播功能)

---

## 📋 API 清單概覽

| 分類 | API 數量 | 說明 |
|------|---------|------|
| **登入核心 API** | 4 | 系統配置、多語言、匯率、預載 |
| **選單管理** | 4 | 主選單、側邊欄選單、頁面選單、選單日誌 |
| **使用者管理** | 3 | 個人資料、使用者清單、更新資料 |
| **部門管理** | 2 | 部門樹、部門成員 |
| **角色權限** | 4 | 角色清單、使用者角色、分配/移除角色 |
| **系統代碼** | 2 | 代碼查詢、代碼類型 |
| **系統日誌** | 2 | 事件日誌、日誌記錄 |
| **總計** | **21 個 API** |  |

---

## 1. 登入核心 API

### 1.1 取得系統配置
```http
GET /api/CMS/GetCMSConfig
Authorization: Bearer {token} (可選 - AllowAnonymous)
```

**用途**: 在登入頁載入系統設定（標題、Logo、版本等）

**回應範例**:
```json
[
  {
    "ConfigGuid": "xxx-xxx-xxx",
    "ConfigKey": "SystemTitle",
    "ConfigValue": "PTSD 專案管理系統",
    "IsActive": true
  }
]
```

---

### 1.2 取得多語言字典
```http
GET /api/CMS/GetCMSLang?LangType=zhTW
Authorization: Bearer {token} (可選 - AllowAnonymous)
```

**參數**:
- `LangType`: 語言代碼 (`zhTW`, `enUS`, `vi`, `th`, `id`, `zhCN`)

**回應範例**:
```json
[
  {
    "LangGuid": "xxx-xxx-xxx",
    "LangType": "zhTW",
    "LangKey": "Login",
    "LangValue": "登入"
  }
]
```

---

### 1.3 取得匯率資料
```http
GET /api/CMS/GetExchange
Authorization: Bearer {token} (可選 - AllowAnonymous)
```

**用途**: 取得最新的貨幣匯率

**回應範例**:
```json
[
  {
    "Currency": "USD",
    "ExchangeRate": 31.250000
  },
  {
    "Currency": "EUR",
    "ExchangeRate": 34.120000
  }
]
```

---

### 1.4 預載 XML 資料
```http
GET /api/CMS/GetCMSFormPreloadByXMLNames?XMLNames=UserForm,DeptForm
Authorization: Bearer {token} (可選 - AllowAnonymous)
```

**參數**:
- `XMLNames`: 以逗號分隔的 XML 表單名稱

**回應**: 
```json
{}
```
*(目前返回空物件，未來實作)*

---

## 2. 選單管理 API

### 2.1 取得主選單磁磚
```http
GET /api/CMS/GetCMSMainMenu?UserGuid={userGuid}&Language=zhTW
Authorization: Bearer {token}
```

**用途**: 首頁主選單磁磚顯示

**參數**:
- `UserGuid`: 使用者 GUID
- `Language`: 語言代碼

**回應範例**:
```json
[
  {
    "MenuTitle": "系統管理",
    "MenuGuid": "xxx-xxx-xxx",
    "MenuPGuid": "yyy-yyy-yyy",
    "MenuIcon": "icon dx-icon-preferences"
  }
]
```

---

### 2.2 取得側邊欄選單樹
```http
GET /api/CMS/GetCMSMenuByUserId?
  KeyParameter={menuGuid}&
  UserGuid={userGuid}&
  Language=zhTW&
  IgnoreACL=0&
  DisplayMode=1
Authorization: Bearer {token}
```

**參數**:
- `KeyParameter`: 根選單 GUID
- `UserGuid`: 使用者 GUID
- `Language`: 語言代碼
- `IgnoreACL`: 是否忽略權限 (`0`=檢查, `1`=忽略)
- `DisplayMode`: 顯示模式 (`1`=PC, `2`=Mobile)

**回應**: 樹狀結構的選單 JSON

---

### 2.3 取得頁面選單項目
```http
GET /api/CMS/GetCMSMenus?MenuGuid={menuGuid}
Authorization: Bearer {token}
```

**用途**: 取得某個選單頁面下的所有 MenuItems (控件配置)

**回應範例**:
```json
[
  {
    "MenusGuid": "xxx-xxx-xxx",
    "MenuGuid": "yyy-yyy-yyy",
    "MenusTitle": "使用者列表",
    "MenusControl": "AdminGridForm",
    "MenusXMLName": "CMSUser",
    "MenusPosition": "3",
    "MenusSequence": 1
  }
]
```

---

### 2.4 記錄選單存取日誌
```http
POST /api/CMS/SetCMSMenuLog
Authorization: Bearer {token}
Content-Type: application/json

{
  "MenuGuid": "xxx-xxx-xxx",
  "UserGuid": "yyy-yyy-yyy"
}
```

**用途**: 記錄使用者存取選單的時間（用於統計分析）

---

## 3. 使用者管理 API

### 3.1 取得使用者個人資料
```http
GET /api/CMS/GetUserProfile?UserGuid={userGuid}
Authorization: Bearer {token}
```

**回應範例**:
```json
[
  {
    "UserGuid": "xxx-xxx-xxx",
    "UserId": "john.doe",
    "UserName": "John Doe",
    "UserTitle": "工程師",
    "UserEmail": "john@example.com",
    "UserCellPhone": "0912345678",
    "UserImageUrl": "/upload/user.jpg",
    "LastActiveDate": "2025-11-20T10:30:00",
    "DeptGuid": "yyy-yyy-yyy",
    "DeptName": "資訊部",
    "DeptNameAll": "總公司/資訊部"
  }
]
```

---

### 3.2 取得使用者清單
```http
GET /api/CMS/GetUserList?
  DeptGuid={deptGuid}&
  Keyword={keyword}&
  PageSize=50&
  PageIndex=1
Authorization: Bearer {token}
```

**參數**:
- `DeptGuid`: 部門 GUID (可選，篩選特定部門)
- `Keyword`: 關鍵字 (模糊搜尋 UserId, UserName, Email)
- `PageSize`: 每頁筆數 (預設 50)
- `PageIndex`: 頁碼 (從 1 開始)

**回應範例**:
```json
[
  {
    "UserGuid": "xxx-xxx-xxx",
    "UserId": "john.doe",
    "UserName": "John Doe",
    "UserTitle": "工程師",
    "UserEmail": "john@example.com",
    "UserCellPhone": "0912345678",
    "UserImageUrl": "/upload/user.jpg",
    "LastActiveDate": "2025-11-20T10:30:00",
    "IsActive": true
  }
]
```

---

### 3.3 更新使用者個人資料
```http
POST /api/CMS/UpdateUserProfile
Authorization: Bearer {token}
Content-Type: application/json

{
  "UserGuid": "xxx-xxx-xxx",
  "UserName": "John Doe",
  "UserTitle": "資深工程師",
  "UserEmail": "john@example.com",
  "UserCellPhone": "0912345678"
}
```

**回應**:
```json
{
  "success": true,
  "message": "更新成功"
}
```

---

## 4. 部門管理 API

### 4.1 取得部門樹狀結構
```http
GET /api/CMS/GetDeptTree
Authorization: Bearer {token}
```

**回應範例**:
```json
[
  {
    "DeptGuid": "xxx-xxx-xxx",
    "DeptPGuid": "00000000-0000-0000-0000-000000000000",
    "DeptID": "HQ",
    "DeptName": "總公司",
    "DeptNameAll": "總公司",
    "DeptOrder": 1
  },
  {
    "DeptGuid": "yyy-yyy-yyy",
    "DeptPGuid": "xxx-xxx-xxx",
    "DeptID": "IT",
    "DeptName": "資訊部",
    "DeptNameAll": "總公司/資訊部",
    "DeptOrder": 2
  }
]
```

**前端處理**: 需自行組裝成樹狀結構 (根據 `DeptPGuid`)

---

### 4.2 取得部門成員
```http
GET /api/CMS/GetDeptMembers?DeptGuid={deptGuid}
Authorization: Bearer {token}
```

**回應範例**:
```json
[
  {
    "UserGuid": "xxx-xxx-xxx",
    "UserId": "john.doe",
    "UserName": "John Doe",
    "UserTitle": "工程師",
    "DeptGuid": "yyy-yyy-yyy",
    "IsDeptAdmin": true
  }
]
```

---

## 5. 角色權限 API

### 5.1 取得角色清單
```http
GET /api/CMS/GetRoles
Authorization: Bearer {token}
```

**回應範例**:
```json
[
  {
    "RoleGuid": "xxx-xxx-xxx",
    "RoleName": "SystemAdmin",
    "RoleDesc": "系統管理員",
    "IsActive": true
  }
]
```

---

### 5.2 取得使用者的角色
```http
GET /api/CMS/GetUserRoles?UserGuid={userGuid}
Authorization: Bearer {token}
```

**回應範例**:
```json
[
  {
    "RoleGuid": "xxx-xxx-xxx",
    "RoleName": "SystemAdmin",
    "RoleDesc": "系統管理員"
  }
]
```

---

### 5.3 分配角色給使用者
```http
POST /api/CMS/AssignUserRole
Authorization: Bearer {token}
Content-Type: application/json

{
  "UserGuid": "xxx-xxx-xxx",
  "RoleGuid": "yyy-yyy-yyy"
}
```

**回應**:
```json
{
  "success": true,
  "message": "角色分配成功"
}
```

---

### 5.4 移除使用者角色
```http
POST /api/CMS/RemoveUserRole
Authorization: Bearer {token}
Content-Type: application/json

{
  "UserGuid": "xxx-xxx-xxx",
  "RoleGuid": "yyy-yyy-yyy"
}
```

**回應**:
```json
{
  "success": true,
  "message": "角色移除成功"
}
```

---

## 6. 系統代碼 API

### 6.1 取得系統代碼
```http
GET /api/CMS/GetCMSCodes?CodesType=01
Authorization: Bearer {token}
```

**參數**:
- `CodesType`: 代碼類型 (例如 `01`=控件類型, `AL`=匯率)

**回應範例**:
```json
[
  {
    "CodesGuid": "xxx-xxx-xxx",
    "CodesType": "01",
    "CodesID": "AdminGridForm",
    "CodesName": "網格表單",
    "CodesDesc1": "GridForm",
    "CodesDesc2": "",
    "CodesDesc3": "1"
  }
]
```

---

### 6.2 取得所有代碼類型
```http
GET /api/CMS/GetCodesTypes
Authorization: Bearer {token}
```

**回應範例**:
```json
[
  {
    "CodesType": "01"
  },
  {
    "CodesType": "AL"
  }
]
```

---

## 7. 系統日誌 API

### 7.1 取得事件日誌
```http
GET /api/CMS/GetEventLog?
  StartDate=2025-11-01&
  EndDate=2025-11-20&
  EventCode=CMS&
  PageSize=50&
  PageIndex=1
Authorization: Bearer {token}
```

**參數**:
- `StartDate`: 開始日期 (可選)
- `EndDate`: 結束日期 (可選)
- `EventCode`: 事件代碼 (可選，例如 `CMS`, `WFS`, `AUTH`)
- `PageSize`: 每頁筆數 (預設 50)
- `PageIndex`: 頁碼 (從 1 開始)

**回應範例**:
```json
[
  {
    "EventGuid": "xxx-xxx-xxx",
    "EventCode": "CMS",
    "EventName": "Login",
    "Parameter": "192.168.1.100",
    "CreatedUserID": "john.doe",
    "CreatedDate": "2025-11-20T10:30:00"
  }
]
```

---

### 7.2 記錄事件日誌
```http
POST /api/CMS/LogEvent
Authorization: Bearer {token}
Content-Type: application/json

{
  "EventCode": "CMS",
  "EventName": "UserLogin",
  "Parameter": "192.168.1.100",
  "CreatedUserID": "john.doe"
}
```

**回應**:
```json
{
  "success": true,
  "message": "日誌記錄成功"
}
```

---

## 使用範例

### 前端 Vue 3 範例

#### 1. 取得使用者清單
```typescript
import { apiGet } from '@/utils/api-util'

const getUserList = async (deptGuid: string, keyword: string) => {
  try {
    const params = {
      DeptGuid: deptGuid,
      Keyword: keyword,
      PageSize: 50,
      PageIndex: 1
    }
    
    const response = await apiGet('/api/CMS/GetUserList', params)
    
    if (response.status === 200) {
      const users = typeof response.data === 'string' 
        ? JSON.parse(response.data) 
        : response.data
      
      console.log('使用者清單:', users)
      return users
    }
  } catch (error) {
    console.error('取得使用者清單失敗:', error)
  }
}
```

---

#### 2. 更新使用者資料
```typescript
import { apiPost } from '@/utils/api-util'

const updateProfile = async (userGuid: string, userData: any) => {
  try {
    const params = {
      UserGuid: userGuid,
      UserName: userData.userName,
      UserTitle: userData.userTitle,
      UserEmail: userData.userEmail,
      UserCellPhone: userData.userCellPhone
    }
    
    const response = await apiPost('/api/CMS/UpdateUserProfile', params)
    
    if (response.status === 200) {
      console.log('更新成功:', response.data)
      return response.data
    }
  } catch (error) {
    console.error('更新失敗:', error)
  }
}
```

---

#### 3. 取得部門樹
```typescript
const getDeptTree = async () => {
  try {
    const response = await apiGet('/api/CMS/GetDeptTree', {})
    
    if (response.status === 200) {
      const depts = typeof response.data === 'string' 
        ? JSON.parse(response.data) 
        : response.data
      
      // 組裝樹狀結構
      const buildTree = (items: any[], parentId: string) => {
        return items
          .filter(item => item.DeptPGuid === parentId)
          .map(item => ({
            ...item,
            children: buildTree(items, item.DeptGuid)
          }))
      }
      
      const tree = buildTree(depts, '00000000-0000-0000-0000-000000000000')
      return tree
    }
  } catch (error) {
    console.error('取得部門樹失敗:', error)
  }
}
```

---

#### 4. 分配角色
```typescript
const assignRole = async (userGuid: string, roleGuid: string) => {
  try {
    const params = {
      UserGuid: userGuid,
      RoleGuid: roleGuid
    }
    
    const response = await apiPost('/api/CMS/AssignUserRole', params)
    
    if (response.status === 200) {
      console.log('角色分配成功:', response.data)
      return response.data.success
    }
  } catch (error) {
    console.error('角色分配失敗:', error)
  }
}
```

---

## 資料庫需求

以下資料表/檢視表/函數必須存在：

### 必要資料表
```sql
-- 使用者與權限
CMSUser, CMSRole, CMSUserInRole
CMSDept, CMSUserInDept
CMSMenu, CMSMenus, CMSDisableMenu, CMSMenuLog
CMSCodes, CMSLang, CMSConfig
CMSEventLog

-- 檢視表
vw_CMSUser
vw_BAT_OneCMSUserInOneCMSDept
BAT_CMSMenu

-- 函數
RS_ACL_CMSMenu(@UserGuid)
RS_CMSCodes(@CodesType)
dbo.GetLang(@LangJson, @Language)
```

---

## 錯誤處理

所有 API 都遵循相同的錯誤處理模式：

### 成功回應
- `200 OK`: 有資料
- `204 No Content`: 無資料

### 錯誤回應
- `400 Bad Request`: 參數錯誤或業務邏輯錯誤
- `401 Unauthorized`: 未授權 (Token 無效)
- `500 Internal Server Error`: 伺服器錯誤

**錯誤格式**:
```json
"Exception message here"
```

---

## 安全性建議

1. **SQL Injection 防護**
   - ✅ 所有 API 都使用參數化查詢
   - ✅ 使用 `SqlCommand.Parameters.AddWithValue()`

2. **JWT 驗證**
   - ✅ 除了登入相關 API (`AllowAnonymous`)，所有 API 都需要 Token
   - ⚠️ 建議加入角色權限檢查

3. **輸入驗證**
   - ⚠️ 建議加入 Model Validation
   - ⚠️ 建議限制分頁大小 (PageSize <= 1000)

---

## 效能建議

1. **分頁查詢**
   - ✅ `GetUserList`, `GetEventLog` 已實作分頁
   - 📊 建議加入總筆數回傳

2. **快取策略**
   - `GetCMSCodes`: 建議快取 (代碼很少變動)
   - `GetDeptTree`: 建議快取 (部門結構穩定)
   - `GetRoles`: 建議快取 (角色清單固定)

3. **索引優化**
   - 確保 `CMSUser.UserGuid`, `UserId` 有索引
   - 確保 `CMSEventLog.CreatedDate` 有索引

---

## 待辦事項

### 🟡 優先級 P1 (建議補充)

- [ ] 使用者管理
  - [ ] 新增使用者 (`POST /CreateUser`)
  - [ ] 刪除使用者 (`POST /DeleteUser`)
  - [ ] 重設密碼 (`POST /ResetPassword`)

- [ ] 選單管理
  - [ ] 新增選單 (`POST /SaveMenu`)
  - [ ] 刪除選單 (`POST /DeleteMenu`)
  - [ ] 選單權限管理

- [ ] 部門管理
  - [ ] 新增/編輯部門 (`POST /SaveDept`)
  - [ ] 刪除部門 (`POST /DeleteDept`)
  - [ ] 調整部門成員

- [ ] 角色管理
  - [ ] 新增/編輯角色 (`POST /SaveRole`)
  - [ ] 刪除角色 (`POST /DeleteRole`)
  - [ ] 角色權限設定

---

## 📝 變更記錄

| 日期 | 版本 | 變更內容 | 作者 |
|------|------|----------|------|
| 2025-11-20 | 1.0 | 建立核心 CMS API (排除推播) | AI Assistant |

---

**文檔狀態**: 🟢 Complete  
**最後更新**: 2025-11-20  
**版本**: 1.0

