<template>
  <div>
    <DxSelectBox
      :read-only="!isEditing"
      :data-source="dataSource"
      value-expr="value"
      display-expr="display"
      v-model:value="cboValue"
      :search-enabled="true"
      @value-changed="onSelectedIndexChanged"
      :tab-index="base_controlIndex"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxSelectBox from 'devextreme-vue/select-box'
import { useControlBase } from '@/composables/useControlBase'

interface Props {
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  setRefDatasource?: (value: any, fieldName: string) => void
  fieldName?: string
  oBody?: any[]
  dataSource?: any[]
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: true,
  value: null,
  onValueChanged: () => () => {},
  setRefDatasource: () => () => {},
  fieldName: '',
  oBody: () => [],
  dataSource: () => []
})

const { isNullOrEmpty } = useControlBase()

const base_controlIndex = computed(() => {
  if (props.oBody && props.oBody.length > 0) {
    const fieldRow = props.oBody.filter((r: any) => r.FName == props.fieldName)[0]
    if (fieldRow && fieldRow.FOrder) {
      return parseInt(fieldRow.FOrder)
    }
  }
  return 0
})

const cboValue = ref<string | null>(null)

const onSelectedIndexChanged = () => {
  props.onValueChanged?.(cboValue.value, props.fieldName || '')
  props.setRefDatasource?.(cboValue.value, props.fieldName || '')
}

onMounted(() => {
  cboValue.value = props.value
})
</script>

<style lang="scss" scoped>
</style>

