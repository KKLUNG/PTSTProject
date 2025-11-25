<template>
  <div class="side-nav-outer-toolbar">
    <DxScrollView ref="scrollViewRef" class="with-footer">
      <slot />
      <slot name="footer" />
    </DxScrollView>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import DxScrollView from 'devextreme-vue/scroll-view'

// ============================================
// Props
// ============================================
interface Props {
  title?: string
  isXSmall?: boolean
  isLarge?: boolean
}

withDefaults(defineProps<Props>(), {
  title: '',
  isXSmall: false,
  isLarge: false
})

// ============================================
// 狀態變數
// ============================================
const scrollViewRef = ref<any>(null)

// ============================================
// 計算屬性
// ============================================
const scrollView = computed(() => {
  return scrollViewRef.value?.instance
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

