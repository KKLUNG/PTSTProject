<template>
  <div>
    <div v-if="multiFields.length > 0" style="display: flex">
      <div
        style="width: 100px; padding: 10px"
        v-for="(item, index) in multiFields"
        :key="index"
      >
        <DxTextBox
          :ref="el => { if (el) textBoxRefs[index] = el }"
          :id="multiFields[index]"
          :value="textData[index]"
          :read-only="textBoxReadonly"
          @value-changed="(e) => textBoxValueChange(e, index)"
          @focus-out="textBoxFocusOut"
        />
      </div>
    </div>
    <div>
      <AdminGridForm
        v-if="isShow"
        :ref="fieldName"
        :XMLName="GRDXMLName"
        :XMLParameter="GRDXMLParameter"
        :JSONDataSource="GRDJSONDataSource"
        :JSONValueChange="setValue"
        :JSONIsEditing="isEditing"
        :JSONParentKey="parentKey"
        :JSONFieldName="fieldName"
        :GRDBody="grdBody"
        :GRDRef="grdRef"
        :GRDPopupWH="grdPopupWH"
        :pageKey="pageKey"
        :showHeader="false"
        :isJSON="true"
        :controlHeight="controlHeight"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import DxTextBox from 'devextreme-vue/text-box'
import AdminGridForm from '@/components/AdminGridForm.vue'
import { useControlBase } from '@/composables/useControlBase'
import appInfo from '@/utils/app-Info'

// ============================================
// Props
// ============================================
interface Props {
  isCustom?: boolean
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  setMultiFieldsValue?: () => void
  fieldName?: string
  oBody?: any[]
  oData?: any
  oRef?: any
  cellInfo?: any
  controlParent?: any
  multiFields?: string[]
  pageKey?: string
}

const props = withDefaults(defineProps<Props>(), {
  isCustom: false,
  isEditing: true,
  value: null,
  onValueChanged: () => {},
  setMultiFieldsValue: () => {},
  fieldName: '',
  oBody: () => [],
  oData: null,
  oRef: () => ({}),
  cellInfo: null,
  controlParent: null,
  multiFields: () => [],
  pageKey: ''
})

// ============================================
// 使用共用方法
// ============================================
const { isNullOrEmpty, getNewGuid } = useControlBase()

// ============================================
// Computed
// ============================================
const isGrid = computed(() => {
  return props.oData == null && props.cellInfo != null
})

const isForm = computed(() => {
  return props.oData != null && props.cellInfo == null
})

const isShow = computed(() => {
  return (
    props.isEditing ||
    isForm.value ||
    (isGrid.value && GRDJSONDataSource.value.length > 0)
  )
})

// ============================================
// 狀態變數
// ============================================
const GRDXMLName = ref('')
const GRDXMLParameter = ref('')
const GRDJSONDataSource = ref<any[]>([])
const GRDDynamicColumnControl = ref<string[]>([])
const GRDDynamicColumnCaptionSuffix = ref<string[]>([])
const parentKey = ref<string | null>(null)
const orgGrdBody = ref<any[]>([])
const grdBody = ref<any[]>([])
const grdRef = ref<any>({})
const grdPopupWH = ref('95%')
const textData = ref<string[]>([])
const textBoxReadonly = ref(false)
const controlHeight = ref('')
const textBoxRefs = ref<any[]>([])

// ============================================
// 方法
// ============================================
const setValue = (e: any) => {
  props.onValueChanged?.(JSON.stringify(e), props.fieldName || '')
  
  // TODO: 實作 FocusIn 邏輯
  // const row = props.oBody.find((x) => x.FName == props.fieldName)
  // if (row && !isNullOrEmpty(row.FocusIn) && isForm.value) {
  //   eval(row.FocusIn)
  // }
}

const refresh = () => {
  if (props.controlParent?.oData) {
    GRDJSONDataSource.value = isNullOrEmpty(
      props.controlParent.oData[props.fieldName]
    )
      ? []
      : JSON.parse(props.controlParent.oData[props.fieldName] || '[]')
  }
}

const getDataSource = () => {
  // TODO: 實作獲取數據源邏輯
  // const gridComponent = document.querySelector(`[ref="${props.fieldName}"]`)
  // return gridComponent?.oData || []
  return []
}

const textBoxValueChange = (e: any, index: number) => {
  if (index < textData.value.length) {
    textData.value[index] = e.value || ''
  }

  if (isGrid.value && props.cellInfo) {
    props.cellInfo.data[props.multiFields[index]] = e.value
  } else if (isForm.value && props.oData) {
    props.oData[props.multiFields[index]] = e.value
    props.setMultiFieldsValue?.()
  }
  renderBody()
}

const textBoxFocusOut = (e: any) => {
  // TODO: 實作焦點離開邏輯
}

const renderBody = () => {
  grdBody.value = JSON.parse(JSON.stringify(orgGrdBody.value))
  
  if (props.multiFields.length === 0) return

  const bodyRow = props.oBody.find((x) => x.FName == props.fieldName)
  if (!bodyRow) return

  for (let i = 0; i < props.multiFields.length; i++) {
    for (let j = 0; j < GRDDynamicColumnControl.value.length; j++) {
      if (!isNullOrEmpty(textData.value[i])) {
        const fcontrol = GRDDynamicColumnControl.value[j]
        const o = JSON.parse(JSON.stringify(grdBody.value[0] || {}))
        
        o['FName'] = textData.value[i] + GRDDynamicColumnCaptionSuffix.value[j]?.trim() || ''
        o['FCaption'] = textData.value[i] + (GRDDynamicColumnCaptionSuffix.value[j] || '')
        o['FCaptionLang'] = null
        o['FControl'] = fcontrol
        o['FShowFV'] = '1'
        o['FShowGV'] = '1'
        o['FShowList'] = '1'
        o['FormFieldGuid'] = getNewGuid()
        o['IsRequired'] = '0'
        o['RefCommand'] = fcontrol == 'CBO' ? bodyRow.RefCommand : ''
        o['DefaultValue'] = ''
        o['FShowGroup'] = null
        o['FShowTotal'] = fcontrol == 'INT' || fcontrol == 'NUM' ? '1' : '0'
        o['SummaryType'] = fcontrol == 'INT' || fcontrol == 'NUM' ? 'sum' : ''
        o['Format'] = fcontrol == 'INT' || fcontrol == 'NUM' ? `{format:'${getSystemConfig(fcontrol + 'Format')}'}` : ''
        o['UDF01'] = fcontrol == 'CBO' ? '1' : ''

        grdBody.value.push(o)
      }
    }
  }
  
  setGrdRef()
}

const setGrdRef = () => {
  if (props.multiFields.length > 0) {
    grdRef.value = {}
    for (let i = 0; i < props.multiFields.length; i++) {
      for (let j = 0; j < GRDDynamicColumnControl.value.length; j++) {
        if (GRDDynamicColumnControl.value[j] == 'CBO') {
          if (!isNullOrEmpty(textData.value[i])) {
            grdRef.value[
              textData.value[i] + GRDDynamicColumnCaptionSuffix.value[j]?.trim() || ''
            ] = props.oRef[props.fieldName]
          }
        }
      }
    }
  }
}

const getSystemConfig = (key: string): string => {
  // TODO: 實作系統配置獲取
  return ''
}

const funcGRDBody = async (xmlName: string): Promise<any[]> => {
  // TODO: 實作 funcGRDBody 方法
  return []
}

// ============================================
// 生命週期
// ============================================
onMounted(async () => {
  parentKey.value = isNullOrEmpty(props.controlParent?.keyParameter)
    ? appInfo.emptyGuid || '00000000-0000-0000-0000-000000000000'
    : props.controlParent.keyParameter

  const bodyRow = props.oBody.find((x) => x.FName == props.fieldName)
  if (!bodyRow) return

  GRDXMLName.value = bodyRow.UDF01 || ''
  GRDXMLParameter.value = bodyRow.UDF02 || ''
  GRDJSONDataSource.value = isNullOrEmpty(props.value)
    ? []
    : JSON.parse(props.value || '[]')

  grdPopupWH.value = isNullOrEmpty(bodyRow.UDF05) ? '95%' : bodyRow.UDF05
  controlHeight.value = isNullOrEmpty(bodyRow.UDF06) ? '' : bodyRow.UDF06

  // 判斷是否動態 grd
  if (props.multiFields.length > 0) {
    if (isNullOrEmpty(bodyRow.UDF03)) {
      alert('grd setting error: you have multifield, but no udf03 fcontrol')
      return
    }

    GRDDynamicColumnControl.value = bodyRow.UDF03.split(';')
    GRDDynamicColumnCaptionSuffix.value = isNullOrEmpty(bodyRow.UDF04)
      ? ['']
      : bodyRow.UDF04.split(';')

    if (
      isNullOrEmpty(bodyRow.UDF04) &&
      GRDDynamicColumnControl.value.length > 1
    ) {
      alert('please set dynamic column suffix, must pair with control')
      return
    }

    if (
      GRDDynamicColumnControl.value.length !=
      GRDDynamicColumnCaptionSuffix.value.length
    ) {
      alert('dynamic control count does not match suffix count')
      return
    }

    textBoxReadonly.value = isNullOrEmpty(bodyRow.UDF05)
      ? false
      : bodyRow.UDF05 == '1'

    orgGrdBody.value = await funcGRDBody(GRDXMLName.value)
    grdBody.value = JSON.parse(JSON.stringify(orgGrdBody.value))

    // 放入 textbox 值
    if (props.oData == null) {
      for (let i = 0; i < props.multiFields.length; i++) {
        textData.value[i] = props.cellInfo?.data?.[props.multiFields[i]] || ''
      }
    } else {
      for (let i = 0; i < props.multiFields.length; i++) {
        textData.value[i] = props.oData[props.multiFields[i]] || ''
      }
    }

    renderBody()
  }
})

// ============================================
// Expose
// ============================================
defineExpose({
  refresh,
  getDataSource
})
</script>

<style scoped>
</style>

