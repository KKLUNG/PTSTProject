<template>
  <div>
    <div
      v-show="isEditing"
      style="display: flex; align-items: center; flex-wrap: wrap"
    >
      <div>
        <DxDateBox
          ref="sBoxRef"
          v-model:value="startDateValue"
          type="date"
          :read-only="!isEditing"
          :width="dateBoxWidth"
          :accept-custom-value="appInfo?.isMobile ? false : true"
          :use-mask-behavior="maxLength == 8 || appInfo?.isMobile ? false : true"
          :max-length="maxLength"
          :open-on-field-click="true"
          :show-clear-button="appInfo?.isCordova ? false : true"
          :show-drop-down-button="appInfo?.isCordova ? false : true"
          date-serialization-format="yyyy-MM-dd"
          :display-format="customFormat"
          @value-changed="onDateBoxClicked1"
          :tab-index="base_controlIndex"
          :element-attr="{ class: 'biaImeMode' }"
          :picker-type="appInfo?.isMobile ? 'rollers' : 'calendar'"
        />
      </div>
      <div>~</div>
      <div>
        <DxDateBox
          ref="eBoxRef"
          v-model:value="endDateValue"
          type="date"
          :read-only="!isEditing"
          :width="dateBoxWidth"
          :accept-custom-value="appInfo?.isMobile ? false : true"
          :use-mask-behavior="maxLength == 8 || appInfo?.isMobile ? false : true"
          :max-length="maxLength"
          :open-on-field-click="true"
          :show-clear-button="appInfo?.isCordova ? false : true"
          :show-drop-down-button="appInfo?.isCordova ? false : true"
          date-serialization-format="yyyy-MM-dd"
          :display-format="customFormat"
          @value-changed="onDateBoxClicked2"
          :tab-index="base_controlIndex + 1"
          :element-attr="{ class: 'biaImeMode' }"
          :picker-type="appInfo?.isMobile ? 'rollers' : 'calendar'"
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
  controlParent?: any
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
  multiFields: () => [],
  controlParent: null
})

const { isNullOrEmpty, dateToString, stringToDate, getFieldFormat, appInfo } = useControlBase()

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
const customFormat = ref<string>('yyyy-MM-dd')
const maxLength = ref<number>(8)

const dateBoxWidth = computed(() => {
  return isForm.value ? "115px" : "135px"
})

const sd = computed(() => {
  return sBoxRef.value?.instance
})

const ed = computed(() => {
  return eBoxRef.value?.instance
})

const onDateBoxClicked1 = (e: any) => {
  const format = sd.value?.option("dateSerializationFormat") || 'yyyy-MM-dd'
  let sdate = (startDateValue.value = e.value)
  let edate = endDateValue.value

  if (props.isEditing) {
    if (!isNullOrEmpty(startDateValue.value) && !isNullOrEmpty(endDateValue.value)) {
      sdate = stringToDate(String(sdate)) || new Date()
      edate = stringToDate(String(edate)) || new Date()
      if (sdate > edate) {
        const year = sdate.getFullYear()
        const month = sdate.getMonth()
        const day = sdate.getDate()
        const hour = edate.getHours()
        const minutes = edate.getMinutes()
        const seconds = edate.getSeconds()
        edate = new Date(year, month, day, hour, minutes, seconds)
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
  onFocusOut()

  if (!isNullOrEmpty(sdate)) {
    setTimeout(() => {
      sd.value?.blur()
      ed.value?.focus()
    }, 100)
  }
}

const onDateBoxClicked2 = (e: any) => {
  const format = sd.value?.option("dateSerializationFormat") || 'yyyy-MM-dd'
  let sdate = startDateValue.value
  let edate = (endDateValue.value = e.value)

  if (props.isEditing) {
    if (!isNullOrEmpty(startDateValue.value) && !isNullOrEmpty(endDateValue.value)) {
      sdate = stringToDate(String(sdate)) || new Date()
      edate = stringToDate(String(edate)) || new Date()

      if (edate < sdate) {
        const year = edate.getFullYear()
        const month = edate.getMonth()
        const day = edate.getDate()
        const hour = sdate.getHours()
        const minutes = sdate.getMinutes()
        const seconds = sdate.getSeconds()
        sdate = new Date(year, month, day, hour, minutes, seconds)
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
  onFocusOut()
}

const onFocusOut = () => {
  if (!isNullOrEmpty(startDateValue.value) && !isNullOrEmpty(endDateValue.value)) {
    if (props.oBody && props.oBody.length > 0) {
      const o = props.oBody.filter((x: any) => props.fieldName == x.FName)
      if (o.length > 0 && o[0].FocusIn) {
        try {
          // eval(o[0].FocusIn) - 注意：eval 有安全風險
        } catch (error) {
          console.error('FocusIn script error:', error)
        }
      }
    }
  }
}

const getDisplayText = () => {
  if (sd.value && ed.value) {
    displayText.value = sd.value.option("text") + " ~ " + ed.value.option("text")
  }
}

onMounted(() => {
  if (props.oBody && props.oBody.length > 0) {
    const bodyRow = props.oBody.filter((x: any) => x.FName == props.fieldName)[0]
    if (bodyRow) {
      const oformat = getFieldFormat(bodyRow.Format)
      const fcontrol = bodyRow.FControl
      if (fcontrol == "DA2") {
        customFormat.value = oformat.displayFormat == undefined ? "yyyy-MM-dd" : oformat.displayFormat
      } else {
        customFormat.value = oformat.displayFormat == undefined ? "yyyy-MM-dd HH:mm" : oformat.displayFormat
      }
      maxLength.value = customFormat.value.length
    }
  }

  // safari 不支援 2021-12-13T12:00:00 這種格式, new Date 會錯
  // 所以要用 stringToDate
  startDateValue.value = isNullOrEmpty(props.value)
    ? null
    : dateToString(stringToDate(props.value || '') || new Date(), "yyyy/MM/dd HH:mm:ss") as any

  endDateValue.value = props.oData == null
    ? (isNullOrEmpty(props.cellInfo?.data?.[props.multiFields?.[0] || ''])
        ? null
        : dateToString(stringToDate(props.cellInfo.data[props.multiFields[0]]) || new Date(), "yyyy/MM/dd HH:mm:ss") as any)
    : (isNullOrEmpty(props.oData[props.multiFields?.[0] || ''])
        ? null
        : dateToString(stringToDate(props.oData[props.multiFields[0]]) || new Date(), "yyyy/MM/dd HH:mm:ss") as any)

  getDisplayText()
})
</script>

<style lang="scss" scoped>
</style>

