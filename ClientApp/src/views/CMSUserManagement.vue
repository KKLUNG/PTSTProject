<template>
  <div class="user-management-container">
    <DxLoadPanel
      :visible="loading"
      :shading="true"
      :show-pane="true"
      shading-color="transparent"
      message="載入中..."
    />

    <div class="content-block">
      <h2 class="page-title">使用者管理</h2>

      <!-- 搜尋列 -->
      <div class="dx-card search-panel">
        <div class="search-row">
          <DxTextBox
            v-model:value="searchKeyword"
            placeholder="搜尋使用者 ID、姓名或 Email"
            width="300px"
            @enter-key="handleSearch"
          >
            <DxButton
              name="search"
              location="after"
              icon="search"
              @click="handleSearch"
            />
          </DxTextBox>

          <DxSelectBox
            v-model:value="selectedDept"
            :data-source="deptList"
            display-expr="DeptName"
            value-expr="DeptGuid"
            placeholder="選擇部門篩選"
            width="200px"
            :show-clear-button="true"
            @value-changed="handleSearch"
          />

          <DxButton
            text="重新整理"
            icon="refresh"
            @click="loadUsers"
          />
        </div>
      </div>

      <!-- 使用者列表 -->
      <div class="dx-card">
        <DxDataGrid
          :data-source="users"
          :show-borders="true"
          :column-auto-width="true"
          :allow-column-resizing="true"
          @row-dbl-click="handleRowDblClick"
        >
          <DxPaging :page-size="pageSize" />
          <DxPager
            :show-page-size-selector="true"
            :allowed-page-sizes="[10, 20, 50, 100]"
            :show-info="true"
          />
          <DxSearchPanel :visible="false" />
          <DxHeaderFilter :visible="true" />
          <DxSelection mode="single" />

          <DxColumn data-field="UserId" caption="帳號" :width="120" />
          <DxColumn data-field="UserName" caption="姓名" :width="150" />
          <DxColumn data-field="UserTitle" caption="職稱" :width="120" />
          <DxColumn data-field="UserEmail" caption="Email" :width="200" />
          <DxColumn data-field="UserCellPhone" caption="手機" :width="120" />
          <DxColumn 
            data-field="LastActiveDate" 
            caption="最後登入" 
            :width="160"
            data-type="datetime"
            format="yyyy-MM-dd HH:mm"
          />
          <DxColumn
            data-field="IsActive"
            caption="狀態"
            :width="80"
            cell-template="statusTemplate"
          />
          
          <template #statusTemplate="{ data }">
            <span :class="data.value ? 'status-active' : 'status-inactive'">
              {{ data.value ? '啟用' : '停用' }}
            </span>
          </template>

          <DxColumn
            caption="操作"
            :width="150"
            cell-template="actionTemplate"
          />

          <template #actionTemplate="{ data }">
            <div class="action-buttons">
              <DxButton
                icon="edit"
                hint="編輯"
                @click="handleEdit(data.data)"
              />
              <DxButton
                icon="group"
                hint="角色管理"
                @click="handleManageRoles(data.data)"
              />
            </div>
          </template>
        </DxDataGrid>
      </div>
    </div>

    <!-- 編輯使用者對話框 -->
    <DxPopup
      v-model:visible="editPopupVisible"
      :drag-enabled="true"
      :close-on-outside-click="false"
      :show-title="true"
      :width="500"
      :height="400"
      title="編輯使用者資料"
    >
      <div class="edit-form">
        <DxForm :form-data="editingUser">
          <DxSimpleItem data-field="UserId" :editor-options="{ disabled: true }">
            <DxLabel text="帳號" />
          </DxSimpleItem>
          <DxSimpleItem data-field="UserName">
            <DxLabel text="姓名" />
            <DxRequiredRule message="姓名不可為空" />
          </DxSimpleItem>
          <DxSimpleItem data-field="UserTitle">
            <DxLabel text="職稱" />
          </DxSimpleItem>
          <DxSimpleItem data-field="UserEmail" editor-type="dxTextBox">
            <DxLabel text="Email" />
            <DxEmailRule message="Email 格式不正確" />
          </DxSimpleItem>
          <DxSimpleItem data-field="UserCellPhone">
            <DxLabel text="手機" />
          </DxSimpleItem>
        </DxForm>
      </div>

      <DxToolbarItem
        widget="dxButton"
        toolbar="bottom"
        location="after"
        :options="{
          text: '儲存',
          type: 'success',
          onClick: handleSave
        }"
      />
      <DxToolbarItem
        widget="dxButton"
        toolbar="bottom"
        location="after"
        :options="{
          text: '取消',
          onClick: () => editPopupVisible = false
        }"
      />
    </DxPopup>

    <!-- 角色管理對話框 -->
    <DxPopup
      v-model:visible="rolePopupVisible"
      :drag-enabled="true"
      :close-on-outside-click="false"
      :show-title="true"
      :width="600"
      :height="500"
      :title="`角色管理 - ${currentUser?.UserName || ''}`"
    >
      <div class="role-management">
        <div class="role-section">
          <h4>已擁有的角色</h4>
          <DxList
            :data-source="userRoles"
            :height="150"
            display-expr="RoleName"
            key-expr="RoleGuid"
          >
            <template #item="{ data }">
              <div class="role-item">
                <span>{{ data.RoleName }} - {{ data.RoleDesc }}</span>
                <DxButton
                  icon="remove"
                  styling-mode="text"
                  @click="handleRemoveRole(data.RoleGuid)"
                />
              </div>
            </template>
          </DxList>
        </div>

        <div class="role-section">
          <h4>可分配的角色</h4>
          <DxList
            :data-source="availableRoles"
            :height="200"
            display-expr="RoleName"
            key-expr="RoleGuid"
          >
            <template #item="{ data }">
              <div class="role-item">
                <span>{{ data.RoleName }} - {{ data.RoleDesc }}</span>
                <DxButton
                  icon="add"
                  styling-mode="text"
                  @click="handleAssignRole(data.RoleGuid)"
                />
              </div>
            </template>
          </DxList>
        </div>
      </div>

      <DxToolbarItem
        widget="dxButton"
        toolbar="bottom"
        location="after"
        :options="{
          text: '關閉',
          onClick: () => rolePopupVisible = false
        }"
      />
    </DxPopup>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed, getCurrentInstance } from 'vue'
import DxLoadPanel from 'devextreme-vue/load-panel'
import DxDataGrid, { 
  DxColumn, 
  DxPaging, 
  DxPager, 
  DxSearchPanel, 
  DxHeaderFilter,
  DxSelection 
} from 'devextreme-vue/data-grid'
import DxTextBox from 'devextreme-vue/text-box'
import DxSelectBox from 'devextreme-vue/select-box'
import DxButton from 'devextreme-vue/button'
import DxPopup, { DxToolbarItem } from 'devextreme-vue/popup'
import DxForm, { DxSimpleItem, DxLabel, DxRequiredRule, DxEmailRule } from 'devextreme-vue/form'
import DxList from 'devextreme-vue/list'
import { useCMS } from '../composables/useCMS'

// ============================================
// Vue 實例和全域屬性
// ============================================
const { appContext } = getCurrentInstance()!
const appInfo = appContext.config.globalProperties.$appInfo
const showAlert = appContext.config.globalProperties.alert as (
  message: string, title?: string
) => void

// ============================================
// Composables
// ============================================
const { 
  loading,
  getUserList,
  updateUserProfile,
  getDeptTree,
  getRoles,
  getUserRoles,
  assignUserRole,
  removeUserRole
} = useCMS()

// ============================================
// 狀態變數
// ============================================
const users = ref<any[]>([])
const deptList = ref<any[]>([])
const allRoles = ref<any[]>([])
const userRoles = ref<any[]>([])
const searchKeyword = ref('')
const selectedDept = ref('')
const pageSize = ref(50)

const editPopupVisible = ref(false)
const rolePopupVisible = ref(false)
const editingUser = ref<any>({})
const currentUser = ref<any>(null)

// ============================================
// 計算屬性
// ============================================
const availableRoles = computed(() => {
  const userRoleGuids = userRoles.value.map(r => r.RoleGuid)
  return allRoles.value.filter(r => !userRoleGuids.includes(r.RoleGuid))
})

// ============================================
// 方法
// ============================================

/**
 * 載入使用者列表
 */
const loadUsers = async () => {
  try {
    const result = await getUserList({
      DeptGuid: selectedDept.value,
      Keyword: searchKeyword.value,
      PageSize: pageSize.value,
      PageIndex: 1
    })
    users.value = result || []
  } catch (error) {
    console.error('載入使用者失敗:', error)
    showAlert('載入使用者失敗', appInfo.title)
  }
}

/**
 * 載入部門列表
 */
const loadDepts = async () => {
  try {
    const tree = await getDeptTree()
    // 扁平化樹狀結構
    const flattenDepts = (items: any[]) => {
      let result: any[] = []
      items.forEach(item => {
        result.push(item)
        if (item.children && item.children.length > 0) {
          result = result.concat(flattenDepts(item.children))
        }
      })
      return result
    }
    deptList.value = flattenDepts(tree)
  } catch (error) {
    console.error('載入部門失敗:', error)
  }
}

/**
 * 載入所有角色
 */
const loadRoles = async () => {
  try {
    allRoles.value = await getRoles()
  } catch (error) {
    console.error('載入角色失敗:', error)
  }
}

/**
 * 搜尋
 */
const handleSearch = () => {
  loadUsers()
}

/**
 * 雙擊行
 */
const handleRowDblClick = (e: any) => {
  handleEdit(e.data)
}

/**
 * 編輯使用者
 */
const handleEdit = (user: any) => {
  editingUser.value = { ...user }
  editPopupVisible.value = true
}

/**
 * 儲存使用者
 */
const handleSave = async () => {
  try {
    await updateUserProfile({
      UserGuid: editingUser.value.UserGuid,
      UserName: editingUser.value.UserName,
      UserTitle: editingUser.value.UserTitle || '',
      UserEmail: editingUser.value.UserEmail || '',
      UserCellPhone: editingUser.value.UserCellPhone || ''
    })
    
    showAlert('更新成功', appInfo.title)
    editPopupVisible.value = false
    loadUsers()
  } catch (error) {
    console.error('更新失敗:', error)
    showAlert('更新失敗', appInfo.title)
  }
}

/**
 * 管理角色
 */
const handleManageRoles = async (user: any) => {
  currentUser.value = user
  try {
    userRoles.value = await getUserRoles(user.UserGuid)
    rolePopupVisible.value = true
  } catch (error) {
    console.error('載入使用者角色失敗:', error)
    showAlert('載入使用者角色失敗', appInfo.title)
  }
}

/**
 * 分配角色
 */
const handleAssignRole = async (roleGuid: string) => {
  try {
    const result = await assignUserRole(currentUser.value.UserGuid, roleGuid)
    if (result && result.success) {
      showAlert('角色分配成功', appInfo.title)
      userRoles.value = await getUserRoles(currentUser.value.UserGuid)
    }
  } catch (error) {
    console.error('分配角色失敗:', error)
    showAlert('分配角色失敗', appInfo.title)
  }
}

/**
 * 移除角色
 */
const handleRemoveRole = async (roleGuid: string) => {
  try {
    const result = await removeUserRole(currentUser.value.UserGuid, roleGuid)
    if (result && result.success) {
      showAlert('角色移除成功', appInfo.title)
      userRoles.value = await getUserRoles(currentUser.value.UserGuid)
    }
  } catch (error) {
    console.error('移除角色失敗:', error)
    showAlert('移除角色失敗', appInfo.title)
  }
}

// ============================================
// 生命週期
// ============================================
onMounted(async () => {
  console.log('🚀 CMSUserManagement mounted')
  await Promise.all([
    loadUsers(),
    loadDepts(),
    loadRoles()
  ])
})
</script>

<style scoped lang="scss">
.user-management-container {
  padding: 20px;
  min-height: 100vh;
  background-color: #f5f5f5;
}

.page-title {
  text-align: center;
  margin-bottom: 20px;
  color: #333;
  font-weight: 500;
}

.content-block {
  max-width: 1400px;
  margin: 0 auto;
}

.search-panel {
  padding: 15px;
  margin-bottom: 20px;
}

.search-row {
  display: flex;
  gap: 10px;
  align-items: center;
}

.status-active {
  color: #28a745;
  font-weight: 500;
}

.status-inactive {
  color: #dc3545;
  font-weight: 500;
}

.action-buttons {
  display: flex;
  gap: 5px;
}

.edit-form {
  padding: 20px;
}

.role-management {
  padding: 20px;
}

.role-section {
  margin-bottom: 20px;

  h4 {
    margin-bottom: 10px;
    color: #333;
  }
}

.role-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 5px 0;
}
</style>

