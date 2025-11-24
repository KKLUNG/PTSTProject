<template>
  <DxTabs 
    :data-source="footerTabs" 
    width="100%" 
    :scrolling-enabled="false" 
    @item-click="onItemClick" 
  />
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import DxTabs from 'devextreme-vue/tabs'
import appInfo from '@/utils/app-Info'
import { apiGet } from '@/utils/api-util'

const router = useRouter()
const route = useRoute()

// ============================================
// 狀態變數
// ============================================
const footerTabs = ref<Array<{
  text: string
  icon: string
  content: string
  routerPath: string
}>>([])

// ============================================
// 方法
// ============================================
const onItemClick = (e: any) => {
  const url = e.itemData.routerPath
  if (route.path !== url) {
    router.push(url).catch(() => {})
  }
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  // 從 API 獲取選單資料作為 footer tabs
  apiGet('/api/cms/GetCMSMenuByUserId', {
    KeyParameter: 'F392721E-8D02-D080-AC75-35D140EC327A',
    UserGuid: appInfo.userInfo.userGuid || '',
    Language: appInfo.language || 'zhTW',
    IgnoreACL: '0',
    DisplayMode: appInfo.isMobile ? '2' : '1',
  })
    .then((res) => {
      if (res.status === 200 && Array.isArray(res.data)) {
        footerTabs.value = res.data.map((item: any) => ({
          text: item.MenuTitle || item.menuTitle || '',
          icon: item.icon || 'home',
          content: item.MenuTitle || item.menuTitle || '',
          routerPath: item.MenuSelfUrl || item.menuSelfUrl || '',
        }))
      }
    })
    .catch((err) => {
      console.error('FooterTab error:', err)
    })
})
</script>

<style lang="scss" scoped>
@use "../themes/bia-custom.scss" as *;
</style>

