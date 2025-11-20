<template>
  <header class="header-component">
    <DxToolbar class="header-toolbar">
      <!-- 選單切換按鈕 -->
      <DxItem
        v-if="menuToggleEnabled"
        location="before"
        css-class="menu-button"
      >
        <template #default>
          <DxButton
            icon="menu"
            styling-mode="text"
            @click="toggleMenuFunc"
          />
        </template>
      </DxItem>

      <!-- Logo -->
      <DxItem
        v-if="title"
        location="before"
        css-class="header-title dx-toolbar-label"
      >
        <template #default>
          <img :src="logoUrl" width="120px" alt="Logo" />
        </template>
      </DxItem>

      <!-- 中間警語 (如果需要) -->
      <DxItem
        v-if="appInfo.isWarningOnHeader && !appInfo.isMobile"
        location="center"
        locate-in-menu="never"
      >
        <template #default>
          <div style="background-color: red; color: white; padding: 5px 10px">
            {{ warningMessage }}
          </div>
        </template>
      </DxItem>

      <!-- 通知鈴鐺 -->
      <DxItem location="after" locate-in-menu="never">
        <template #default>
          <div style="padding-top: 5px">
            <a v-show="hasMsg" class="notification" @click="onNotificationClick">
              <div class="icon-bell-o" style="margin: 3px; font-size: 1.2em"></div>
              <span v-show="hasMsg" :class="badgeClass">{{ badge }}</span>
            </a>
          </div>
        </template>
      </DxItem>

      <!-- 語言切換 -->
      <DxItem
        v-if="appInfo.isShowLang"
        location="after"
        locate-in-menu="never"
      >
        <template #default>
          <DxDropDownButton
            :items="langSet"
            :display-expr="appInfo.isMobile ? undefined : 'display'"
            :icon="appInfo.isMobile ? 'icon' : undefined"
            key-expr="value"
            styling-mode="text"
            :use-select-mode="true"
            :selected-item-key="currentLang"
            :drop-down-options="{ width: appInfo.isMobile ? '60px' : '120px' }"
            @selection-changed="onSelectedLang"
          />
        </template>
      </DxItem>

      <!-- 使用者面板 -->
      <DxItem
        location="after"
        locate-in-menu="never"
        menu-item-template="menuUserItem"
      >
        <template #default>
          <div style="display: flex">
            <div style="align-items: center">
              <DxButton
                class="user-button authorization"
                :width="userPanelWidth"
                height="100%"
                styling-mode="text"
              >
                <UserPanel :menu-items="userMenuItems" menu-mode="context" />
              </DxButton>
            </div>
          </div>
        </template>
      </DxItem>

      <!-- 使用者選單項目模板 -->
      <template #menuUserItem>
        <UserPanel :menu-items="userMenuItems" menu-mode="list" />
      </template>
    </DxToolbar>
  </header>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount, watch, getCurrentInstance } from 'vue'
import { useRouter } from 'vue-router'
import { useStore } from 'vuex'
import DxToolbar, { DxItem } from 'devextreme-vue/toolbar'
import DxButton from 'devextreme-vue/button'
import DxDropDownButton from 'devextreme-vue/drop-down-button'
import UserPanel from './user-panel.vue'
import auth from '../utils/auth'
import { langSet } from '../utils/constData'
import { apiGet } from '../utils/api-util'

// ============================================
// Props
// ============================================
interface Props {
  menuToggleEnabled?: boolean
  title?: string
  toggleMenuFunc?: () => void
  logOutFunc?: () => void
}

const props = withDefaults(defineProps<Props>(), {
  menuToggleEnabled: false,
  title: '',
  toggleMenuFunc: () => {},
  logOutFunc: () => {}
})

// ============================================
// Vue 實例
// ============================================
const { appContext } = getCurrentInstance()!
const router = useRouter()
const store = useStore()
const appInfo = appContext.config.globalProperties.$appInfo
const showAlert = appContext.config.globalProperties.alert as (
  message: string, title?: string
) => void
const confirm = appContext.config.globalProperties.confirm

// ============================================
// 狀態變數
// ============================================
const userMenuItems = ref<any[]>([])
const currentLang = ref(appInfo.language || 'zhTW')
const logoUrl = ref('/images/logo.png')
const badgeClass = ref('badge1')
const badgeTimer = ref<number | null>(null)
const badgeCounter = ref<number | null>(null)
const warningMessage = ref('系統警語')

// ============================================
// 計算屬性
// ============================================
const badge = computed(() => {
  return store.state.badge || 0
})

const hasMsg = computed(() => {
  return badge.value > 0
})

const userPanelWidth = computed(() => {
  return appInfo.isMobile ? 40 : 'inherit'
})

// ============================================
// 方法
// ============================================

/**
 * 載入使用者選單
 */
const loadUserMenu = async () => {
  try {
    const language = appInfo.language || 'zhTW'
    const response = await apiGet('/api/CMS/GetCMSMenuByUserId', {
      KeyParameter: '26D7E1E3-8F58-1D92-2D43-4989E2260523', // 使用者選單的 MenuGuid
      UserGuid: appInfo.userInfo.userGuid,
      Language: language,
      IgnoreACL: '0',
      DisplayMode: appInfo.isMobile ? '2' : '1'
    })

    if (response.status === 200) {
      const data = typeof response.data === 'string' ? JSON.parse(response.data) : response.data
      userMenuItems.value = []

      data.forEach((item: any) => {
        if (item.ACLSuccess === '1') {
          userMenuItems.value.push({
            visible: true,
            text: item.MenuTitle,
            icon: item.icon,
            path: item.Path,
            onClick: (e: any) => {
              router.push(e.itemData.path).catch(() => {})
            }
          })
        }
      })

      // 加入「返回主選單」
      if (appInfo.isShowMainMenu && router.currentRoute.value.path !== '/CMSMainMenu') {
        userMenuItems.value.push({
          visible: true,
          text: '返回主選單',
          icon: 'hidepanel',
          onClick: onBackMainMenuClick
        })
      }

      // 加入「登出」
      userMenuItems.value.push({
        visible: true,
        text: '登出',
        icon: 'runner',
        onClick: onLogoutClick
      })
    }
  } catch (err) {
    console.error('載入使用者選單失敗:', err)
  }
}

/**
 * 語言切換
 */
const onSelectedLang = (e: any) => {
  if (e.item.value !== appInfo.language) {
    appInfo.language = e.item.value
    location.reload()
  }
}

/**
 * 登出
 */
const onLogoutClick = () => {
  auth.logOut()
  window.sessionStorage.setItem('routerCurrentPath', '')
  router.push('/CMSLogin').catch(() => {})
}

/**
 * 返回主選單
 */
const onBackMainMenuClick = () => {
  router.push('/CMSMainMenu').catch(() => {})
}

/**
 * 通知點擊
 */
const onNotificationClick = () => {
  // 導航到通知頁面
  const url = appInfo.isMobile
    ? '/CMSFrame/我的通知/AdminGridForm/3/Code_CMSNotifyAndWFSMsg.xml'
    : '/CMSFrame/AdminGridForm/3/Code_CMSNotifyAndWFSMsg.xml'
  
  router.push(url).catch(() => {})
}

/**
 * 更新 Badge 樣式
 */
const updateBadgeClass = () => {
  if (badge.value >= 100) {
    badgeClass.value = 'badge3'
  } else if (badge.value >= 10 && badge.value < 100) {
    badgeClass.value = 'badge2'
  } else {
    badgeClass.value = 'badge1'
  }
}

// ============================================
// 生命週期
// ============================================
onMounted(async () => {
  currentLang.value = appInfo.language || 'zhTW'

  // 載入選單
  await store.dispatch('CREATE_MENUS', appInfo)
  await loadUserMenu()

  // 設定 Badge 自動更新 (如果有設定)
  const autoRefreshBadge = 60 // 預設 60 秒，可從系統設定讀取
  if (autoRefreshBadge > 0) {
    badgeTimer.value = window.setInterval(() => {
      if (auth.authenticated()) {
        store.dispatch('CREATE_BADGE', {
          UserGuid: appInfo.userInfo.userGuid
        })
      }
    }, autoRefreshBadge * 1000)
  }

  // 初始載入 Badge
  store.dispatch('CREATE_BADGE', {
    UserGuid: appInfo.userInfo.userGuid
  })
})

onBeforeUnmount(() => {
  if (badgeTimer.value) {
    clearInterval(badgeTimer.value)
  }
})

// ============================================
// 監聽
// ============================================
watch(badge, (newVal) => {
  updateBadgeClass()

  if (badgeCounter.value === null) {
    badgeCounter.value = newVal
  } else {
    // 如果有新訊息，可以彈出提示
    if (badgeCounter.value < newVal) {
      // 可選：顯示新訊息提示
      console.log('📬 有新訊息!')
    }
    badgeCounter.value = newVal
  }
})
</script>

<style lang="scss" scoped>
.header-component {
  display: flex;
  align-items: center;
  z-index: 1;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12), 0 1px 2px rgba(0, 0, 0, 0.24);
  background-color: #fff;
  height: 35px;
}

:deep(.dx-toolbar.header-toolbar) {
  padding-left: 10px;
  text-align: center;

  .dx-toolbar-items-container .dx-toolbar-after {
    padding: 0px 40px;

    @media (max-width: 768px) {
      padding: 0px 20px;
    }
  }

  .dx-toolbar-item.dx-toolbar-button.menu-button {
    width: 40px;
    text-align: center;
    padding: 0;
  }

  .dx-toolbar-item.menu-button > .dx-toolbar-item-content .dx-icon {
    color: #337ab7;
  }
}

.header-title :deep(.dx-item-content) {
  padding: 0;
  margin: 0;
}

:deep(.user-button > .dx-button-content) {
  padding: 3px;
}

a {
  text-decoration: none;
}

.notification {
  text-decoration: none;
  padding: 0px 20px;
  position: relative;
  display: inline-block;
  margin-right: 0px;
  border-radius: 2px;
  cursor: pointer;
}

.notification .badge1 {
  font-size: 12px;
  width: 20px;
  height: 20px;
  position: absolute;
  top: 0px;
  right: 5px;
  padding: 3px 7px;
  border-radius: 100%;
  background: red;
  color: white;
}

.notification .badge2 {
  font-size: 12px;
  width: 20px;
  height: 20px;
  position: absolute;
  top: 0px;
  right: 5px;
  padding: 3px 4px;
  border-radius: 100%;
  background: red;
  color: white;
}

.notification .badge3 {
  font-size: 12px;
  width: 20px;
  height: 20px;
  position: absolute;
  top: 0px;
  right: 5px;
  padding: 3px 0px;
  border-radius: 100%;
  background: red;
  color: white;
}
</style>

