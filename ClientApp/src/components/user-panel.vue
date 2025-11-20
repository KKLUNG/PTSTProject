<template>
  <div class="user-panel">
    <div class="user-info">
      <div class="image-container">
        <div
          class="user-image"
          :style="{ backgroundImage: 'url(' + imageUrl + ')' }"
        />
      </div>
      <div v-if="!appInfo.isMobile" class="user-name">
        <span>{{ appInfo.userInfo.userName }}</span>
        <span class="clock-span">
          <span class="clock-font">{{ currentDateTime }}</span>
        </span>
      </div>
    </div>

    <DxContextMenu
      v-if="menuMode === 'context'"
      target=".user-button"
      :items="menuItems"
      :width="contextMenuWidth"
      show-event="dxclick"
      css-class="user-menu"
    >
      <DxPosition my="top center" at="bottom center" />
    </DxContextMenu>

    <DxList
      v-if="menuMode === 'list'"
      class="dx-toolbar-menu-action"
      :items="menuItems"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount, getCurrentInstance } from 'vue'
import DxContextMenu, { DxPosition } from 'devextreme-vue/context-menu'
import DxList from 'devextreme-vue/list'

// ============================================
// Props
// ============================================
interface Props {
  menuMode?: 'context' | 'list'
  menuItems?: any[]
}

const props = withDefaults(defineProps<Props>(), {
  menuMode: 'context',
  menuItems: () => []
})

// ============================================
// Vue 實例
// ============================================
const { appContext } = getCurrentInstance()!
const appInfo = appContext.config.globalProperties.$appInfo

// ============================================
// 狀態變數
// ============================================
const currentDateTime = ref('')
const timer = ref<number | null>(null)

// ============================================
// 計算屬性
// ============================================
const imageUrl = computed(() => {
  const userImageUrl = appInfo.userInfo.userImageUrl || '/images/default-user.png'
  return appInfo.serverUrl + userImageUrl
})

const contextMenuWidth = computed(() => {
  return appInfo.isMobile ? 40 : 240
})

// ============================================
// 方法
// ============================================

/**
 * 建立時鐘
 */
const createClock = () => {
  const updateTime = () => {
    const cd = new Date()
    const month = zeroPadding(cd.getMonth() + 1, 2)
    const date = zeroPadding(cd.getDate(), 2)
    const hours = zeroPadding(cd.getHours(), 2)
    const minutes = zeroPadding(cd.getMinutes(), 2)
    const seconds = zeroPadding(cd.getSeconds(), 2)
    
    currentDateTime.value = `${month}-${date} ${hours}:${minutes}:${seconds}`
  }

  const zeroPadding = (num: number, digit: number): string => {
    let zero = ''
    for (let i = 0; i < digit; i++) {
      zero += '0'
    }
    return (zero + num).slice(-digit)
  }

  updateTime()
  timer.value = window.setInterval(updateTime, 1000)
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  if (!appInfo.isMobile) {
    createClock()
  }
})

onBeforeUnmount(() => {
  if (timer.value) {
    clearInterval(timer.value)
  }
})
</script>

<style lang="scss" scoped>
@import "../css/dx-styles.scss";

.user-info {
  display: flex;
  align-items: center;

  .image-container {
    align-items: center;
    overflow: hidden;
    border-radius: 50%;
    height: 25px;
    width: 25px;
    margin-top: 0px;
    padding-bottom: 0px;
    
    .user-image {
      width: 100%;
      height: 100%;
      background: no-repeat #fff;
      background-size: cover;
      background-position: center center;
      background-repeat: no-repeat;
    }
  }

  .user-name {
    align-items: center;
    font-size: 14px;
    color: #333;
    margin: 0px 0px 0px 9px;
  }
}

.user-panel {
  :deep(.dx-list-item .dx-icon) {
    vertical-align: middle;
    color: #333;
    margin-right: 16px;
  }
}

:deep(.dx-context-menu.user-menu.dx-menu-base) {
  .dx-submenu .dx-menu-items-container .dx-icon {
    margin-right: 16px;
  }
  .dx-menu-item .dx-menu-item-content {
    padding: 3px 15px 4px;
  }
}

.clock-font {
  color: #333;
}

.clock-span {
  border-radius: 15px;
  background-color: #f0f0f0;
  margin-left: 10px;
  padding: 5px;
}
</style>

