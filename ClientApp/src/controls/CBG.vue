<template>
  <div>
    <div v-if="!isEditing">
      <div v-if="oData != null">
        <DxTextBox :read-only="true" :value="gridBoxDisplayExprValue" />
      </div>
      <div v-else>
        {{ gridBoxDisplayExprValue }}
      </div>
    </div>
    <div v-else>
      <DxDropDownBox
        ref="ddbRef"
        :value="ddlValue"
        :data-source="refDatasource"
        :defer-rendering="true"
        :display-expr="gridBoxDisplayExpr"
        :show-clear-button="true"
        :value-expr="pkeyFieldName"
        @value-changed="onDropDownBoxChanged"
        :read-only="!isEditing"
        :tab-index="base_controlIndex"
      >
        <template #content="{}">
          <div>
            <DxDataGrid
              :ref="gridViewNameRef"
              :data-source="refDatasource"
              :hover-state-enabled="true"
              :selected-row-keys="gridBoxValue"
              :height="gridHeight"
              @editor-preparing="onEditorPreparing"
              @row-click="onRowClick"
            >
              <DxSelection mode="single" show-check-boxes-mode="always" />
              <DxPaging :enabled="false" :page-size="99999" />
              <DxFilterRow :visible="true" apply-filter="onClick" />
              <DxScrolling mode="standard" />
              <DxColumn
                v-for="item in gridColumns"
                :key="'column' + item.columnName"
                :data-field="item.columnName"
                :caption="item.columnCaption"
                :width="item.columnWidth"
              />
            </DxDataGrid>
          </div>
        </template>
      </DxDropDownBox>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import DxDropDownBox from 'devextreme-vue/drop-down-box'
import {
  DxDataGrid,
  DxSelection,
  DxPaging,
  DxFilterRow,
  DxScrolling,
  DxColumn,
} from 'devextreme-vue/data-grid'
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

const ddbRef = ref()
const gridViewNameRef = ref()

const gridViewName = ref<string>('grid-view')
const gridBoxValue = ref<any[]>([])
const ddlValue = ref<string>('')
const gridColumns = ref<any[]>([])
const gridCaptions = ref<string[]>([])
const gridFields = ref<string[]>([])
const gridWidths = ref<string[]>([])
const gridShows = ref<string[]>([])
const pkeyFieldName = ref<string>('value')
const refDatasource = ref<any[]>([])
const gridBoxDisplayExprValue = ref<string>('')

const gridHeight = computed(() => {
  return "100%"
})

const gridBoxDisplayExpr = (item: any): string => {
  let displayStr = ""
  if (item != undefined) {
    if (gridShows.value.length == 0) {
      for (let i = 0; i < gridColumns.value.length; i++) {
        displayStr += item[gridColumns.value[i].columnName] + " / "
      }
      displayStr = displayStr.substring(0, displayStr.length - 2)
    } else {
      for (let j = 0; j < gridShows.value.length; j++) {
        displayStr += item[gridShows.value[j]] + " / "
      }
      displayStr = displayStr.substring(0, displayStr.length - 2)
    }
    return displayStr
  }
  return ""
}

watch(() => props.dataSource, (newVal) => {
  // THIS IS USED FOR FORMVIEW !!!! GRIDVIEW NO USE
  refDatasource.value = JSON.parse(JSON.stringify(newVal || []))
}, { immediate: true })

const refreshDatasource = (filterColumn: string) => {
  const filterValue = props.oData == null
    ? props.cellInfo?.data?.[filterColumn]
    : props.oData[filterColumn]

  if (filterValue == undefined) {
    refDatasource.value = []
  } else {
    const r = props.dataSource
    refDatasource.value = r.filter(
      (r: any) => r[filterColumn]?.toUpperCase() == filterValue?.toUpperCase()
    )
  }
}

const onDropDownBoxChanged = (e: any) => {
  if (e.value == null) {
    ddlValue.value = ""
    props.onValueChanged?.(e.value, props.fieldName || '')
    if (props.oData != null) {
      if (props.multiFields && props.multiFields.length > 0) {
        for (let i = 0; i < props.multiFields.length; i++) {
          props.oData[props.multiFields[i]] = null
        }
      }
    } else {
      if (props.cellInfo && props.multiFields && props.multiFields.length > 0) {
        props.cellInfo.data[props.multiFields[0]] = null
      }
    }
  }
}

const onEditorPreparing = (e: any) => {
  if (e.parentType == "filterRow") {
    e.editorOptions.onEnterKey = function (arg: any) {
      const applyButton = e.element.getElementsByClassName("dx-apply-button")[0] as HTMLElement
      if (applyButton) {
        applyButton.click()
      }
    }
  }
}

const onRowClick = (e: any) => {
  const grid = gridViewNameRef.value?.instance
  if (!grid) return

  const keys = grid.getSelectedRowKeys()
  if (keys.length > 0) {
    gridBoxValue.value = keys
    ddlValue.value = gridBoxValue.value[0][pkeyFieldName.value]
    props.onValueChanged?.(gridBoxValue.value[0][pkeyFieldName.value], props.fieldName || '')

    if (props.oData != null) {
      if (props.multiFields && props.multiFields.length > 0) {
        for (let i = 0; i < props.multiFields.length; i++) {
          props.oData[props.multiFields[i]] = gridBoxValue.value[0][props.multiFields[i]]
        }
      }
      props.setMultiFieldsValue?.()
    } else {
      if (props.cellInfo && props.multiFields && props.multiFields.length > 0) {
        for (let i = 0; i < props.multiFields.length; i++) {
          props.cellInfo.component.cellValue(
            props.cellInfo.row.rowIndex,
            props.multiFields[i],
            gridBoxValue.value[0][props.multiFields[i]]
          )
        }
      }
    }

    const ddb = ddbRef.value?.instance
    if (ddb) {
      ddb.close()
    }
  }
}

onMounted(() => {
  console.log('please use POG instead of CBG')
  
  if (props.oBody && props.oBody.length > 0) {
    const bodyRow = props.oBody.filter((rs1: any) => rs1.FName == props.fieldName)[0]
    if (bodyRow) {
      gridCaptions.value = bodyRow.UDF01 ? String(bodyRow.UDF01).split(";") : []
      gridWidths.value = bodyRow.UDF02 ? String(bodyRow.UDF02).split(";") : []
      gridShows.value = bodyRow.UDF03 ? String(bodyRow.UDF03).split(";") : []
      gridFields.value = bodyRow.UDF04 ? String(bodyRow.UDF04).split(";") : []
      pkeyFieldName.value = "value"

      refDatasource.value = props.dataSource || []
      
      if (!isNullOrEmpty(bodyRow.UDF05) && props.isEditing) {
        refreshDatasource(bodyRow.UDF05)
      }

      const columnCount = gridCaptions.value.length
      const defaultColumnWidth = 100.0 / columnCount

      for (let i = 0; i < gridShows.value.length; i++) {
        const columns: any = {}
        columns["columnName"] = gridShows.value[i]
        columns["columnCaption"] = isNullOrEmpty(gridCaptions.value[i])
          ? "no caption"
          : gridCaptions.value[i]
        columns["columnWidth"] = isNullOrEmpty(gridWidths.value[i])
          ? defaultColumnWidth.toString() + "%"
          : gridWidths.value[i]
        gridColumns.value.push(columns)
      }

      if (!isNullOrEmpty(props.value)) {
        const rsA = refDatasource.value
        const rowA = rsA.filter(
          (rsA: any) =>
            rsA[pkeyFieldName.value]?.toUpperCase() == String(props.value).toUpperCase()
        )

        if (rowA.length > 0) {
          gridBoxValue.value = rowA
          ddlValue.value = gridBoxValue.value[0][pkeyFieldName.value]
          
          let displayStr = ""
          if (gridShows.value.length == 0) {
            for (let i = 0; i < gridColumns.value.length; i++) {
              displayStr += rowA[0][gridColumns.value[i].columnName] + " / "
            }
            displayStr = displayStr.substring(0, displayStr.length - 2)
          } else {
            for (let j = 0; j < gridShows.value.length; j++) {
              displayStr += rowA[0][gridShows.value[j]] + " / "
            }
            displayStr = displayStr.substring(0, displayStr.length - 2)
          }
          gridBoxDisplayExprValue.value = displayStr
        } else {
          ddlValue.value = props.value || ''
        }
      } else {
        ddlValue.value = ""
      }
    }
  }
})
</script>

<style lang="scss" scoped>
</style>

