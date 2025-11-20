<template>
  <div class="side-nav-outer-toolbar">
    <HeaderToolbar
      class="layout-header"
      :menu-toggle-enabled="true"
      :toggle-menu-func="toggleMenu"
      :title="title"
    />

    <DxDrawer
      class="layout-body"
      position="before"
      template="menu"
      v-model:opened="menuOpened"
      :opened-state-mode="drawerOptions.menuMode"
      :reveal-mode="drawerOptions.menuRevealMode"
      :min-size="drawerOptions.minMenuSize"
      :shading="drawerOptions.shaderEnabled"
      :close-on-outside-click="drawerOptions.closeOnOutsideClick"
    >
      <DxScrollView
        ref="scrollViewRef"
        :bounce-enabled="false"
        :use-native="appInfo.isMobile ? false : appInfo.isNativeScrollBar"
        show-scrollbar="always"
        class="with-footer"
      >
        <slot />
        <slot name="footer" />
      </DxScrollView>

      <template #menu>
        <SideNavMenu
          :compact-mode="!menuOpened"
          @click="handleSideBarClick"
        />
      </template>
    </DxDrawer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, getCurrentInstance } from 'vue'
import { useRoute } from 'vue-router'
import DxDrawer from 'devextreme-vue/drawer'
import DxScrollView from 'devextreme-vue/scroll-view'
import HeaderToolbar from '../components/header-toolbar.vue'
import SideNavMenu from '../components/side-nav-menu.vue'
import { useScreenSize } from '../composables/useScreenSize'

// ============================================
// Props
// ============================================
interface Props {
  title?: string
}

const props = withDefaults(defineProps<Props>(), {
  title: ''
})

// ============================================
// Vue 實例
// ============================================
const { appContext } = getCurrentInstance()!
const route = useRoute()
const appInfo = appContext.config.globalProperties.$appInfo

// ============================================
// Composables
// ============================================
const { isXSmall, isLarge } = useScreenSize()

// ============================================
// 狀態變數
// ============================================
const menuOpened = ref(isLarge.value)
const menuTemporaryOpened = ref(false)
const scrollViewRef = ref<any>(null)

// ============================================
// 計算屬性
// ============================================
const drawerOptions = computed(() => {
  const shaderEnabled = !isLarge.value
  return {
    menuMode: isLarge.value ? 'shrink' : 'overlap',
    menuRevealMode: isXSmall.value ? 'slide' : 'expand',
    minMenuSize: isXSmall.value ? 0 : (appInfo.isMobile ? 0 : 60),
    menuOpened: isLarge.value,
    closeOnOutsideClick: shaderEnabled,
    shaderEnabled
  }
})

// ============================================
// 方法
// ============================================

/**
 * 切換選單
 */
const toggleMenu = (e: any) => {
  const pointerEvent = e.event
  pointerEvent.stopPropagation()
  
  if (menuOpened.value) {
    menuTemporaryOpened.value = false
  }
  menuOpened.value = !menuOpened.value
}

/**
 * 處理側邊欄點擊
 */
const handleSideBarClick = () => {
  if (menuOpened.value === false) {
    menuTemporaryOpened.value = true
  }
  menuOpened.value = true
}

// ============================================
// 監聽
// ============================================
watch(isLarge, (newVal) => {
  if (!menuTemporaryOpened.value) {
    menuOpened.value = newVal
  }
})

watch(() => route.path, () => {
  if (menuTemporaryOpened.value || !isLarge.value) {
    menuOpened.value = false
    menuTemporaryOpened.value = false
  }
  
  // 滾動到頂部
  scrollViewRef.value?.instance?.scrollTo(0)
})
</script>

<style lang="scss" scoped>
.side-nav-outer-toolbar {
  flex-direction: column;
  display: flex;
  height: 100%;
  width: 100%;
}

.layout-header {
  z-index: 1501;
}

.layout-body {
  flex: 1;
  min-height: 0;
}

.content {
  flex-grow: 1;
}

.with-footer {
  height: 100%;
}
</style>

