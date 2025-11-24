<template>
  <div>
    <div
      v-show="isEditing"
      style="display: flex; align-items: center; flex-wrap: wrap"
    >
      <DxDateBox
        ref="sDateBoxRef"
        :value="monthValue"
        type="date"
        :read-only="!isEditing"
        width="120px"
        :accept-custom-value="true"
        :use-mask-behavior="true"
        :open-on-field-click="true"
        :show-clear-button="true"
        display-format="yyyy-MM"
        :calendar-options="{
          maxZoomLevel: 'year',
          minZoomLevel: 'century',
        }"
        @value-changed="onDateBoxClicked1"
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
const monthValue = ref<Date | null>(null)
const displayText = ref<string>('')

const sd = computed(() => {
  return sDateBoxRef.value?.instance
})

const getDisplayText = () => {
  if (sd.value && !isNullOrEmpty(sd.value.option("text"))) {
    displayText.value = sd.value.option("text")
  }
}

const onDateBoxClicked1 = (e: any) => {
  let sdate: string | null = null
  if (sd.value && sd.value.option("value") != null) {
    sdate = dateToString(sd.value.option("value"), "yyyy/MM/dd")
  }

  props.onValueChanged?.(sdate, props.fieldName || '')
  getDisplayText()

  if (props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.fieldName)
    if (o.length > 0 && o[0]["FocusIn"]) {
      try {
        // eval(o[0]["FocusIn"]) - 注意：eval 有安全風險
      } catch (error) {
        console.error('FocusIn script error:', error)
      }
    }
  }
}

onMounted(() => {
  // safari 不支援 2021-12-13T12:00:00 這種格式, new Date 會錯
  // 所以要用 stringToDate
  monthValue.value = isNullOrEmpty(props.value)
    ? null
    : stringToDate(props.value || '')

  getDisplayText()
})
</script>

<style lang="scss" scoped>
</style>

