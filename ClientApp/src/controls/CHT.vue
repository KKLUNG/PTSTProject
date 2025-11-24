<template>
  <div>
    <div v-if="isEditing">
      <DxTagBox
        :read-only="!isEditing"
        :data-source="{
          store: {
            type: 'array',
            data: dataSource,
          },
        }"
        value-expr="value"
        display-expr="display"
        :search-enabled="true"
        :show-clear-button="true"
        :show-selection-controls="true"
        :show-drop-down-button="true"
        v-model:value="tagBoxValue"
        @value-changed="setValue"
        height="27px"
        :tab-index="base_controlIndex"
      />
    </div>
    <div v-else style="white-space: pre-wrap">
      {{ displayText }}
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import DxTagBox from 'devextreme-vue/tag-box'
import { useControlBase } from '@/composables/useControlBase'

interface Props {
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  fieldName?: string
  oBody?: any[]
  dataSource?: any[]
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: true,
  value: null,
  onValueChanged: () => () => {},
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

const tagBoxValue = ref<string[]>([])
const displayText = ref<string>("")

const setValue = (e: any) => {
  props.onValueChanged?.(tagBoxValue.value.toString(), props.fieldName || '')
  getDisplayText()
}

const getDisplayText = () => {
  if (tagBoxValue.value.length > 0) {
    let t = ""
    for (let i = 0; i < tagBoxValue.value.length; i++) {
      const row = props.dataSource?.filter(
        (rs: any) => rs["value"].toUpperCase() == tagBoxValue.value[i].toUpperCase()
      )
      if (row && row.length > 0) {
        t += row[0].display + ", "
      }
    }
    if (!isNullOrEmpty(t)) {
      displayText.value = t.substring(0, t.length - 2)
    }
  } else {
    displayText.value = ""
  }
}

watch(() => tagBoxValue.value, () => {
  getDisplayText()
}, { deep: true })

onMounted(() => {
  if (!isNullOrEmpty(props.value)) {
    tagBoxValue.value = String(props.value).toLowerCase().split(',')
  }
  getDisplayText()
})
</script>

<style lang="scss" scoped>
</style>

