<template>
  <div id="root">
    <div :class="cssClasses">
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
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, markRaw, reactive, onMounted, onBeforeUnmount } from 'vue'
import { useRoute } from 'vue-router'
import type { Component } from 'vue'
import { sizes, subscribe, unsubscribe } from './utils/media-query'

const route = useRoute()

// 根據路由 meta 動態載入 layout，如果沒有指定則使用預設的 div
const layout = computed<Component | string>(() => {
  const layoutComponent = route.meta?.layout as Component | undefined
  if (layoutComponent) {
    return markRaw(layoutComponent)
  }
  return 'div' // 預設沒有 layout
})

// 螢幕尺寸資訊（用於響應式設計）
function getScreenSizeInfo() {
  const screenSizes = sizes()
  return {
    isXSmall: screenSizes['screen-x-small'],
    isLarge: screenSizes['screen-large'],
    cssClasses: Object.keys(screenSizes).filter(cl => screenSizes[cl as keyof typeof screenSizes])
  }
}

const screen = reactive(getScreenSizeInfo())
const cssClasses = computed(() => ['app'].concat(screen.cssClasses))

// 監聽螢幕尺寸變化
const screenSizeChanged = () => {
  const newInfo = getScreenSizeInfo()
  screen.isXSmall = newInfo.isXSmall
  screen.isLarge = newInfo.isLarge
  screen.cssClasses = newInfo.cssClasses
}

// 生命週期
onMounted(() => {
  subscribe(screenSizeChanged)
})

onBeforeUnmount(() => {
  unsubscribe(screenSizeChanged)
})
</script>

<style lang="scss">
@use "./themes/bia-custom.scss" as *;

html,
body {
  margin: 0px;
  min-height: 100dvh;
  height: 100dvh;
}

#root {
  height: 100dvh;
}

* {
  box-sizing: border-box;
}

.app {
  display: flex;
  height: 100dvh;
  width: 100dvw;
  background-size: cover;
  background-position: center center;
  background-repeat: no-repeat;
  background-attachment: fixed;
}

.loading { 
  max-width: 420px; 
  margin: 40px auto; 
  font-family: system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif; 
}

.tile-background {
  background-size: cover;
  background-position: center center;
  background-repeat: no-repeat;
  background-attachment: fixed;
}
</style>
