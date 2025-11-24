<template>
  <div>
    <div v-if="!isEditing">
      <div v-if="oData != null || cellInfo?.isOnForm">
        <DxTextBox :read-only="true" v-model:value="displayText" />
      </div>
      <div v-else>
        {{ displayText }}
      </div>
    </div>
    <div v-else>
      <div class="formButtonArea2" style="justify-content: flex-start">
        <div>
          <DxRadioGroup
            :ref="rboIdRef"
            :data-source="dataSource"
            value-expr="value"
            display-expr="display"
            :value="radioValue"
            :layout="direction"
            @value-changed="onClickHandler"
            :tab-index="base_controlIndex"
          />
        </div>
        <div style="width: 10px" v-if="multiFields.length > 0"></div>
        <div v-if="multiFields.length > 0">
          <DxTextBox
            :read-only="!isEditing"
            :value="textBoxValue"
            @value-changed="setTextBoxValue"
            :width="textBoxwidth"
            :tab-index="base_controlIndex + 1"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxRadioGroup from 'devextreme-vue/radio-group'
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
  dataSource?: any[]
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
  dataSource: () => [],
  multiFields: () => [],
  controlParent: null
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

const isForm = computed(() => {
  return props.oData != null
})

const rboIdRef = ref()
const radioValue = ref<string | null>(null)
const textBoxValue = ref<string>('')
const orgTextBoxValue = ref<string>('')
const textBoxPlaceholder = ref<string>('')
const textBoxwidth = ref<string | null>(null)
const displayText = ref<string>('')
const direction = ref<'horizontal' | 'vertical'>('horizontal')

const rbo = computed(() => {
  return rboIdRef.value?.instance
})

const onClickHandler = (e: any) => {
  radioValue.value = e.value
  textBoxValue.value = ''
  props.onValueChanged?.(radioValue.value, props.fieldName || '')

  if (props.oBody && props.oBody.length > 0) {
    const row = props.oBody.filter((x: any) => x.FName == props.fieldName)
    if (row.length > 0 && row[0].FocusIn && isForm.value) {
      try {
        // eval(row[0].FocusIn) - 注意：eval 有安全風險
      } catch (error) {
        console.error('FocusIn script error:', error)
      }
    }
  }

  if (e.value == "ZZ") {
    textBoxValue.value = orgTextBoxValue.value
  } else {
    textBoxValue.value = ""
  }
}

const setTextBoxValue = (e: any) => {
  if (props.oData != null) {
    if (props.multiFields && props.multiFields.length > 0) {
      props.oData[props.multiFields[0]] = e.value
      props.setMultiFieldsValue?.()
    }
  } else {
    if (props.cellInfo && props.multiFields && props.multiFields.length > 0) {
      props.cellInfo.component.cellValue(
        props.cellInfo.row.rowIndex,
        props.multiFields[0],
        e.value
      )
    }
  }
}

const getDisplayText = () => {
  if (!isNullOrEmpty(radioValue.value)) {
    const row = props.dataSource?.filter(
      (rs: any) => rs["value"].toUpperCase() == String(radioValue.value).toUpperCase()
    )

    if (row && row.length > 0) {
      displayText.value = row[0].display
      if (row[0].value == "ZZ") {
        displayText.value += textBoxValue.value
      }
    }
  }
}

onMounted(() => {
  radioValue.value = props.value

  if (props.oBody && props.oBody.length > 0) {
    const row = props.oBody.filter((x: any) => x.FName == props.fieldName)
    if (row.length > 0) {
      direction.value = row[0].UDF01 == "1" ? "vertical" : "horizontal"

      if (props.multiFields && props.multiFields.length > 0) {
        const o = props.oBody.filter((x: any) => x.FName == props.multiFields[0])
        if (o.length > 0) {
          textBoxwidth.value = isNullOrEmpty(o[0].FWidth) ? null : o[0].FWidth
          textBoxPlaceholder.value = o[0].Placeholder || ''

          const zzo = props.dataSource?.filter((x: any) => x.value == "ZZ")
          if (!zzo || zzo.length == 0) {
            const oo: any = {
              display: isNullOrEmpty(o[0].FCaption) ? "Other  " : o[0].FCaption + "  ",
              value: "ZZ"
            }
            if (props.dataSource) {
              props.dataSource.push(oo)
            }
          }
        }
      }
    }
  }

  if (props.multiFields && props.multiFields.length > 0) {
    textBoxValue.value = props.oData == null
      ? props.cellInfo?.data?.[props.multiFields[0]] || ''
      : props.oData[props.multiFields[0]] || ''
    orgTextBoxValue.value = textBoxValue.value
  }

  getDisplayText()
})
</script>

<style lang="scss" scoped>
</style>

