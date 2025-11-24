<template>
  <div>
    <div v-if="!isEditing">
      <div v-if="oData != null">
        <DxTextBox :read-only="true" v-model:value="displayText" />
      </div>
      <div v-else style="white-space: pre-wrap">
        {{ displayText }}
      </div>
    </div>
    <div v-else :style="style">
      <div
        v-for="item in dataSource"
        :key="'cho' + item.value"
        :style="styleItem"
      >
        <DxCheckBox
          :read-only="!isEditing"
          :value="isCheck(item.value)"
          :text="item.display"
          :name="item.value"
          @value-changed="onClickHandler"
          :tab-index="base_controlIndex"
        />
      </div>
      <div :style="styleItem" v-if="multiFields.length > 0">
        <DxCheckBox
          :read-only="!isEditing"
          v-model:value="isOtherChecked"
          :text="checkBoxCaption"
          @value-changed="onOtherClickHandler"
          :tab-index="base_controlIndex"
        />
      </div>
      <div v-if="multiFields.length > 0">
        <DxTextBox
          :read-only="!isEditing"
          :placeholder="textBoxPlaceholder"
          v-model:value="textBoxValue"
          @value-changed="setTextBoxValue"
          :width="textBoxwidth"
          :tab-index="base_controlIndex + 1"
        />
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
  multiFields: () => []
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

const checkBoxValues = ref<string>('')
const textBoxValue = ref<string>('')
const textBoxPlaceholder = ref<string>('')
const textBoxwidth = ref<string | null>(null)
const isOtherChecked = ref<boolean>(false)
const checkBoxCaption = ref<string>('Other ')
const displayText = ref<string>('')
const style = ref<string>('display: flex;flex-direction :column; flex-wrap: wrap;justify-content:space-between;width:99%')
const styleItem = ref<string>('margin: 10px;width:220px;justify-content: center;align-items: center;')

const isCheck = (val: string): boolean => {
  return isNullOrEmpty(props.value)
    ? false
    : String(props.value).toLowerCase().includes(val.toLowerCase())
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

  checkBoxValues.value = arrChecked.toString()
  props.onValueChanged?.(checkBoxValues.value, props.fieldName || '')
  getDisplayText()
}

const onOtherClickHandler = (e: any) => {
  if (!e.value) {
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

  if (isNullOrEmpty(e.value)) {
    isOtherChecked.value = true
  }
}

const getDisplayText = () => {
  if (!isNullOrEmpty(checkBoxValues.value)) {
    const o = checkBoxValues.value.split(",")
    let t = ""
    for (let i = 0; i < o.length; i++) {
      const row = props.dataSource?.filter(
        (rs: any) => rs["value"].toUpperCase() == o[i].toUpperCase()
      )
      if (row && row.length > 0) {
        t += row[0].display + ", "
      }
    }
    if (!isNullOrEmpty(t)) {
      displayText.value = t.substring(0, t.length - 2)
    }
  }
}

onMounted(() => {
  console.log('2023.07.01 CHO is going to deprecated, please use CHH instead.')
  
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

      if (props.multiFields && props.multiFields.length > 0) {
        const o = props.oBody.filter((x: any) => x.FName == props.multiFields[0])
        if (o.length > 0) {
          checkBoxCaption.value = isNullOrEmpty(o[0].FCaption)
            ? "Other  "
            : o[0].FCaption + "  "
          textBoxwidth.value = isNullOrEmpty(o[0].Width) ? null : o[0].Width
          textBoxPlaceholder.value = o[0].Placeholder || ''

          let otherText = ""
          if (props.oData == null) {
            if (!isNullOrEmpty(props.cellInfo?.data?.[props.multiFields[0]])) {
              otherText = props.cellInfo.data[props.multiFields[0]]
              isOtherChecked.value = true
            } else {
              isOtherChecked.value = false
            }
          } else {
            if (!isNullOrEmpty(props.oData[props.multiFields[0]])) {
              otherText = props.oData[props.multiFields[0]]
              isOtherChecked.value = true
            } else {
              isOtherChecked.value = false
            }
          }
          textBoxValue.value = otherText
          getDisplayText()
          displayText.value = isNullOrEmpty(displayText.value)
            ? otherText
            : displayText.value + "," + otherText
        }
      } else {
        getDisplayText()
      }
    }
  }
})
</script>

<style lang="scss" scoped>
</style>

