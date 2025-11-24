<template>
  <div>
    <div v-if="!isEditing && isGrid">
      <div>{{ value }}</div>
    </div>
    <div v-else style="display: flex">
      <div>
        <DxAutocomplete
          v-model:data-source="txtDataSource"
          value-expr="value"
          v-model:value="txcValue"
          :max-item-count="10"
          :read-only="!isEditing"
          :placeholder="txcPlaceHolder"
          :show-clear-button="true"
          @value-changed="onTextChange"
          @focus-out="onFocusOut"
          :tab-index="base_controlIndex"
          :input-attr="{ style: 'text-transform: uppercase' }"
        />
      </div>
      <div v-if="isShowAdd && isEditing">
        <DxButton icon="icon icon-search-plus" @click="onAddClick" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { DxAutocomplete } from 'devextreme-vue/autocomplete'
import DxButton from 'devextreme-vue/button'
import { useControlBase } from '@/composables/useControlBase'

interface Props {
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  fieldName?: string
  oBody?: any[]
  dataSource?: any[]
  controlParent?: any
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: true,
  value: null,
  onValueChanged: () => () => {},
  fieldName: '',
  oBody: () => [],
  dataSource: () => [],
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

const isGrid = computed(() => {
  return props.controlParent?.oData == null
})

const isForm = computed(() => {
  return props.controlParent?.oData != null
})

const txtDataSource = ref<any[]>([])
const txcValue = ref<string | null>(null)
const txcPlaceHolder = ref<string>('')
const isShowAdd = ref<boolean>(false)

const onTextChange = (e: any) => {
  props.onValueChanged?.(txcValue.value, props.fieldName || '')
}

const onFocusOut = (e: any) => {
  if (props.controlParent?.isAdminPanel || props.controlParent?.isQueryPanel)
    return

  const o = { display: txcValue.value, value: txcValue.value }
  const existObj = txtDataSource.value.filter((x: any) => x.value == txcValue.value)
  if (existObj.length == 0) {
    txtDataSource.value.push(o)
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

const onAddClick = (e: any) => {
  if (props.oBody && props.oBody.length > 0) {
    const row = props.oBody.filter((x: any) => x.FName == props.fieldName)
    if (row.length > 0) {
      const custFunction = row[0].UDF02
      if (!isNullOrEmpty(custFunction)) {
        try {
          // 注意：eval 有安全風險，實際使用時應該改用更安全的方式
          // if (custFunction.trimLeft().substring(0, 5) == 'async')
          //   eval("(" + custFunction + ")()")
          // else
          //   eval(custFunction)
        } catch (error) {
          console.error('UDF02 script error:', error)
        }
      }
    }
  }
}

onMounted(() => {
  txtDataSource.value = props.dataSource || []
  txcValue.value = props.value

  if (props.oBody && props.oBody.length > 0) {
    const row = props.oBody.filter((x: any) => x.FName == props.fieldName)
    if (row.length > 0) {
      txcPlaceHolder.value = row[0].Placeholder || ''

      if (row[0].UDF01 == "1" && !props.controlParent?.isQueryPanel && !props.controlParent?.isAdminPanel) {
        isShowAdd.value = true
      } else {
        isShowAdd.value = false
      }
    }
  }
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

