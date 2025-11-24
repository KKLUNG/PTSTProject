<template>
  <keep-alive>
    <div style="display: flex; align-items: center; width: 100%">
      <div>
        <DxSelectBox
          ref="cbo1Ref"
          :read-only="!isEditing"
          :data-source="dataSource"
          value-expr="value"
          display-expr="display"
          v-model:value="cboValue1"
          :search-enabled="isSearchEnabled1"
          :show-clear-button="true"
          :width="cbo1Width"
          @value-changed="() => onSelectedIndexChanged(1)"
          :tab-index="base_controlIndex"
        />
      </div>
      <div :style="{ width: cbo2Width }" v-if="cbmCount > 1">
        <DxSelectBox
          ref="cbo2Ref"
          v-if="cbmCount > 1"
          :read-only="!isEditing"
          :data-source="oRef2"
          value-expr="value"
          display-expr="display"
          v-model:value="cboValue2"
          :search-enabled="isSearchEnabled2"
          :show-clear-button="true"
          :width="cbo2Width"
          @value-changed="() => onSelectedIndexChanged(2)"
          :tab-index="base_controlIndex + 1"
        />
      </div>
      <div :style="{ width: cbo3Width }" v-if="cbmCount > 2">
        <DxSelectBox
          ref="cbo3Ref"
          v-if="cbmCount > 2"
          :read-only="!isEditing"
          :data-source="oRef3"
          value-expr="value"
          display-expr="display"
          v-model:value="cboValue3"
          :search-enabled="isSearchEnabled3"
          :show-clear-button="true"
          :width="cbo3Width"
          @value-changed="() => onSelectedIndexChanged(3)"
          :tab-index="base_controlIndex + 2"
        />
      </div>
      <div :style="{ width: cbo4Width }" v-if="cbmCount > 3">
        <DxSelectBox
          ref="cbo4Ref"
          v-if="cbmCount > 3"
          :read-only="!isEditing"
          :data-source="oRef4"
          value-expr="value"
          display-expr="display"
          v-model:value="cboValue4"
          :search-enabled="isSearchEnabled4"
          :show-clear-button="true"
          :width="cbo4Width"
          @value-changed="() => onSelectedIndexChanged(4)"
          :tab-index="base_controlIndex + 3"
        />
      </div>
      <div :style="{ width: cbo5Width }" v-if="cbmCount > 4">
        <DxSelectBox
          ref="cbo5Ref"
          v-if="cbmCount > 4"
          :read-only="!isEditing"
          :data-source="oRef5"
          value-expr="value"
          display-expr="display"
          v-model:value="cboValue5"
          :search-enabled="isSearchEnabled5"
          :show-clear-button="true"
          :width="cbo5Width"
          @value-changed="() => onSelectedIndexChanged(5)"
          :tab-index="base_controlIndex + 4"
        />
      </div>
    </div>
  </keep-alive>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxSelectBox from 'devextreme-vue/select-box'
import { useControlBase } from '@/composables/useControlBase'
import { apiPost } from '@/utils/api-util'

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

const cbo1Ref = ref()
const cbo2Ref = ref()
const cbo3Ref = ref()
const cbo4Ref = ref()
const cbo5Ref = ref()

const cbmCount = computed(() => {
  return props.multiFields.length + 1
})

const cbo1Width = computed(() => {
  if (props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.fieldName)
    if (o.length > 0) {
      return isNullOrEmpty(o[0].FWidth) ? "100%" : o[0].FWidth
    }
  }
  return "100%"
})

const cbo2Width = computed(() => {
  if (props.multiFields && props.multiFields.length > 0 && props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.multiFields[0])
    if (o.length > 0) {
      return isNullOrEmpty(o[0].FWidth) ? "100%" : o[0].FWidth
    }
  }
  return "100%"
})

const cbo3Width = computed(() => {
  if (props.multiFields && props.multiFields.length > 1 && props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.multiFields[1])
    if (o.length > 0) {
      return isNullOrEmpty(o[0].FWidth) ? "100%" : o[0].FWidth
    }
  }
  return "100%"
})

const cbo4Width = computed(() => {
  if (props.multiFields && props.multiFields.length > 2 && props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.multiFields[2])
    if (o.length > 0) {
      return isNullOrEmpty(o[0].FWidth) ? "100%" : o[0].FWidth
    }
  }
  return "100%"
})

const cbo5Width = computed(() => {
  if (props.multiFields && props.multiFields.length > 3 && props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.multiFields[3])
    if (o.length > 0) {
      return isNullOrEmpty(o[0].FWidth) ? "100%" : o[0].FWidth
    }
  }
  return "100%"
})

const isSearchEnabled1 = computed(() => {
  if (props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.fieldName)
    if (o.length > 0) {
      return !isNullOrEmpty(o[0].UDF01) && o[0].UDF01 == "1"
    }
  }
  return false
})

const isSearchEnabled2 = computed(() => {
  if (props.multiFields && props.multiFields.length > 0 && props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.multiFields[0])
    if (o.length > 0) {
      return !isNullOrEmpty(o[0].UDF01) && o[0].UDF01 == "1"
    }
  }
  return false
})

const isSearchEnabled3 = computed(() => {
  if (props.multiFields && props.multiFields.length > 1 && props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.multiFields[1])
    if (o.length > 0) {
      return !isNullOrEmpty(o[0].UDF01) && o[0].UDF01 == "1"
    }
  }
  return false
})

const isSearchEnabled4 = computed(() => {
  if (props.multiFields && props.multiFields.length > 2 && props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.multiFields[2])
    if (o.length > 0) {
      return !isNullOrEmpty(o[0].UDF01) && o[0].UDF01 == "1"
    }
  }
  return false
})

const isSearchEnabled5 = computed(() => {
  if (props.multiFields && props.multiFields.length > 3 && props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.multiFields[3])
    if (o.length > 0) {
      return !isNullOrEmpty(o[0].UDF01) && o[0].UDF01 == "1"
    }
  }
  return false
})

const cboValue1 = ref<string | null>(null)
const cboValue2 = ref<string | null>(null)
const cboValue3 = ref<string | null>(null)
const cboValue4 = ref<string | null>(null)
const cboValue5 = ref<string | null>(null)

const oRef2 = ref<any[]>([])
const oRef3 = ref<any[]>([])
const oRef4 = ref<any[]>([])
const oRef5 = ref<any[]>([])

const getCBO = async (key: string | null, field: string, cboId: string) => {
  if (isNullOrEmpty(key)) return

  if (props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == field)
    if (o.length > 0 && o[0].RefCommand) {
      const formData = {
        Command: o[0].RefCommand,
        KeyParameter: isNullOrEmpty(key) ? (appInfo?.emptyGuid || '') : key,
        UserGuid: appInfo?.userInfo?.userGuid || '',
        Lang: appInfo?.language || 'zhTW',
      }

      try {
        const res = await apiPost("/api/func/GetDataBySQL", formData)
        if (res.status == 200) {
          const result: any[] = []
          for (let i = 0; i < res.data.length; i++) {
            const obj: any = {}
            obj["value"] = Object.values(res.data[i])[0]
            obj["display"] = Object.values(res.data[i])[1]
            result.push(obj)
          }

          if (isForm.value && props.controlParent?.oRef) {
            props.controlParent.oRef[field] = Object.assign(
              props.controlParent.oRef[field] || {},
              result
            )
          }

          if (cboId == "2") oRef2.value = result
          if (cboId == "3") oRef3.value = result
          if (cboId == "4") oRef4.value = result
          if (cboId == "5") oRef5.value = result
        }
      } catch (err) {
        console.error(err)
        if (appInfo?.alert) {
          appInfo.alert(JSON.stringify(err), "GetDataBySQL")
        }
      }
    }
  }
}

const onSelectedIndexChanged = async (j: number) => {
  const componentRefs: Record<number, any> = {
    1: cbo1Ref,
    2: cbo2Ref,
    3: cbo3Ref,
    4: cbo4Ref,
    5: cbo5Ref
  }

  const component = componentRefs[j]?.value?.instance
  if (!component) return

  const e: any = { value: component.option("value") }

  if (j == 1) {
    if (cbmCount.value > 1) {
      oRef2.value = []
      oRef3.value = []
      oRef4.value = []
      oRef5.value = []
      cboValue2.value = null
      cboValue3.value = null
      cboValue4.value = null
      cboValue5.value = null
      if (props.multiFields && props.multiFields.length > 0) {
        await getCBO(cboValue1.value, props.multiFields[0], "2")
      }
    }
    if (props.oBody && props.oBody.length > 0) {
      const row = props.oBody.filter((x: any) => x.FName == props.fieldName)
      if (row.length > 0 && row[0].FocusIn) {
        try {
          // eval(row[0].FocusIn) - 注意：eval 有安全風險
        } catch (error) {
          console.error('FocusIn script error:', error)
        }
      }
    }
  }

  if (j == 2) {
    if (cbmCount.value > 2) {
      oRef3.value = []
      oRef4.value = []
      oRef5.value = []
      cboValue3.value = null
      cboValue4.value = null
      cboValue5.value = null
      if (props.multiFields && props.multiFields.length > 1) {
        await getCBO(cboValue2.value, props.multiFields[1], "3")
      }
    }
    if (props.oBody && props.oBody.length > 0 && props.multiFields && props.multiFields.length > 0) {
      const row = props.oBody.filter((x: any) => x.FName == props.multiFields[0])
      if (row.length > 0 && row[0].FocusIn) {
        try {
          // eval(row[0].FocusIn) - 注意：eval 有安全風險
        } catch (error) {
          console.error('FocusIn script error:', error)
        }
      }
    }
  }

  if (j == 3) {
    if (cbmCount.value > 3) {
      oRef4.value = []
      oRef5.value = []
      cboValue4.value = null
      cboValue5.value = null
      if (props.multiFields && props.multiFields.length > 2) {
        await getCBO(cboValue3.value, props.multiFields[2], "4")
      }
    }
    if (props.oBody && props.oBody.length > 0 && props.multiFields && props.multiFields.length > 1) {
      const row = props.oBody.filter((x: any) => x.FName == props.multiFields[1])
      if (row.length > 0 && row[0].FocusIn) {
        try {
          // eval(row[0].FocusIn) - 注意：eval 有安全風險
        } catch (error) {
          console.error('FocusIn script error:', error)
        }
      }
    }
  }

  if (j == 4) {
    if (cbmCount.value > 4) {
      oRef5.value = []
      cboValue5.value = null
      if (props.multiFields && props.multiFields.length > 3) {
        await getCBO(cboValue4.value, props.multiFields[3], "5")
      }
    }
    if (props.oBody && props.oBody.length > 0 && props.multiFields && props.multiFields.length > 2) {
      const row = props.oBody.filter((x: any) => x.FName == props.multiFields[2])
      if (row.length > 0 && row[0].FocusIn) {
        try {
          // eval(row[0].FocusIn) - 注意：eval 有安全風險
        } catch (error) {
          console.error('FocusIn script error:', error)
        }
      }
    }
  }

  if (j == 5) {
    if (props.oBody && props.oBody.length > 0 && props.multiFields && props.multiFields.length > 3) {
      const row = props.oBody.filter((x: any) => x.FName == props.multiFields[3])
      if (row.length > 0 && row[0].FocusIn) {
        try {
          // eval(row[0].FocusIn) - 注意：eval 有安全風險
        } catch (error) {
          console.error('FocusIn script error:', error)
        }
      }
    }
  }

  props.onValueChanged?.(cboValue1.value, props.fieldName || '')

  if (isForm.value && props.oData) {
    if (cbmCount.value > 1) props.oData[props.multiFields[0]] = cboValue2.value
    if (cbmCount.value > 2) props.oData[props.multiFields[1]] = cboValue3.value
    if (cbmCount.value > 3) props.oData[props.multiFields[2]] = cboValue4.value
    if (cbmCount.value > 4) props.oData[props.multiFields[3]] = cboValue5.value
    props.setMultiFieldsValue?.()
  } else {
    if (props.cellInfo) {
      props.cellInfo.component.beginUpdate()
      if (cbmCount.value > 1) {
        props.cellInfo.component.cellValue(
          props.cellInfo.row.rowIndex,
          props.multiFields[0],
          cboValue2.value
        )
      }
      if (cbmCount.value > 2) {
        props.cellInfo.component.cellValue(
          props.cellInfo.row.rowIndex,
          props.multiFields[1],
          cboValue3.value
        )
      }
      if (cbmCount.value > 3) {
        props.cellInfo.component.cellValue(
          props.cellInfo.row.rowIndex,
          props.multiFields[2],
          cboValue4.value
        )
      }
      if (cbmCount.value > 4) {
        props.cellInfo.component.cellValue(
          props.cellInfo.row.rowIndex,
          props.multiFields[3],
          cboValue5.value
        )
      }
      props.cellInfo.component.endUpdate()
    }
  }
}

const refresh = async () => {
  if (props.controlParent?.oData) {
    cboValue1.value = props.controlParent.oData[props.fieldName]
    if (props.multiFields.length >= 1) {
      await getCBO(cboValue1.value, props.multiFields[0], "2")
      cboValue2.value = props.controlParent.oData[props.multiFields[0]]
    }
    if (props.multiFields.length >= 2) {
      await getCBO(cboValue2.value, props.multiFields[1], "3")
      cboValue3.value = props.controlParent.oData[props.multiFields[1]]
    }
    if (props.multiFields.length >= 3) {
      await getCBO(cboValue3.value, props.multiFields[2], "4")
      cboValue4.value = props.controlParent.oData[props.multiFields[2]]
    }
    if (props.multiFields.length >= 4) {
      await getCBO(cboValue4.value, props.multiFields[3], "5")
      cboValue5.value = props.controlParent.oData[props.multiFields[3]]
    }
  }
}

// 暴露 refresh 方法供外部調用
defineExpose({
  refresh
})

onMounted(async () => {
  cboValue1.value = props.value

  if (props.multiFields && props.multiFields.length > 0) {
    cboValue2.value = props.oData == null
      ? props.cellInfo?.data?.[props.multiFields[0]] || null
      : props.oData[props.multiFields[0]] || null
  }

  if (props.multiFields && props.multiFields.length > 1) {
    cboValue3.value = props.oData == null
      ? props.cellInfo?.data?.[props.multiFields[1]] || null
      : props.oData[props.multiFields[1]] || null
  }

  if (props.multiFields && props.multiFields.length > 2) {
    cboValue4.value = props.oData == null
      ? props.cellInfo?.data?.[props.multiFields[2]] || null
      : props.oData[props.multiFields[2]] || null
  }

  if (props.multiFields && props.multiFields.length > 3) {
    cboValue5.value = props.oData == null
      ? props.cellInfo?.data?.[props.multiFields[3]] || null
      : props.oData[props.multiFields[3]] || null
  }

  if (!isNullOrEmpty(cboValue1.value) && cbmCount.value > 1 && props.multiFields && props.multiFields.length > 0) {
    await getCBO(cboValue1.value, props.multiFields[0], "2")
  }
  if (!isNullOrEmpty(cboValue2.value) && cbmCount.value > 2 && props.multiFields && props.multiFields.length > 1) {
    await getCBO(cboValue2.value, props.multiFields[1], "3")
  }
  if (!isNullOrEmpty(cboValue3.value) && cbmCount.value > 3 && props.multiFields && props.multiFields.length > 2) {
    await getCBO(cboValue3.value, props.multiFields[2], "4")
  }
  if (!isNullOrEmpty(cboValue4.value) && cbmCount.value > 4 && props.multiFields && props.multiFields.length > 3) {
    await getCBO(cboValue4.value, props.multiFields[3], "5")
  }
})
</script>

<style lang="scss" scoped>
</style>

