<template>
  <div class="swi">
    <DxSwitch
      v-model:value="switchValue"
      @value-changed="setValue"
      :read-only="!isEditing"
      switched-off-text="Off"
      switched-on-text="On"
      :hint="hint"
      :tab-index="base_controlIndex"
    />
    <div>
      <span>{{ hint }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { DxSwitch } from 'devextreme-vue/switch'
import { useControlBase } from '@/composables/useControlBase'

interface Props {
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  fieldName?: string
  oBody?: any[]
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: true,
  value: null,
  onValueChanged: () => () => {},
  fieldName: '',
  oBody: () => []
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

const switchValue = ref<boolean>(false)

const hint = computed(() => {
  return switchValue.value ? "On" : "Off"
})

const setValue = (e: any) => {
  props.onValueChanged?.(switchValue.value ? "1" : "0", props.fieldName || '')
}

onMounted(() => {
  switchValue.value = props.value == "1" ? true : false

  if (isNullOrEmpty(props.value)) {
    props.onValueChanged?.(switchValue.value ? "1" : "0", props.fieldName || '')
  }
})
</script>

<style lang="scss" scoped>
.swi {
  justify-content: flex-start;
  align-items: center;
  flex-wrap: wrap;
  display: flex;
}
</style>

