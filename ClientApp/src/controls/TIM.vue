<template>
  <div>
    <div v-show="isEditing">
      <DxDateBox
        ref="sTimeBoxRef"
        :read-only="!isEditing"
        v-model:value="timeValue"
        @value-changed="onDateBoxClicked"
        :show-clear-button="true"
        :interval="timeInterval"
        type="time"
        :display-format="displayFormat"
        :use-mask-behavior="true"
        :max="maxValue"
        :min="minValue"
        :accept-custom-value="true"
        :open-on-field-click="true"
        :tab-index="base_controlIndex"
      />
    </div>
    <div v-show="!isEditing">
      <div v-if="oData != null">
        <DxTextBox :read-only="true" :value="displayText" />
      </div>
      <div v-else style="white-space: pre-wrap">
        {{ displayText }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxDateBox from 'devextreme-vue/date-box'
import DxTextBox from 'devextreme-vue/text-box'
import { useControlBase } from '@/composables/useControlBase'

interface Props {
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  fieldName?: string
  oBody?: any[]
  oData?: any
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: true,
  value: null,
  onValueChanged: () => () => {},
  fieldName: '',
  oBody: () => [],
  oData: null
})

const { isNullOrEmpty, dateToString } = useControlBase()

const base_controlIndex = computed(() => {
  if (props.oBody && props.oBody.length > 0) {
    const fieldRow = props.oBody.filter((r: any) => r.FName == props.fieldName)[0]
    if (fieldRow && fieldRow.FOrder) {
      return parseInt(fieldRow.FOrder)
    }
  }
  return 0
})

const sTimeBoxRef = ref()
const timeValue = ref<Date | null>(null)
const timeInterval = ref<number>(30)
const minValue = ref<Date | null>(null)
const maxValue = ref<Date | null>(null)
const displayText = ref<string>('')
const displayFormat = ref<string>('HH:mm')

const st = computed(() => {
  return sTimeBoxRef.value?.instance
})

const onDateBoxClicked = (e: any) => {
  if (timeValue.value) {
    props.onValueChanged?.(dateToString(timeValue.value, "HH:mm:ss"), props.fieldName || '')
    getDisplayText()
  }
}

const getDisplayText = () => {
  if (st.value) {
    displayText.value = st.value.option("text") || ''
  }
}

onMounted(() => {
  if (!isNullOrEmpty(props.value)) {
    timeValue.value = new Date("1970-01-01 " + props.value)
  }

  if (props.oBody && props.oBody.length > 0) {
    const bodyRow = props.oBody.filter((rs1: any) => rs1.FName == props.fieldName)[0]
    if (bodyRow) {
      if (!isNullOrEmpty(bodyRow.UDF01)) {
        timeInterval.value = parseInt(bodyRow.UDF01)
      }

      if (!isNullOrEmpty(bodyRow.UDF02)) {
        const parts = bodyRow.UDF02.split(";")
        if (parts.length >= 2) {
          minValue.value = new Date("1970-01-01 " + parts[0])
          maxValue.value = new Date("1970-01-01 " + parts[1])
        }
      }

      // show second
      displayFormat.value = (bodyRow.UDF03 == '1') ? "HH:mm:ss" : "HH:mm"
    }
  }

  getDisplayText()
})
</script>

<style lang="scss" scoped>
</style>

