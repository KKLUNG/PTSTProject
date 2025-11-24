<template>
  <div>
    <div
      v-show="isEditing"
      style="display: flex; align-items: center; flex-wrap: wrap"
    >
      <div>
        <DxDateBox
          ref="sDateBoxRef"
          :value="startDateValue"
          type="date"
          :read-only="!isEditing"
          width="115px"
          :accept-custom-value="true"
          :use-mask-behavior="true"
          :open-on-field-click="true"
          :show-clear-button="true"
          date-serialization-format="yyyy-MM-ddTHH:mm:ss"
          display-format="yyyy-MM-dd"
          @value-changed="onDateBoxClicked1"
          :tab-index="base_controlIndex"
        />
      </div>
      <div>
        <DxDateBox
          ref="sTimeBoxRef"
          :read-only="!isEditing"
          :value="startTimeValue"
          @value-changed="onDateBoxClicked1"
          :show-clear-button="true"
          :interval="timeInterval"
          width="90px"
          type="time"
          :accept-custom-value="true"
          :use-mask-behavior="true"
          :open-on-field-click="true"
          date-serialization-format="yyyy-MM-ddTHH:mm:ss"
          display-format="HH:mm"
          picker-type="list"
          :tab-index="base_controlIndex + 1"
        />
      </div>
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

const { isNullOrEmpty, dateToString, stringToDate } = useControlBase()

const base_controlIndex = computed(() => {
  if (props.oBody && props.oBody.length > 0) {
    const fieldRow = props.oBody.filter((r: any) => r.FName == props.fieldName)[0]
    if (fieldRow && fieldRow.FOrder) {
      return parseInt(fieldRow.FOrder)
    }
  }
  return 0
})

const sDateBoxRef = ref()
const sTimeBoxRef = ref()

const startDateValue = ref<Date | null>(null)
const startTimeValue = ref<Date | null>(null)
const timeInterval = ref<number>(30)
const displayText = ref<string>('')

const sd = computed(() => sDateBoxRef.value?.instance)
const st = computed(() => sTimeBoxRef.value?.instance)

const onDateBoxClicked1 = (e: any) => {
  const format = 'yyyy-MM-ddTHH:mm:ss'
  let combinedValue = null
  
  if (startDateValue.value && startTimeValue.value) {
    const date = stringToDate(String(startDateValue.value)) || new Date()
    const time = stringToDate(String(startTimeValue.value)) || new Date()
    combinedValue = new Date(
      date.getFullYear(), date.getMonth(), date.getDate(),
      time.getHours(), time.getMinutes(), time.getSeconds()
    )
  }

  props.onValueChanged?.(combinedValue ? dateToString(combinedValue, format) : null, props.fieldName || '')
  getDisplayText()
}

const getDisplayText = () => {
  if (sd.value && st.value) {
    displayText.value = `${sd.value.option("text")} ${st.value.option("text")}`
  }
}

onMounted(() => {
  if (props.oBody && props.oBody.length > 0) {
    const bodyRow = props.oBody.filter((rs1: any) => rs1.FName == props.fieldName)[0]
    if (bodyRow && !isNullOrEmpty(bodyRow.UDF01)) {
      timeInterval.value = parseInt(bodyRow.UDF01)
    }
  }

  if (!isNullOrEmpty(props.value)) {
    const dateTime = stringToDate(props.value || '')
    if (dateTime) {
      startDateValue.value = dateTime
      startTimeValue.value = new Date("1970-01-01 " + dateToString(dateTime, "HH:mm:ss"))
    }
  }

  getDisplayText()
})
</script>

<style lang="scss" scoped>
</style>

