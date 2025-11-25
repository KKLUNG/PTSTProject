<template>
  <div :style="cssCustom">
    <DxScrollView
      height="100%"
      width="100vw"
      class="hero"
      :style="styleBackground"
    >
      <HeaderToolbar
        class="layout-header"
        :menu-toggle-enabled="false"
        :toggle-menu-func="toggleMenu"
        :title="title"
      />
      <DxLoadPanel
        :hide-on-outside-click="false"
        :visible="loading"
        :shading="true"
        :show-pane="true"
        shading-color="transparent"
        message="loadingMessage"
      />
      <div class="showflex">
        <!-- 循環菜單 -->
        <div
          v-for="item in mainmenudata"
          :key="'MenuGuid' + item.MenuGuid"
          class="menubox"
          @click="() => onMenuClick(item.MenuGuid, item.MenuPGuid)"
        >
          <div style="height: 15%"></div>
          <div class="imgbox">
            <i :class="item.MenuIcon" :style="iconStyle" />
          </div>
          <div class="textbox">{{ item.MenuTitle }}</div>
        </div>
      </div>
    </DxScrollView>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, getCurrentInstance } from 'vue'
import { useRouter } from 'vue-router'
import DxScrollView from 'devextreme-vue/scroll-view'
import DxLoadPanel from 'devextreme-vue/load-panel'
import HeaderToolbar from '../components/header-toolbar.vue'
import { apiGet } from '../utils/api-util'

// ============================================
// Props
// ============================================
interface Props {
  title?: string
  isXSmall?: boolean
  isLarge?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  title: '',
  isXSmall: false,
  isLarge: false
})

// ============================================
// Vue 實例和路由
// ============================================
const { appContext } = getCurrentInstance()!
const router = useRouter()

// ============================================
// 全域屬性
// ============================================
const appInfo = appContext.config.globalProperties.$appInfo
const cssVariable = appContext.config.globalProperties.$cssVariable || {}
const showAlert = appContext.config.globalProperties.alert as (
  message: string, title?: string, buttonText?: string, f?: () => void
) => void

// ============================================
// 狀態變數
// ============================================
const mainmenudata = ref<any[]>([])
const loading = ref(false)

// ============================================
// 計算屬性
// ============================================
const cssCustom = computed(() => {
  return {
    '--textColor': cssVariable.baseTextColor || '#333',
    '--bgColor': cssVariable.baseBg || '#fff',
    '--mainColor': cssVariable.baseAccent || '#337ab7'
  }
})

const styleBackground = computed(() => {
  return {
    backgroundColor: cssVariable.baseBg || '#fff'
  }
})

const iconStyle = computed(() => {
  return {
    fontSize: '6rem',
    color: cssVariable.baseWebPartIcon || '#337ab7'
  }
})

// ============================================
// 方法
// ============================================

/**
 * 點擊菜單項目
 */
const onMenuClick = (MenuGuid: string, MenuPGuid: string) => {
  loading.value = true

  appInfo.rootGuid = MenuPGuid
  appInfo.homeGuid = MenuGuid

  // TODO: 如果使用 Vuex/Pinia，可以調用 store.dispatch("CREATE_MENUS", appInfo)
  
  router.push(`/CMSPage/${appInfo.homeGuid}`).catch(() => {})
}

/**
 * 切換選單（此佈局不需要）
 */
const toggleMenu = (e?: any) => {
  // 此佈局不支援選單切換
}

/**
 * 載入主菜單資料
 */
const loadMainMenu = async () => {
  loading.value = true
  
  try {
    const userGuid = appInfo.userInfo?.userGuid || ''
    const language = appInfo.language || 'zhTW'

    if (!userGuid) {
      showAlert('使用者資訊不完整，請重新登入', appInfo.title)
      router.push('/CMSLogin')
      return
    }

    // 檢查本地快取
    const cacheKey = `MainMenu_data`
    const cachedData = localStorage.getItem(cacheKey)
    
    if (cachedData) {
      mainmenudata.value = JSON.parse(cachedData)
      loading.value = false
      return
    }

    // 從 API 載入
    const para = {
      UserGuid: userGuid,
      Language: language
    }

    const response = await apiGet('/api/CMS/GetCMSMainMenu', para)

    if (response.status === 200 && response.data) {
      const data = typeof response.data === 'string' 
        ? JSON.parse(response.data) 
        : response.data
      
      // 儲存到快取
      localStorage.setItem(cacheKey, JSON.stringify(data))
      mainmenudata.value = data
    } else {
      showAlert('No record, please contact IT.', appInfo.title)
    }
  } catch (error) {
    console.error('❌ 載入主菜單失敗:', error)
    showAlert(String(error), appInfo.title)
  } finally {
    loading.value = false
  }
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  loadMainMenu()
})
</script>

<style lang="scss" scoped>
.hero {
  width: 100vw;
  height: 100vh;
  justify-content: center;
  align-items: center;
  background-size: cover;
  background-position: center center;
  background-repeat: no-repeat;
  background-attachment: fixed;
}

.showflex {
  width: 100vw;
  height: 100vh;
  display: flex;
  flex-wrap: wrap;
  flex-direction: row;
  align-items: center;
  justify-content: center;
}

.menubox {
  width: 200px;
  height: 200px;
  margin: 15px 30px 0px 30px;
  animation: animated-border 3s infinite;
  border-radius: 10px;
  background: linear-gradient(var(--bgColor), var(--mainColor));
  background-origin: border-box;
  background-color: var(--bgColor);
  cursor: pointer;
  transition: transform 0.2s ease;

  &:hover {
    transform: scale(1.05);
  }
}

.imgbox {
  width: 100%;
  height: 48%;
  font-weight: bold;
  color: var(--textColor);
  display: flex;
  align-items: center;
  justify-content: center;
}

.textbox {
  width: 100%;
  height: 25%;
  font-weight: bold;
  color: var(--textColor);
  font-size: 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
}

@keyframes animated-border {
  0% {
    box-shadow: 0 0 0 0 rgba(255, 255, 255, 0.4);
  }
  100% {
    box-shadow: 0 0 0 30px rgba(255, 255, 255, 0);
  }
}

.layout-header {
  z-index: 1501;
}
</style>

