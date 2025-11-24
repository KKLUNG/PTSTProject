<template>
  <div>
    <div
      :class="appInfo.isMobile ? 'webPartHeaderTransparent' : 'webPartHeader'"
      v-if="!isNullOrEmpty(toolbarCaption)"
    >
      <div class="webPartCaption">
        <div><i :class="toolbarClass" /></div>
        <div>{{ toolbarCaption }}</div>
      </div>
    </div>
    <hr
      style="border: 1px solid"
      v-if="!isNullOrEmpty(toolbarCaption)"
    />
    <div>
      <component
        :is="XMLName"
        :XMLParameter="childXMLParameter"
        :FormMode="FormMode"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { markRaw } from 'vue'
import appInfo from '@/utils/app-Info'

// ============================================
// Props
// ============================================
interface Props {
  XMLName?: string
  XMLParameter?: string
  XMLCaption?: string
  showHeader?: boolean
  FormMode?: string
  pageKey?: string
}

const props = withDefaults(defineProps<Props>(), {
  XMLName: '',
  XMLParameter: '',
  XMLCaption: '',
  showHeader: true,
  FormMode: '',
  pageKey: ''
})

// ============================================
// 狀態變數
// ============================================
const toolbarCaption = ref('')
const childXMLParameter = ref('')

// ============================================
// Computed
// ============================================
const toolbarClass = computed(() => {
  return 'icon dx-icon-bookmark toolbarIcon'
})

// ============================================
// 方法
// ============================================
const isNullOrEmpty = (value: any): boolean => {
  return value === null || value === undefined || value === ''
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  if (!isNullOrEmpty(props.XMLParameter)) {
    const ary = props.XMLParameter.split(';')
    toolbarCaption.value = ary[0] || ''
    
    if (ary.length > 1) {
      childXMLParameter.value = props.XMLParameter.replace(ary[0] + ';', '')
    } else {
      childXMLParameter.value = props.FormMode || ''
    }
  } else {
    toolbarCaption.value = 'Caption is empty'
    childXMLParameter.value = ''
  }
})
</script>

<style lang="scss" scoped>
</style>

