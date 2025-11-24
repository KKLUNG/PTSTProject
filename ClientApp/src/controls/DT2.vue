<template>
  <div>
    <div v-show="isEditing" style="display: flex; align-items: center">
      <div>
        <DxDateBox
          ref="sBoxRef"
          :value="startDateValue"
          type="datetime"
          :read-only="!isEditing"
          :width="dateTimeBoxWidth"
          :accept-custom-value="appInfo?.isMobile ? false : true"
          :use-mask-behavior="appInfo?.isMobile ? false : true"
          max-length="16"
          :open-on-field-click="true"
          :show-clear-button="appInfo?.isCordova ? false : true"
          :show-drop-down-button="appInfo?.isCordova ? false : true"
          date-serialization-format="yyyy-MM-ddTHH:mm:ss"
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
          :value="endDateValue"
          type="datetime"
          :read-only="!isEditing"
          :width="dateTimeBoxWidth"
          :accept-custom-value="appInfo?.isMobile ? false : true"
          :use-mask-behavior="appInfo?.isMobile ? false : true"
          max-length="16"
          :show-clear-button="appInfo?.isCordova ? false : true"
          :show-drop-down-button="appInfo?.isCordova ? false : true"
          date-serialization-format="yyyy-MM-ddTHH:mm:ss"
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
        <DxTextBox :read-only="true" v-model:value="displayText" />
      </div>
      <div v-else>
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
const customFormat = ref<string>('yyyy-MM-dd HH:mm')

const dateTimeBoxWidth = computed(() => {
  return isForm.value ? "170px" : "185px"
})

const sd = computed(() => {
  return sBoxRef.value?.instance
})

const ed = computed(() => {
  return eBoxRef.value?.instance
})

const onDateBoxClicked1 = (e: any) => {
  const format = 'yyyy-MM-ddTHH:mm:ss'
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
  getDisplayText()
}

const onDateBoxClicked2 = (e: any) => {
  const format = 'yyyy-MM-ddTHH:mm:ss'
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
  getDisplayText()
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
      customFormat.value = oformat.displayFormat == undefined ? "yyyy-MM-dd HH:mm" : oformat.displayFormat
    }
  }

  startDateValue.value = isNullOrEmpty(props.value)
    ? null
    : stringToDate(props.value || '') || null

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

