<template>
  <div></div>
</template>

<script setup lang="ts">
import { computed, ref, getCurrentInstance } from 'vue'
import { useControlBase } from '@/composables/useControlBase'

// Props 定義
interface Props {
  isCustom?: boolean
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  setMultiFieldsValue?: () => void
  setRefDatasource?: (fieldName: string, datasource: any[]) => void
  setUploadStatus?: (fieldName: string, status: string) => void
  multiFields?: string[]
  XMLName?: string
  XMLParameter?: string
  fieldName?: string
  cellInfo?: any
  formMode?: string
  dataSource?: any[]
  oData?: any
  oBody?: any[]
  oRef?: any
  pageKey?: string
  controlParent?: any
  showList?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  isCustom: false,
  isEditing: true,
  value: null,
  onValueChanged: () => () => {},
  setMultiFieldsValue: () => () => {},
  setRefDatasource: () => () => {},
  setUploadStatus: () => () => {},
  multiFields: () => [],
  showList: false
})

// 使用共用方法
const { isNullOrEmpty, appInfo, cssVariable } = useControlBase()

// Computed
const isGrid = computed(() => {
  return props.oData == null && !props.showList
})

const isForm = computed(() => {
  return props.oData != null
})

const isList = computed(() => {
  return props.oData == null && props.showList
})

// Data
const base_controlIndex = ref(0)
const hyperLinkStyle = computed(() => {
  return `word-wrap: break-word; color: ${cssVariable.baseHyperLinkColor || '#000000'}`
})

// 初始化
if (props.oBody != null && props.oBody != undefined && props.oBody.length > 0) {
  const fieldRow = props.oBody.filter((r: any) => r.FName == props.fieldName)[0]
  if (fieldRow && fieldRow.FOrder) {
    base_controlIndex.value = parseInt(fieldRow.FOrder)
  }
}

// 暴露給子元件使用
defineExpose({
  isNullOrEmpty,
  base_controlIndex,
  hyperLinkStyle,
  isGrid,
  isForm,
  isList
})
</script>

