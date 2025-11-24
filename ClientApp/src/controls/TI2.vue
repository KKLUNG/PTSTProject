<template>
  <div>
    <div v-show="isEditing" style="display: flex; align-items: center">
      <div>
        <DxDateBox
          ref="sTimeBoxRef"
          v-model:value="startDateValue"
          type="time"
          :read-only="!isEditing"
          width="90px"
          :accept-custom-value="false"
          :open-on-field-click="true"
          :show-clear-button="true"
          :interval="timeInterval"
          :display-format="displayFormat"
          :use-mask-behavior="true"
          @value-changed="onDateBoxClicked1"
          :tab-index="base_controlIndex"
        />
      </div>
      <div>~</div>
      <div>
        <DxDateBox
          ref="eTimeBoxRef"
          v-model:value="endDateValue"
          type="time"
          :read-only="!isEditing"
          width="90px"
          :accept-custom-value="false"
          :open-on-field-click="true"
          :show-clear-button="true"
          :interval="timeInterval"
          :max="maxValue"
          :min="minValue"
          :display-format="displayFormat"
          :use-mask-behavior="true"
          @value-changed="onDateBoxClicked2"
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

const sTimeBoxRef = ref()
const eTimeBoxRef = ref()

const startDateValue = ref<Date | null>(null)
const endDateValue = ref<Date | null>(null)
const timeInterval = ref<number>(30)
const minValue = ref<Date | null>(null)
const maxValue = ref<Date | null>(null)
const displayText = ref<string>('')
const displayFormat = ref<string>('HH:mm')

const st = computed(() => {
  return sTimeBoxRef.value?.instance
})

const et = computed(() => {
  return eTimeBoxRef.value?.instance
})

const onDateBoxClicked1 = (e: any) => {
  props.onValueChanged?.(
    dateToString(startDateValue.value || new Date(), "HH:mm:ss"),
    props.fieldName || ''
  )
  getDisplayText()
}

const onDateBoxClicked2 = (e: any) => {
  props.onValueChanged?.(
    dateToString(startDateValue.value || new Date(), "HH:mm:ss"),
    props.fieldName || ''
  )

  if (props.isForm) {
    if (props.multiFields && props.multiFields.length > 0) {
      props.oData![props.multiFields[0]] = dateToString(endDateValue.value || new Date(), "HH:mm:ss")
      props.setMultiFieldsValue?.()
    }
  } else {
    if (props.cellInfo && props.multiFields && props.multiFields.length > 0) {
      props.cellInfo.component.cellValue(
        props.cellInfo.row.rowIndex,
        props.multiFields[0],
        dateToString(endDateValue.value || new Date(), "HH:mm:ss")
      )
    }
  }
  getDisplayText()
}

const getDisplayText = () => {
  if (st.value && et.value) {
    displayText.value = st.value.option("text") + " ~ " + et.value.option("text")
  }
}

onMounted(() => {
  if (!isNullOrEmpty(props.value)) {
    startDateValue.value = new Date("1970-01-01 " + props.value)
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

      displayFormat.value = (bodyRow.UDF03 == '1') ? "HH:mm:ss" : "HH:mm"
    }
  }

  if (props.multiFields && props.multiFields.length > 0) {
    endDateValue.value = props.oData == null
      ? (isNullOrEmpty(props.cellInfo?.data?.[props.multiFields[0]])
          ? null
          : new Date("1970-01-01 " + props.cellInfo.data[props.multiFields[0]]))
      : (isNullOrEmpty(props.oData[props.multiFields[0]])
          ? null
          : new Date("1970-01-01 " + props.oData[props.multiFields[0]]))
  }

  getDisplayText()
})
</script>

<style lang="scss" scoped>
</style>

