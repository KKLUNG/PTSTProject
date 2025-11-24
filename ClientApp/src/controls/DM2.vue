<template>
  <div>
    <div v-show="isEditing" style="display: flex; align-items: center; flex-wrap: wrap;">
      <div>
        <DxDateBox
          ref="sBoxRef"
          :value="startDateValue"
          type="date"
          :read-only="!isEditing"
          width="115px"
          :accept-custom-value="true"
          :use-mask-behavior="true"
          :open-on-field-click="true"
          :show-clear-button="true"
          date-serialization-format="yyyy-MM-dd"
          display-format="yyyy-MM"
          :calendar-options="{
            maxZoomLevel: 'year',
            minZoomLevel: 'century',
          }"
          @value-changed="onDateBoxClicked1"
          :tab-index="base_controlIndex"
        />
      </div>
      <div>~</div>
      <div>
        <DxDateBox
          ref="eBoxRef"
          :value="endDateValue"
          type="date"
          :read-only="!isEditing"
          width="115px"
          :accept-custom-value="true"
          :use-mask-behavior="true"
          :open-on-field-click="true"
          :show-clear-button="true"
          date-serialization-format="yyyy-MM-dd"
          display-format="yyyy-MM"
          :calendar-options="{
            maxZoomLevel: 'year',
            minZoomLevel: 'century',
          }"
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

const isForm = computed(() => {
  return props.oData != null
})

const sBoxRef = ref()
const eBoxRef = ref()

const startDateValue = ref<Date | null>(null)
const endDateValue = ref<Date | null>(null)
const displayText = ref<string>('')

const sd = computed(() => {
  return sBoxRef.value?.instance
})

const ed = computed(() => {
  return eBoxRef.value?.instance
})

const onDateBoxClicked1 = (e: any) => {
  const format = 'yyyy-MM-dd'
  let sdate = (startDateValue.value = e.value)
  let edate = endDateValue.value

  if (props.isEditing) {
    if (!isNullOrEmpty(startDateValue.value) && !isNullOrEmpty(endDateValue.value)) {
      sdate = stringToDate(String(sdate)) || new Date()
      edate = stringToDate(String(edate)) || new Date()
      if (sdate > edate) {
        const year = sdate.getFullYear()
        const month = sdate.getMonth()
        const day = 1
        edate = new Date(year, month, day)
      }
    }

    if (isNullOrEmpty(edate)) {
      if (isNullOrEmpty(sdate)) {
        startDateValue.value = endDateValue.value = null
      } else {
        endDateValue.value = startDateValue.value
      }
    } else {
      endDateValue.value = dateToString(edate, format) as any
    }
  }

  props.onValueChanged?.(startDateValue.value, props.fieldName || '')

  if (isForm.value) {
    if (props.multiFields && props.multiFields.length > 0) {
      props.oData![props.multiFields[0]] = endDateValue.value
      props.setMultiFieldsValue?.()
    }
  } else {
    if (props.cellInfo && props.multiFields && props.multiFields.length > 0) {
      props.cellInfo.component.cellValue(
        props.cellInfo.row.rowIndex,
        props.multiFields[0],
        endDateValue.value
      )
    }
  }
  getDisplayText()
}

const onDateBoxClicked2 = (e: any) => {
  const format = 'yyyy-MM-dd'
  let sdate = startDateValue.value
  let edate = (endDateValue.value = e.value)

  if (props.isEditing) {
    if (!isNullOrEmpty(startDateValue.value) && !isNullOrEmpty(endDateValue.value)) {
      sdate = stringToDate(String(sdate)) || new Date()
      edate = stringToDate(String(edate)) || new Date()

      if (edate < sdate) {
        const year = edate.getFullYear()
        const month = edate.getMonth()
        const day = 1
        sdate = new Date(year, month, day)
        startDateValue.value = dateToString(sdate, format) as any
      }
    }

    if (isNullOrEmpty(sdate)) {
      startDateValue.value = endDateValue.value
    }
  }

  props.onValueChanged?.(startDateValue.value, props.fieldName || '')

  if (isForm.value) {
    if (props.multiFields && props.multiFields.length > 0) {
      props.oData![props.multiFields[0]] = endDateValue.value
      props.setMultiFieldsValue?.()
    }
  } else {
    if (props.cellInfo && props.multiFields && props.multiFields.length > 0) {
      props.cellInfo.component.cellValue(
        props.cellInfo.row.rowIndex,
        props.multiFields[0],
        endDateValue.value
      )
    }
  }
  getDisplayText()
}

const getDisplayText = () => {
  if (sd.value && ed.value) {
    displayText.value = sd.value.option("text") + " ~ " + ed.value.option("text")
  }
}

onMounted(() => {
  startDateValue.value = isNullOrEmpty(props.value)
    ? null
    : stringToDate(props.value || '')

  endDateValue.value = props.oData == null
    ? (isNullOrEmpty(props.cellInfo?.data?.[props.multiFields?.[0] || ''])
        ? null
        : stringToDate(props.cellInfo.data[props.multiFields[0]]) || null)
    : (isNullOrEmpty(props.oData[props.multiFields?.[0] || ''])
        ? null
        : stringToDate(props.oData[props.multiFields[0]]) || null)

  getDisplayText()
})
</script>

<style lang="scss" scoped>
</style>

