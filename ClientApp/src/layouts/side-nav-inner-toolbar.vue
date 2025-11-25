<template>
  <div class="side-nav-inner-toolbar">
    <DxDrawer
      class="drawer"
      position="before"
      template="menu"
      v-model:opened="menuOpened"
      :opened-state-mode="drawerOptions.menuMode"
      :reveal-mode="drawerOptions.menuRevealMode"
      :min-size="drawerOptions.minMenuSize"
      :shading="drawerOptions.shaderEnabled"
      :hide-on-outside-click="drawerOptions.hideOnOutsideClick"
    >
      <div class="container">
        <HeaderToolbar
          :menu-toggle-enabled="headerMenuTogglerEnabled"
          :toggle-menu-func="toggleMenu"
        />
        <DxScrollView ref="scrollViewRef" class="layout-body with-footer">
          <slot />
          <slot name="footer" />
        </DxScrollView>
      </div>

      <template #menu>
        <SideNavMenu
          :compact-mode="!menuOpened"
          @click="handleSideBarClick"
        >
          <DxToolbar id="navigation-header">
            <DxItem v-if="!isXSmall" location="before" css-class="menu-button">
              <DxButton
                icon="menu"
                styling-mode="text"
                @click="toggleMenu"
              />
            </DxItem>
            <DxItem location="before" css-class="header-title dx-toolbar-label">
              <div>{{ title }}</div>
            </DxItem>
          </DxToolbar>
        </SideNavMenu>
      </template>
    </DxDrawer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, getCurrentInstance } from 'vue'
import { useRoute } from 'vue-router'
import DxDrawer from 'devextreme-vue/drawer'
import DxScrollView from 'devextreme-vue/scroll-view'
import DxToolbar, { DxItem } from 'devextreme-vue/toolbar'
import DxButton from 'devextreme-vue/button'
import HeaderToolbar from '../components/header-toolbar.vue'
import SideNavMenu from '../components/side-nav-menu.vue'
import { useScreenSize } from '../composables/useScreenSize'

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
// Vue 實例
// ============================================
const { appContext } = getCurrentInstance()!
const route = useRoute()

// ============================================
// Composables
// ============================================
const { isXSmall: screenIsXSmall, isLarge: screenIsLarge } = useScreenSize()

// 使用 props 或 screen size
const isXSmall = computed(() => props.isXSmall || screenIsXSmall.value)
const isLarge = computed(() => props.isLarge || screenIsLarge.value)

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
    minMenuSize: isXSmall.value ? 0 : 60,
    menuOpened: isLarge.value,
    hideOnOutsideClick: shaderEnabled,
    shaderEnabled
  }
})

const headerMenuTogglerEnabled = computed(() => {
  return isXSmall.value
})

const scrollView = computed(() => {
  return scrollViewRef.value?.instance
})

// ============================================
// 方法
// ============================================

/**
 * 切換選單
 */
const toggleMenu = (e?: any) => {
  if (e?.event) {
    e.event.stopPropagation()
  }
  
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
  scrollView.value?.scrollTo(0)
})
</script>

<style lang="scss" scoped>
.side-nav-inner-toolbar {
  width: 100%;
}

.container {
  height: 100%;
  flex-direction: column;
  display: flex;
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

#navigation-header {
  background-color: var(--base-accent, #337ab7);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12), 0 1px 2px rgba(0, 0, 0, 0.24);

  :deep(.menu-button .dx-icon) {
    color: var(--base-text-color, #333);
  }

  @media (max-width: 768px) {
    padding-left: 20px;
  }
}
</style>

