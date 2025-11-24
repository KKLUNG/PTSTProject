<template>
  <div>
    <div v-if="!isEditing && isGrid">
      <div style="display: flex; align-items: center">
        <div :class="'icon dx-icon-' + value" style="margin: 3px"></div>
        <div style="margin: 3px">
          {{ displayText }}
        </div>
      </div>
    </div>
    <div v-else style="display: flex">
      <div :style="{ width: textBoxWidth }">
        <DxTextBox
          ref="txt1Ref"
          v-model:value="txt1Value"
          @value-changed="onTxt1Change"
        />
      </div>
      <div style="width:30px;font-size: 1.7em;margin: auto;text-align: center;">
        -
      </div>
      <div :style="{ width: textBoxWidth }">
        <DxTextBox
          ref="txt2Ref"
          v-model:value="txt2Value"
          @value-changed="onTxt2Change"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxTextBox from 'devextreme-vue/text-box'
import { useControlBase } from '@/composables/useControlBase'

// Props
interface Props {
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  setMultiFieldsValue?: () => void
  fieldName?: string
  oBody?: any[]
  oData?: any
  multiFields?: string[]
  cellInfo?: any
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
  multiFields: () => [],
  cellInfo: null,
  controlParent: null
})

// 使用共用方法
const { isNullOrEmpty } = useControlBase()

// Computed
const isGrid = computed(() => {
  return props.oData == null
})

const displayText = computed(() => {
  return [txt1Value.value, txt2Value.value].filter(v => v != null).join(' - ')
})

// Data
const txt1Value = ref<string | null>(null)
const txt2Value = ref<string | null>(null)
const textBoxWidth = ref<string>('60px')
const txt1Ref = ref()
const txt2Ref = ref()

// Methods
const onTxt1Change = (e: any) => {
  txt1Value.value = e.value
  props.onValueChanged?.(txt1Value.value, props.fieldName || '')

  if (props.isForm) {
    if (props.oData && props.multiFields && props.multiFields.length > 0) {
      props.oData[props.multiFields[0]] = txt2Value.value
      props.setMultiFieldsValue?.()
    }
  } else {
    if (props.cellInfo && props.multiFields && props.multiFields.length > 0) {
      props.cellInfo.component.cellValue(
        props.cellInfo.row.rowIndex,
        props.multiFields[0],
        txt2Value.value
      )
    }
  }
}

const onTxt2Change = (e: any) => {
  txt2Value.value = e.value
  props.onValueChanged?.(txt1Value.value, props.fieldName || '')

  if (props.isForm) {
    if (props.oData && props.multiFields && props.multiFields.length > 0) {
      props.oData[props.multiFields[0]] = txt2Value.value
      props.setMultiFieldsValue?.()
    }
  } else {
    if (props.cellInfo && props.multiFields && props.multiFields.length > 0) {
      props.cellInfo.component.cellValue(
        props.cellInfo.row.rowIndex,
        props.multiFields[0],
        txt2Value.value
      )
    }
  }
}

// Lifecycle
onMounted(() => {
  if (props.oBody && props.oBody.length > 0) {
    const row = props.oBody.filter((x: any) => x.FName == props.fieldName)[0]
    if (row && row.FWidth) {
      textBoxWidth.value = row.FWidth
    }
  }

  txt1Value.value = props.value

  if (props.isForm) {
    if (props.oData && props.multiFields && props.multiFields.length > 0) {
      txt2Value.value = props.oData[props.multiFields[0]]
    }
  } else {
    if (props.cellInfo && props.multiFields && props.multiFields.length > 0) {
      txt2Value.value = props.cellInfo.data[props.multiFields[0]]
    }
  }
})

// Computed for isForm
const isForm = computed(() => {
  return props.oData != null
})
</script>

<style lang="scss" scoped>
.dx-texteditor-border:before {
  border: medium none;
}
.dx-texteditor-border:after {
  border: medium none;
}
</style>

