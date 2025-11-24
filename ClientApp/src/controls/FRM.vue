<template>
  <div>
    <AdminFormView
      :ref="fieldName"
      :XMLName="FRMXMLName"
      :XMLParameter="FRMXMLParameter"
      :JSONDataSource="FRMJSONDataSource"
      :JSONValueChange="setValue"
      :JSONIsEditing="isEditing"
      :JSONFieldName="fieldName"
      :isJSON="isJSON"
      :isFRM="true"
      :showHeader="showHeader"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import AdminFormView from '@/components/AdminFormView.vue'
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
  pageKey?: string
}

const props = withDefaults(defineProps<Props>(), {
  isCustom: false,
  isEditing: true,
  value: null,
  onValueChanged: () => {},
  fieldName: '',
  oBody: () => [],
  pageKey: ''
})

// ============================================
// 使用共用方法
// ============================================
const { isNullOrEmpty } = useControlBase()

// ============================================
// 狀態變數
// ============================================
const FRMXMLName = ref('')
const FRMXMLParameter = ref('')
const FRMJSONDataSource = ref<any>({})
const showHeader = ref(true)
const isJSON = ref(true)

// ============================================
// 方法
// ============================================
const setValue = (e: any) => {
  if (isJSON.value) {
    props.onValueChanged?.(JSON.stringify(e), props.fieldName || '')
  }
}

const refresh = () => {
  console.log('not implement yet!')
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  const bodyRow = props.oBody.find((x) => x.FName == props.fieldName)
  if (!bodyRow) return

  FRMXMLName.value = bodyRow.UDF01 || '' // XMLName
  FRMXMLParameter.value = isNullOrEmpty(bodyRow.UDF02)
    ? props.value || ''
    : `${props.value};${bodyRow.UDF02}` // XMLParameter
  FRMJSONDataSource.value =
    bodyRow.UDF04 == '1'
      ? isNullOrEmpty(props.value)
        ? {}
        : JSON.parse(props.value || '{}')
      : {}
  showHeader.value = bodyRow.UDF03 == '1'
  isJSON.value = bodyRow.UDF04 == '1'
})

// ============================================
// Expose
// ============================================
defineExpose({
  refresh
})
</script>

<style scoped>
</style>

