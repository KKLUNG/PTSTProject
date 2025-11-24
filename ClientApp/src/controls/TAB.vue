<template>
  <div>
    <AdminTab
      ref="tab"
      :XMLName="TabXMLName"
      :XMLParameter="TabXMLParameter"
      :TabParentParameter="value"
      :AdminTabControlParent="controlParent"
      :isEditing="isEditing"
      :JSONDataSource="parentData"
      :JSONParentKey="value"
      :JSONValueChange="setTabValue"
      :pageKey="pageKey"
      :isTAB="true"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import AdminTab from '@/components/AdminTab.vue'
import { useControlBase } from '@/composables/useControlBase'

// ============================================
// Props
// ============================================
interface Props {
  isCustom?: boolean
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  fieldName?: string
  oBody?: any[]
  oData?: any
  cellInfo?: any
  controlParent?: any
  pageKey?: string
}

const props = withDefaults(defineProps<Props>(), {
  isCustom: false,
  isEditing: true,
  value: null,
  onValueChanged: () => {},
  fieldName: '',
  oBody: () => [],
  oData: null,
  cellInfo: null,
  controlParent: null,
  pageKey: ''
})

// ============================================
// 使用共用方法
// ============================================
const { isNullOrEmpty } = useControlBase()

// ============================================
// Computed
// ============================================
const isGrid = computed(() => {
  return props.oData == null && props.cellInfo != null
})

// ============================================
// 狀態變數
// ============================================
const TabXMLName = ref('')
const TabXMLParameter = ref('')
const parentData = ref<any>(null)

// ============================================
// 方法
// ============================================
const setTabValue = (e: any, fn: string) => {
  props.onValueChanged?.(JSON.stringify(e), fn)
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  const bodyRow = props.oBody.find((x) => x.FName == props.fieldName)
  if (!bodyRow) return

  TabXMLName.value = bodyRow.UDF01 || '' // XMLName
  TabXMLParameter.value = bodyRow.UDF02 || '' // XMLParameter

  if (isGrid.value) {
    parentData.value = props.cellInfo?.row?.data || null
  } else {
    parentData.value = props.oData
  }
})
</script>

<style scoped>
</style>

