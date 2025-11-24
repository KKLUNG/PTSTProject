<template>
  <div>
    <div v-show="!isEditing">
      <div v-if="oData != null || cellInfo?.isOnForm">
        <DxTextBox :read-only="true" v-model:value="displayText" />
      </div>
      <div v-else>
        {{ displayText }}
      </div>
    </div>
    <div v-show="isEditing">
      <DxDropDownBox
        ref="ddlboxRef"
        v-model:value="treeBoxValue"
        :data-source="dataSource"
        :value-expr="valueExpr"
        :display-expr="displayAllExpr"
        :read-only="!isEditing"
        @value-changed="syncTreeViewSelection"
        :tab-index="base_controlIndex"
      >
        <template #content="{}">
          <DxTreeView
            :ref="treeViewNameRef"
            :items="dataSource"
            :select-by-click="true"
            :root-value="rootValue"
            data-structure="plain"
            :key-expr="valueExpr"
            :parent-id-expr="parentIdExpr"
            :selection-mode="selectionMode"
            :select-nodes-recursive="false"
            show-check-boxes-mode="normal"
            :display-expr="displayExpr"
            :search-enabled="true"
            @content-ready="syncTreeViewSelection"
            @selection-changed="treeView_itemSelectionChanged"
            @item-expanded="onItemExpanded"
            @item-selection-changed="onItemSelectionChanged"
          />
        </template>
      </DxDropDownBox>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxDropDownBox from 'devextreme-vue/drop-down-box'
import DxTreeView from 'devextreme-vue/tree-view'
import DxTextBox from 'devextreme-vue/text-box'
import { useControlBase } from '@/composables/useControlBase'

interface Props {
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  fieldName?: string
  oBody?: any[]
  oData?: any
  cellInfo?: any
  dataSource?: any[]
  controlParent?: any
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: true,
  value: null,
  onValueChanged: () => () => {},
  fieldName: '',
  oBody: () => [],
  oData: null,
  cellInfo: null,
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

const ddlboxRef = ref()
const treeViewNameRef = ref()

const treeBoxValue = ref<string[]>([])
const treeViewName = ref<string>('treeView')
const valueExpr = ref<string>('')
const displayExpr = ref<string>('')
const parentIdExpr = ref<string>('')
const displayAllExpr = ref<string>('')
const rootValue = ref<string | null>(null)
const selectionMode = ref<'single' | 'multiple'>('single')
const displayText = ref<string>('')

const ddl = computed(() => {
  return ddlboxRef.value?.instance
})

const tree = computed(() => {
  return treeViewNameRef.value?.instance
})

const syncTreeViewSelection = (e: any) => {
  const treeView =
    (e.component?.selectItem && e.component) ||
    (treeViewNameRef.value && tree.value)

  if (treeView) {
    if (e.value === null || e.value === undefined) {
      if (treeBoxValue.value.length == 0) {
        treeView.unselectAll()
      }
    } else {
      const values = e.value || treeBoxValue.value
      if (values && Array.isArray(values)) {
        values.forEach((value: any) => {
          treeView.selectItem(value)
          treeView.expandItem(value)
        })
      }
    }
  }
}

const treeView_itemSelectionChanged = (e: any) => {
  treeBoxValue.value = e.component.getSelectedNodeKeys()
  props.onValueChanged?.(
    e.component.getSelectedNodeKeys().toString(),
    props.fieldName || ''
  )
  getDisplayText()
}

const onItemExpanded = (e: any) => {
  // 2023.02.15 23.1 不需要了
}

const onItemSelectionChanged = (e: any) => {
  // 暫時不需要
}

const getDisplayText = () => {
  if (ddl.value) {
    displayText.value = ddl.value.option('text') || ''
  }
}

onMounted(() => {
  if (!isNullOrEmpty(props.value)) {
    const values = String(props.value).split(',')
    treeBoxValue.value = values.map(v => v.toLowerCase())
  } else {
    treeBoxValue.value = []
  }

  if (props.oBody && props.oBody.length > 0) {
    const bodyRow = props.oBody.filter((x: any) => x.FName == props.fieldName)[0]
    if (bodyRow && bodyRow.UDF01) {
      const arrayUDF01 = String(bodyRow.UDF01).split(';')
      valueExpr.value = arrayUDF01[0] || ''
      displayExpr.value = arrayUDF01[1] || ''
      parentIdExpr.value = arrayUDF01[2] || ''
      displayAllExpr.value = arrayUDF01[3] || ''
      rootValue.value = arrayUDF01[4] || null
    }

    selectionMode.value = isNullOrEmpty(bodyRow?.UDF02)
      ? 'single'
      : (bodyRow.UDF02 as 'single' | 'multiple')
  }

  getDisplayText()
})
</script>

<style lang="scss" scoped>
</style>

