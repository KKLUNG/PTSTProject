<template>
  <div class="main-menu-container">
    <DxLoadPanel
      :hide-on-outside-click="false"
      :visible="loading"
      :shading="true"
      :show-pane="true"
      shading-color="transparent"
      message="Loading...please Wait"
    />
    
    <div class="content-block">
      <h2 class="page-title">{{ $appInfo.title }} - 主選單</h2>
      
      <div class="menu-tiles" v-if="menuItems.length > 0">
        <div 
          v-for="item in menuItems" 
          :key="item.MenuPGuid"
          class="menu-tile dx-card"
          @click="navigateToMenu(item)"
        >
          <div class="tile-icon">
            <i :class="item.MenuIcon"></i>
          </div>
          <div class="tile-title">
            {{ item.MenuTitle }}
          </div>
        </div>
      </div>
      
      <div v-else-if="!loading" class="no-menu">
        <p>沒有可用的選單項目</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, getCurrentInstance } from 'vue'
import { useRouter } from 'vue-router'
import DxLoadPanel from 'devextreme-vue/load-panel'
import { apiGet } from '../utils/api-util'

// ============================================
// Vue 實例和路由
// ============================================
const { appContext } = getCurrentInstance()!
const router = useRouter()

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
const menuItems = ref<any[]>([])

// ============================================
// 介面定義
// ============================================
interface MenuItem {
  MenuGuid: string
  MenuPGuid: string
  MenuTitle: string
  MenuIcon: string
}

// ============================================
// 方法
// ============================================

/**
 * 載入主菜單資料
 */
const loadMainMenu = async () => {
  loading.value = true
  try {
    const userGuid = appInfo.userInfo.userGuid || ''
    const language = appInfo.language || 'zhTW'
    if (!userGuid) {
      showAlert('使用者資訊不完整，請重新登入', appInfo.title)
      router.push('/CMSLogin')
      return
    }
    var para = {
      UserGuid: userGuid,
      Language: language
    }
    console.log('2');
    
    const response = await apiGet('/api/CMS/GetCMSMainMenu', para)
    console.log('3');
    if (response.status === 200 && response.data) {
      menuItems.value = (typeof response.data === 'string') 
        ? JSON.parse(response.data) 
        : response.data
      console.log('📋 CMSMainMenu: 載入菜單成功', menuItems.value)
    } else if (response.status === 204) {
      menuItems.value = []
      console.log('📋 CMSMainMenu: 沒有可用的菜單')
    }
  } catch (error) {
    console.error('❌ 載入主菜單失敗:', error)
    showAlert('載入選單失敗: ' + error, appInfo.title)
  } finally {
    loading.value = false
  }
}

/**
 * 導航到指定菜單
 */
const navigateToMenu = (item: MenuItem) => {
  console.log('🔗 導航到菜單:', item)
  
  if (item.MenuGuid) {
    // 導航到 CMSPage
    router.push(`/CMSPage/${item.MenuGuid}`)
  }
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  console.log('🚀 CMSMainMenu mounted')
  console.log('📦 appInfo:', appInfo)
  loadMainMenu()
})
</script>

<style scoped lang="scss">
.main-menu-container {
  padding: 20px;
  min-height: 100vh;
  background-color: #f5f5f5;
}

.page-title {
  text-align: center;
  margin-bottom: 30px;
  color: #333;
  font-weight: 500;
}

.menu-tiles {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 20px;
  max-width: 1200px;
  margin: 0 auto;
}

.menu-tile {
  cursor: pointer;
  padding: 30px;
  text-align: center;
  transition: all 0.3s ease;
  border-radius: 8px;
  background: white;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  
  &:hover {
    transform: translateY(-5px);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
    background-color: #f8f9fa;
  }
}

.tile-icon {
  font-size: 48px;
  margin-bottom: 15px;
  color: #337ab7;
  
  i {
    display: inline-block;
  }
}

.tile-title {
  font-size: 16px;
  font-weight: 500;
  color: #333;
  word-wrap: break-word;
}

.no-menu {
  text-align: center;
  padding: 50px;
  color: #999;
  
  p {
    font-size: 18px;
  }
}

// 響應式設計
@media (max-width: 768px) {
  .menu-tiles {
    grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
    gap: 15px;
  }
  
  .menu-tile {
    padding: 20px;
  }
  
  .tile-icon {
    font-size: 36px;
    margin-bottom: 10px;
  }
  
  .tile-title {
    font-size: 14px;
  }
}

@media (max-width: 480px) {
  .menu-tiles {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>

