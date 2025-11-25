<template>
  <div>Connect to {{ ssoTarget }}</div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import appInfo from '@/utils/app-Info'

// ============================================
// Props
// ============================================
interface Props {
  XMLName?: string
  XMLParameter?: string
  XMLCaption?: string
  pageKey?: string
}

const props = withDefaults(defineProps<Props>(), {
  XMLName: '',
  XMLParameter: '',
  XMLCaption: '',
  pageKey: ''
})

const router = useRouter()

// ============================================
// 狀態變數
// ============================================
const ssoTarget = ref('')

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  const userId = appInfo.userInfo.userId || ''
  const params = props.XMLParameter.split(',')
  const systemId = params[0] || ''
  const wfSNMsgGuid = params.length > 1 ? params[1] : ''
  
  // 處理 SSO 目標 URL
  let target = props.XMLName.replace(/\^\^\^/g, '/')
  target = target.replace(/\$\$\$/g, '/')
  ssoTarget.value = target

  // 開啟 SSO 視窗
  if (!wfSNMsgGuid) {
    if ((window as any).cordova) {
      (window as any).cordova.InAppBrowser.open(
        `${target}/#/CMSSSO/${userId}/${systemId}`,
        '_blank'
      )
    } else {
      window.open(`${target}/#/CMSSSO/${userId}/${systemId}`, '_blank')
    }
  } else {
    if ((window as any).cordova) {
      (window as any).cordova.InAppBrowser.open(
        `${target}/#/CMSSSOFlowSign/${userId}/${systemId}/${wfSNMsgGuid}`,
        '_blank'
      )
    } else {
      window.open(
        `${target}/#/CMSSSOFlowSign/${userId}/${systemId}/${wfSNMsgGuid}`,
        '_blank'
      )
    }
  }

  // 如果有工作流訊息 GUID，跳轉到指定頁面
  if (wfSNMsgGuid) {
    router.push('/CMSPage/7ca3d384-5b88-4620-9c9e-02a1076b9783').catch(() => {})
  }
})
</script>

<style scoped>
</style>

