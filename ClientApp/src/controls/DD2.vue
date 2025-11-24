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
      <div>~</div>
      <div>
        <DxDateBox
          ref="eDateBoxRef"
          :value="endDateValue"
          type="date"
          :read-only="!isEditing"
          width="115px"
          :accept-custom-value="true"
          :use-mask-behavior="true"
          :open-on-field-click="true"
          :show-clear-button="true"
          date-serialization-format="yyyy-MM-ddTHH:mm:ss"
          display-format="yyyy-MM-dd"
          @value-changed="onDateBoxClicked2"
          :tab-index="base_controlIndex + 2"
        />
      </div>
      <div>
        <DxDateBox
          ref="eTimeBoxRef"
          :read-only="!isEditing"
          :value="endTimeValue"
          @value-changed="onDateBoxClicked2"
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
          :tab-index="base_controlIndex + 3"
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
  setMultiFieldsValue?: () => void
  fieldName?: string
  oBody?: any[]
  oData?: any
  cellInfo?: any
  multiFields?: string[]
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: true,
  value: null,
  onValueChanged: () => () => {},
  setMultiFieldsValue: () => () => {},
  fieldName: '',
  oBody: () => [],
  oData: null,
  cellInfo: null,
  multiFields: () => []
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

const isForm = computed(() => {
  return props.oData != null
})

const sDateBoxRef = ref()
const sTimeBoxRef = ref()
const eDateBoxRef = ref()
const eTimeBoxRef = ref()

const startDateValue = ref<Date | null>(null)
const startTimeValue = ref<Date | null>(null)
const endDateValue = ref<Date | null>(null)
const endTimeValue = ref<Date | null>(null)
const timeInterval = ref<number>(30)
const displayText = ref<string>('')

const sd = computed(() => sDateBoxRef.value?.instance)
const st = computed(() => sTimeBoxRef.value?.instance)
const ed = computed(() => eDateBoxRef.value?.instance)
const et = computed(() => eTimeBoxRef.value?.instance)

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

const onDateBoxClicked2 = (e: any) => {
  const format = 'yyyy-MM-ddTHH:mm:ss'
  let combinedValue = null
  
  if (endDateValue.value && endTimeValue.value) {
    const date = stringToDate(String(endDateValue.value)) || new Date()
    const time = stringToDate(String(endTimeValue.value)) || new Date()
    combinedValue = new Date(
      date.getFullYear(), date.getMonth(), date.getDate(),
      time.getHours(), time.getMinutes(), time.getSeconds()
    )
  }

  if (isForm.value) {
    if (props.multiFields && props.multiFields.length > 0) {
      props.oData![props.multiFields[0]] = combinedValue ? dateToString(combinedValue, format) : null
      props.setMultiFieldsValue?.()
    }
  } else {
    if (props.cellInfo && props.multiFields && props.multiFields.length > 0) {
      props.cellInfo.component.cellValue(
        props.cellInfo.row.rowIndex,
        props.multiFields[0],
        combinedValue ? dateToString(combinedValue, format) : null
      )
    }
  }
  getDisplayText()
}

const getDisplayText = () => {
  if (sd.value && st.value && ed.value && et.value) {
    displayText.value = `${sd.value.option("text")} ${st.value.option("text")} ~ ${ed.value.option("text")} ${et.value.option("text")}`
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

  if (props.multiFields && props.multiFields.length > 0) {
    const multiValue = props.oData == null
      ? props.cellInfo?.data?.[props.multiFields[0]]
      : props.oData[props.multiFields[0]]
    
    if (!isNullOrEmpty(multiValue)) {
      const dateTime = stringToDate(multiValue)
      if (dateTime) {
        endDateValue.value = dateTime
        endTimeValue.value = new Date("1970-01-01 " + dateToString(dateTime, "HH:mm:ss"))
      }
    }
  }

  getDisplayText()
})
</script>

<style lang="scss" scoped>
</style>

