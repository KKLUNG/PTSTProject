<template>
  <div class="cms-page-container">
    <DxLoadPanel
      :hide-on-outside-click="false"
      :visible="loading"
      :shading="true"
      :show-pane="true"
      shading-color="transparent"
      message="Loading...please Wait"
    />
    
    <div class="content-block">
      <h2 class="page-title">{{ menuTitle }}</h2>
      
      <!-- Top Section -->
      <div class="row" v-if="topItems.length > 0">
        <div class="col-12">
          <div class="dx-card" v-for="item in topItems" :key="item.MenusGuid">
            <component
              :is="getComponentName(item.MenusControl)"
              :xml-name="item.MenusXMLName"
              :xml-parameter="item.MenusParameter"
              :xml-caption="getFieldCaption(item.MenusTitle, item.MenusTitleLang)"
              :page-key="pageKey"
            />
          </div>
        </div>
      </div>
      
      <!-- Main Content (Left/Center/Right) -->
      <div class="row">
        <div :class="leftColClass" v-if="leftItems.length > 0">
          <div class="dx-card" v-for="item in leftItems" :key="item.MenusGuid">
            <component
              :is="getComponentName(item.MenusControl)"
              :xml-name="item.MenusXMLName"
              :xml-parameter="item.MenusParameter"
              :xml-caption="getFieldCaption(item.MenusTitle, item.MenusTitleLang)"
              :page-key="pageKey"
            />
          </div>
        </div>
        
        <div :class="centerColClass" v-if="centerItems.length > 0">
          <div class="dx-card" v-for="item in centerItems" :key="item.MenusGuid">
            <component
              :is="getComponentName(item.MenusControl)"
              :xml-name="item.MenusXMLName"
              :xml-parameter="item.MenusParameter"
              :xml-caption="getFieldCaption(item.MenusTitle, item.MenusTitleLang)"
              :page-key="pageKey"
            />
          </div>
        </div>
        
        <div :class="rightColClass" v-if="rightItems.length > 0">
          <div class="dx-card" v-for="item in rightItems" :key="item.MenusGuid">
            <component
              :is="getComponentName(item.MenusControl)"
              :xml-name="item.MenusXMLName"
              :xml-parameter="item.MenusParameter"
              :xml-caption="getFieldCaption(item.MenusTitle, item.MenusTitleLang)"
              :page-key="pageKey"
            />
          </div>
        </div>
      </div>
      
      <!-- Bottom Section -->
      <div class="row" v-if="bottomItems.length > 0">
        <div class="col-12">
          <div class="dx-card" v-for="item in bottomItems" :key="item.MenusGuid">
            <component
              :is="getComponentName(item.MenusControl)"
              :xml-name="item.MenusXMLName"
              :xml-parameter="item.MenusParameter"
              :xml-caption="getFieldCaption(item.MenusTitle, item.MenusTitleLang)"
              :page-key="pageKey"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, getCurrentInstance } from 'vue'
import { useRoute } from 'vue-router'
import DxLoadPanel from 'devextreme-vue/load-panel'
import { apiGet } from '../utils/api-util'
import AdminFormView from '../components/AdminFormView.vue'
import AdminGridForm from '../components/AdminGridForm.vue'
import AdminTab from '../components/AdminTab.vue'

// ============================================
// Vue 實例和路由
// ============================================
const { appContext } = getCurrentInstance()!
const route = useRoute()

// ============================================
// 全域屬性
// ============================================
const appInfo = appContext.config.globalProperties.$appInfo
const showAlert = appContext.config.globalProperties.alert as (
  message: string, title?: string, buttonText?: string, f?: () => void
) => void

// ============================================
// 狀態變數
// ============================================
const loading = ref(false)
const menuTitle = ref('')
const menuGuid = ref('')
const menuData = ref<any[]>([])
const pageKey = ref(generateGuid())

// 位置分組
const topItems = ref<any[]>([])
const leftItems = ref<any[]>([])
const centerItems = ref<any[]>([])
const rightItems = ref<any[]>([])
const bottomItems = ref<any[]>([])

// ============================================
// 計算屬性 - 響應式欄位寬度
// ============================================
const leftColClass = computed(() => {
  const hasLeft = leftItems.value.length > 0
  const hasCenter = centerItems.value.length > 0
  const hasRight = rightItems.value.length > 0
  
  if (hasLeft && hasCenter && hasRight) return 'col-4'
  if (hasLeft && hasCenter) return 'col-4'
  if (hasLeft && hasRight) return 'col-6'
  if (hasLeft) return 'col-12'
  return ''
})

const centerColClass = computed(() => {
  const hasLeft = leftItems.value.length > 0
  const hasCenter = centerItems.value.length > 0
  const hasRight = rightItems.value.length > 0
  
  if (hasLeft && hasCenter && hasRight) return 'col-4'
  if (hasCenter && hasRight) return 'col-8'
  if (hasLeft && hasCenter) return 'col-8'
  if (hasCenter) return 'col-12'
  return ''
})

const rightColClass = computed(() => {
  const hasLeft = leftItems.value.length > 0
  const hasCenter = centerItems.value.length > 0
  const hasRight = rightItems.value.length > 0
  
  if (hasLeft && hasCenter && hasRight) return 'col-4'
  if (hasCenter && hasRight) return 'col-4'
  if (hasLeft && hasRight) return 'col-6'
  if (hasRight) return 'col-12'
  return ''
})

// ============================================
// 方法
// ============================================

/**
 * 生成 GUID
 */
function generateGuid(): string {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    const r = Math.random() * 16 | 0
    const v = c === 'x' ? r : (r & 0x3 | 0x8)
    return v.toString(16)
  })
}

/**
 * 取得多語言標題
 */
function getFieldCaption(title: string, langJson: string): string {
  try {
    if (!langJson) return title
    
    const language = appInfo.language || 'zhTW'
    const langDict = JSON.parse(langJson)
    if (langDict && langDict[language]) {
      return langDict[language]
    }
  } catch {
    // 解析失敗，返回原始標題
  }
  return title
}

/**
 * 動態載入元件名稱
 * 將後端回傳的控制元件名稱轉換為前端元件名稱
 */
function getComponentName(controlName: string): string {
  // 根據後端回傳的控件名稱，返回對應的前端組件
  const componentMap: Record<string, any> = {
    'AdminFormView': AdminFormView,
    'AdminGridForm': AdminGridForm,
    'AdminTab': AdminTab,
    // 未來可以繼續加入其他組件
    // 'AdminTreeList': AdminTreeList,
    // 'AdminCalendar': AdminCalendar,
    // 等等...
  }

  return componentMap[controlName] || 'div'
}

/**
 * 載入選單項目資料
 */
async function loadMenuItems() {
  loading.value = true
  
  try {
    menuGuid.value = route.params.menuGuid as string
    
    if (!menuGuid.value) {
      showAlert('選單 GUID 不存在', appInfo.title)
      return
    }
    
    // 從 sessionStorage 檢查快取
    const cacheKey = `${menuGuid.value}_menu`
    const cachedData = window.sessionStorage.getItem(cacheKey)
    
    if (cachedData) {
      const cachedResponse = JSON.parse(cachedData)
      processMenuData(cachedResponse.data)
      return
    }
    
    // 呼叫 API 取得選單項目
    const para = {
      MenuGuid: menuGuid.value
    }
    
    const response = await apiGet('/api/CMS/GetCMSMenus', para)
    
    if (response.status === 200 && response.data) {
      // 儲存到 sessionStorage
      window.sessionStorage.setItem(cacheKey, JSON.stringify(response))
      
      // response.data 可能是字串或物件
      const data = typeof response.data === 'string' 
        ? JSON.parse(response.data) 
        : response.data
      
      processMenuData(data)
    } else if (response.status === 204) {
      showAlert('此選單沒有可用的項目', appInfo.title)
    }
  } catch (error) {
    console.error('❌ 載入選單項目失敗:', error)
    showAlert('載入選單項目失敗: ' + error, appInfo.title)
  } finally {
    loading.value = false
  }
}

/**
 * 處理選單資料，依照位置分組
 */
function processMenuData(data: any[]) {
  menuData.value = data
  
  if (data && data.length > 0) {
    menuTitle.value = data[0].MenuTitle || '頁面'
    
    // 依照 MenusPosition 分組
    // 1: Top, 2: Left, 3: Center, 4: Right, 5: Bottom
    topItems.value = data.filter(item => item.MenusPosition === '1')
    leftItems.value = data.filter(item => item.MenusPosition === '2')
    centerItems.value = data.filter(item => item.MenusPosition === '3')
    rightItems.value = data.filter(item => item.MenusPosition === '4')
    bottomItems.value = data.filter(item => item.MenusPosition === '5')
    
    console.log('📋 CMSPage: 載入選單項目成功', {
      top: topItems.value.length,
      left: leftItems.value.length,
      center: centerItems.value.length,
      right: rightItems.value.length,
      bottom: bottomItems.value.length
    })
  }
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  console.log('🚀 CMSPage mounted')
  console.log('📦 MenuGuid:', route.params.menuGuid)
  loadMenuItems()
})
</script>

<style scoped lang="scss">
.cms-page-container {
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

.row {
  display: flex;
  flex-wrap: wrap;
  margin: 0 -10px;
}

.col-4,
.col-6,
.col-8,
.col-12 {
  padding: 0 10px;
  margin-bottom: 20px;
}

.col-4 {
  flex: 0 0 33.333333%;
  max-width: 33.333333%;
}

.col-6 {
  flex: 0 0 50%;
  max-width: 50%;
}

.col-8 {
  flex: 0 0 66.666667%;
  max-width: 66.666667%;
}

.col-12 {
  flex: 0 0 100%;
  max-width: 100%;
}

.dx-card {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  margin-bottom: 15px;
}

// 響應式設計
@media (max-width: 768px) {
  .col-4,
  .col-6,
  .col-8 {
    flex: 0 0 100%;
    max-width: 100%;
  }
}
</style>

