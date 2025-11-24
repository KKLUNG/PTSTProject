<template>
  <div>
    <DxCheckBox
      v-model:value="switchValue"
      @value-changed="setValue"
      :read-only="!isEditing"
      :tab-index="base_controlIndex"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { DxCheckBox } from 'devextreme-vue/check-box'
import { useControlBase } from '@/composables/useControlBase'

// Props (繼承自 _BASE)
interface Props {
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  fieldName?: string
  oBody?: any[]
  oData?: any
  controlParent?: any
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: true,
  value: null,
  onValueChanged: () => () => {},
  fieldName: '',
  oBody: () => [],
  oData: null
})

// 使用共用方法
const { isNullOrEmpty } = useControlBase()

// 計算 base_controlIndex
const base_controlIndex = computed(() => {
  if (props.oBody && props.oBody.length > 0) {
    const fieldRow = props.oBody.filter((r: any) => r.FName == props.fieldName)[0]
    if (fieldRow && fieldRow.FOrder) {
      return parseInt(fieldRow.FOrder)
    }
  }
  return 0
})

// Data
const switchValue = ref<boolean>(false)

// Computed
const hint = computed(() => {
  return switchValue.value ? "1" : "0"
})

// Methods
const setValue = (e: any) => {
  props.onValueChanged?.(switchValue.value ? "1" : "0", props.fieldName || '')

  if (props.oBody && props.oBody.length > 0) {
    const row = props.oBody.filter((x: any) => x.FName == props.fieldName)
    if (row.length > 0 && row[0].FocusIn && props.oData != null) {
      // 執行 FocusIn 腳本（如果有的話）
      try {
        // 注意：eval 有安全風險，實際使用時應該改用更安全的方式
        // eval(row[0].FocusIn)
      } catch (error) {
        console.error('FocusIn script error:', error)
      }
    }
  }
}

// Lifecycle
onMounted(() => {
  switchValue.value = props.value == "1" ? true : false

  // 加這一行是因為新增若未修改，抓不到資料，因為這種 control 只有 on/off，default is off
  if (isNullOrEmpty(props.value)) {
    props.onValueChanged?.(switchValue.value ? "1" : "0", props.fieldName || '')
  }
})
</script>

<style lang="scss" scoped>
</style>

