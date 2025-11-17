<template>
  <component :is="layout">
    <router-view v-slot="{ Component, route }">
      <Suspense>
        <template #default>
          <component :is="Component" :key="route.path" />
        </template>
        <template #fallback>
          <div class="loading">載入中...</div>
        </template>
      </Suspense>
    </router-view>
  </component>
</template>

<script setup lang="ts">
import { computed, markRaw } from 'vue'
import { useRoute } from 'vue-router'
import type { Component } from 'vue'

const route = useRoute()

// 根據路由 meta 動態載入 layout，如果沒有指定則使用預設的 div
const layout = computed<Component | string>(() => {
  const layoutComponent = route.meta?.layout as Component | undefined
  if (layoutComponent) {
    return markRaw(layoutComponent)
  }
  return 'div' // 預設沒有 layout
})
</script>

<style lang="scss">
@use "./themes/bia-custom.scss" as *;

.loading { 
  max-width: 420px; 
  margin: 40px auto; 
  font-family: system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif; 
}
</style>
