<template>
  <div class="dx-swatch-additional side-navigation-menu" @click="forwardClick">
    <slot />
    <div class="menu-container">
      <DxTreeView
        ref="treeViewRef"
        :items="menus"
        key-expr="Path"
        display-expr="MenuTitle"
        selection-mode="single"
        :focus-state-enabled="false"
        expand-event="click"
        @item-click="handleItemClick"
        width="100%"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch, getCurrentInstance } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useStore } from 'vuex'
import DxTreeView from 'devextreme-vue/tree-view'
import type TreeView from 'devextreme/ui/tree_view'

// ============================================
// Props
// ============================================
interface Props {
  compactMode?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  compactMode: false
})

// ============================================
// Emits
// ============================================
const emit = defineEmits(['click'])

// ============================================
// Vue 實例
// ============================================
const router = useRouter()
const route = useRoute()
const store = useStore()

// ============================================
// 狀態變數
// ============================================
const treeViewRef = ref<{ instance: TreeView } | null>(null)

// ============================================
// 計算屬性
// ============================================
const menus = computed(() => {
  return store.state.menus || []
})

// ============================================
// 方法
// ============================================

/**
 * 轉發點擊事件
 */
const forwardClick = (...args: any[]) => {
  emit('click', ...args)
}

/**
 * 處理選單項目點擊
 */
const handleItemClick = (e: any) => {
  // 如果有外部連結，在新視窗開啟
  if (e.itemData.MenuSelfUrl && e.itemData.MenuSelfUrl !== '') {
    window.open(e.itemData.MenuSelfUrl, '_blank')
    return
  }

  // 如果有子項目且在緊湊模式，不導航
  if ((!e.itemData.Path || props.compactMode) && e.node.children && e.node.children.length > 0) {
    return
  }

  // 儲存選單標題
  window.sessionStorage.setItem('menuTitle', e.itemData.MenuTitle || '')
  
  // 導航到對應路徑
  if (e.itemData.Path) {
    router.push(e.itemData.Path).catch(() => {})
  }

  // 停止事件傳播
  if (e.event) {
    e.event.stopPropagation()
  }
}

/**
 * 更新選取狀態
 */
const updateSelection = () => {
  const treeView = treeViewRef.value?.instance
  if (!treeView) {
    return
  }

  // 如果是 CMSFrame 路由，不選取任何項目
  if (route.path.includes('CMSFrame')) {
    treeView.unselectAll()
    window.sessionStorage.setItem('routerCurrentPath', route.path)
  } else {
    treeView.selectItem(route.path)
    treeView.expandItem(route.path)
  }
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  updateSelection()
  
  if (props.compactMode) {
    treeViewRef.value?.instance.collapseAll()
  }
})

// ============================================
// 監聽
// ============================================
watch(() => route.path, () => {
  updateSelection()
})

watch(() => props.compactMode, (newVal) => {
  if (newVal) {
    treeViewRef.value?.instance.collapseAll()
  } else {
    updateSelection()
  }
})
</script>

<style lang="scss" scoped>
@import "../css/dx-styles.scss";

.side-navigation-menu {
  display: flex;
  flex-direction: column;
  min-height: 100%;
  height: 100%;
  width: 250px !important;

  .menu-container {
    min-height: 100%;
    display: flex;
    flex: 1;
  }
}

:deep(.dx-treeview) {
  white-space: wrap;

  .dx-treeview-item {
    padding-left: 0;
    padding-right: 0;
    
    .dx-icon {
      width: 40px !important;
      color: #666;
      margin: 0 !important;
    }
  }

  .dx-treeview-node {
    padding: 0 0 !important;
  }

  .dx-treeview-toggle-item-visibility {
    right: 10px;
    left: auto;
  }

  .dx-treeview-node {
    &[aria-level="1"] {
      font-weight: bold;
      border-bottom: 1px solid #ddd;
    }

    &[aria-level="2"] .dx-treeview-item-content {
      font-weight: normal;
      padding: 0px 10px;
    }

    &[aria-level="3"] .dx-treeview-item-content {
      font-weight: normal;
      padding: 0px 20px;
    }
  }

  .dx-treeview-node-container {
    .dx-treeview-node {
      &.dx-state-selected:not(.dx-state-focused) > .dx-treeview-item {
        background: transparent;
      }

      &.dx-state-selected > .dx-treeview-item * {
        color: #337ab7;
      }

      &:not(.dx-state-focused) > .dx-treeview-item.dx-state-hover {
        background-color: rgba(0, 0, 0, 0.05);
      }
    }
  }
}
</style>

