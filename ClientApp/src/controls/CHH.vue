<template>
  <div :style="style">
    <div
      v-for="item in dataSource"
      :key="'cho' + item.value"
      :style="styleItem"
    >
      <div style="display: flex">
        <div>
          <DxCheckBox
            :ref="el => { if (el) chkRefs[item.value] = el }"
            :read-only="!isEditing"
            :value="isCheck(item.value)"
            :text="item.display"
            :name="item.value"
            @value-changed="onClickHandler"
            :tab-index="base_controlIndex"
          />
        </div>
        <div
          v-show="item.showTextBox == '1'"
          style="margin-left: 5px; margin-top: -5px"
        >
          <DxTextBox
            :ref="el => { if (el) txtRefs[item.value] = el }"
            :read-only="!isEditing"
            :value="getText(item.value)"
            @value-changed="onTextChanged"
            :name="item.value"
            :tab-index="base_controlIndex"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { DxCheckBox } from 'devextreme-vue/check-box'
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

const { isNullOrEmpty, appInfo } = useControlBase()

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

const checkBoxValues = ref<string>('')
const textBoxValues = ref<Record<string, string>>({})
const style = ref<string>('display: flex;flex-direction :column; flex-wrap: wrap;justify-content:space-between;width:99%')
const styleItem = ref<string>('margin: 10px;width:220px;justify-content: center;align-items: center;')
const chkRefs: Record<string, any> = {}
const txtRefs: Record<string, any> = {}

const isCheck = (val: string): boolean => {
  return isNullOrEmpty(props.value)
    ? false
    : String(props.value).toLowerCase().includes(val.toLowerCase())
}

const getText = (val: string): string => {
  return isNullOrEmpty(textBoxValues.value)
    ? ""
    : isNullOrEmpty(textBoxValues.value[val])
      ? ""
      : textBoxValues.value[val]
}

const onClickHandler = (e: any) => {
  let arrChecked = isNullOrEmpty(checkBoxValues.value)
    ? []
    : checkBoxValues.value.split(",")

  if (e.value && !arrChecked.toString().includes(e.component.option("name"))) {
    arrChecked.push(e.component.option("name"))
  } else {
    arrChecked = arrChecked.filter(
      (item: string) => item !== e.component.option("name")
    )
  }

  checkBoxValues.value = arrChecked.toString().toLowerCase()
  props.onValueChanged?.(checkBoxValues.value, props.fieldName || '')

  if (!e.value && txtRefs[e.component.option("name")]) {
    const tb = txtRefs[e.component.option("name")].instance
    if (tb) {
      tb.option("value", "")
    }
  }

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
}

const onTextChanged = (e: any) => {
  textBoxValues.value[e.component.option("name")] = e.value
  const ch = chkRefs[e.component.option("name")]?.instance
  if (ch) {
    if (isNullOrEmpty(e.value)) {
      ch.option("value", false)
    } else {
      ch.option("value", true)
    }
  }

  if (props.oData != null) {
    if (props.multiFields && props.multiFields.length > 0) {
      props.oData[props.multiFields[0]] = JSON.stringify(textBoxValues.value)
      props.setMultiFieldsValue?.()
    }
  } else {
    props.onValueChanged?.(checkBoxValues.value, props.fieldName || '')
    if (props.cellInfo && props.multiFields && props.multiFields.length > 0) {
      props.cellInfo.component.cellValue(
        props.cellInfo.row.rowIndex,
        props.multiFields[0],
        JSON.stringify(textBoxValues.value)
      )
    }
  }
  props.onValueChanged?.(checkBoxValues.value, props.fieldName || '')
}

onMounted(() => {
  checkBoxValues.value = props.value || ''

  if (props.oBody && props.oBody.length > 0) {
    const row = props.oBody.filter((x: any) => x.FName == props.fieldName)
    if (row.length > 0) {
      const cellWidth = isNullOrEmpty(row[0].UDF02)
        ? String(window.innerWidth)
        : row[0].UDF02

      if (row[0].UDF01 == "1") {  // Vertical
        style.value = "width:99%;display: flex;flex-direction :column; flex-wrap: wrap;"
        styleItem.value = "margin: 10px;justify-content: center;align-items: center;"
      } else {  // Horizontal
        style.value = "display: flex; flex-flow: row wrap;"
        styleItem.value = `margin: 10px;width:${cellWidth};justify-content: center;align-items: center;`
      }

      const hasTextBox = props.dataSource?.filter((x: any) => x.showTextBox == "1")

      if (hasTextBox && hasTextBox.length > 0 && props.multiFields && props.multiFields.length == 0) {
        if (appInfo?.alert) {
          appInfo.alert(
            "if CHH with TextBox, please set up MultiField",
            appInfo.title
          )
        }
      }

      if (hasTextBox && hasTextBox.length > 0 && props.multiFields && props.multiFields.length > 0) {
        if (props.oData == null) {
          textBoxValues.value = isNullOrEmpty(props.cellInfo?.data?.[props.multiFields[0]])
            ? {}
            : JSON.parse(props.cellInfo.data[props.multiFields[0]])
        } else {
          textBoxValues.value = isNullOrEmpty(props.oData[props.multiFields[0]])
            ? {}
            : JSON.parse(props.oData[props.multiFields[0]])
        }
      } else {
        textBoxValues.value = {}
      }
    }
  }
})
</script>

<style lang="scss" scoped>
</style>

