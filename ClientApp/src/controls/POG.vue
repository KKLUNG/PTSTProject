<template>
  <div v-show="isNullOrEmpty(orgPopupUrl) && !isEditing">
    <div v-if="isForm || (cellInfo && cellInfo.isOnForm)">
      <DxTextBox :value="displayValue" :read-only="true" />
    </div>
    <div v-else>
      {{ displayValue }}
    </div>
  </div>
  <div v-show="!(isNullOrEmpty(orgPopupUrl) && !isEditing)" style="width: 100%">
    <div class="formButtonArea1">
      <div>
        <DxTagBox
          ref="tagBox"
          :read-only="!isEditing"
          v-model:value="dxTagBoxKeys"
          :data-source="{
            store: {
              type: 'array',
              data: dataSource,
            },
          }"
          display-expr="display"
          :show-clear-button="false"
          value-expr="value"
          placeholder=""
          :search-enabled="false"
          :show-selection-controls="false"
          :show-drop-down-button="false"
          :open-on-field-click="false"
          @value-changed="onTagBoxValueChanged"
          tag-template="tagTemplate"
          styling-mode="underlined"
        >
          <template #tagTemplate="{ data }">
            <div v-if="!isEditing">
              <div>
                <span
                  style="text-decoration: underline"
                  @click.self="onTagItemClick(data.value)"
                >
                  {{ data.display }}
                </span>
              </div>
            </div>
            <div v-else>
              <div class="dx-tag-content">
                <span @click.self="onTagItemClick(data.value)">
                  {{ data.display }}
                </span>
                <div class="dx-tag-remove-button"></div>
              </div>
            </div>
          </template>
        </DxTagBox>
      </div>
      <div v-if="isEditing">
        <DxButton
          icon="check"
          :text="isNullOrEmpty(buttonText) ? Message('sys_select') : buttonText"
          :visible="isEditing"
          styling-mode="outlined"
          type="default"
          @click="onButtonClick"
        />
      </div>
    </div>
    <div v-if="isGrid">
      <hr
        style="position: relative; top: 8px; border: 0.1px solid"
        v-if="isEditing"
      />
    </div>
    <div v-else>
      <hr style="position: relative; top: 8px; border: 0.1px solid" />
    </div>

    <DxPopup
      ref="selectionPopup"
      :width="popupWidth"
      :height="popupHeight"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :visible="isSelectionPopupVisible"
      @shown="onSelectionPopupShown"
      @hidden="onSelectionPopupHidden"
    >
      <template #title>
        <div class="toolbarArea" style="padding: 5px">
          <div class="leftToolbar">
            <img :src="logoUrl" width="120px" />
          </div>
          <div class="rightToolbar">
            <DxButton icon="close" @click="onCloseSelectionPopup" />
          </div>
        </div>
      </template>
      <DxScrollView width="100%" height="100%">
        <div style="padding: 5px">
          <AdminGridForm
            v-if="isSelectionPopup"
            :ref="fieldName"
            :XMLName="selectionXML"
            :XMLParameter="xmlParameter"
            @onSelectionReturn="onSelectionSave"
          />
        </div>
      </DxScrollView>
    </DxPopup>

    <DxPopup
      ref="tagPopup"
      :width="popupWidth"
      :height="popupHeight"
      title="Content"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :visible="isTagPopupVisible"
      @shown="onTagPopupShown"
      @hidden="onTagPopupHidden"
    >
      <template #title>
        <div class="toolbarArea" style="padding: 5px">
          <div class="leftToolbar">
            <img :src="logoUrl" width="120px" />
          </div>
          <div class="rightToolbar">
            <DxButton icon="close" @click="onCloseTagPopup" />
          </div>
        </div>
      </template>

      <DxScrollView width="100%" height="100%">
        <div>
          <!-- TODO: 需要實作 CMSFrame 組件 -->
          <div v-if="isRootPopup" style="padding: 20px;">
            <p>CMSFrame 組件尚未實作</p>
            <p>URL: {{ popupUrl }}</p>
          </div>
        </div>
      </DxScrollView>
    </DxPopup>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import DxTagBox from 'devextreme-vue/tag-box'
import DxTextBox from 'devextreme-vue/text-box'
import DxButton from 'devextreme-vue/button'
import DxPopup from 'devextreme-vue/popup'
import DxScrollView from 'devextreme-vue/scroll-view'
import AdminGridForm from '@/components/AdminGridForm.vue'
import { useControlBase } from '@/composables/useControlBase'
import { apiPost } from '@/utils/api-util'
import appInfo from '@/utils/app-Info'

// ============================================
// Props
// ============================================
interface Props {
  isCustom?: boolean
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  setMultiFieldsValue?: (flag?: boolean) => void
  fieldName?: string
  oBody?: any[]
  oData?: any
  cellInfo?: any
  dataSource?: any[]
  XMLName?: string
  buttonText?: string
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
  cellInfo: null,
  dataSource: () => [],
  XMLName: '',
  buttonText: ''
})

// ============================================
// 使用共用方法
// ============================================
const { isNullOrEmpty } = useControlBase()

// ============================================
// Computed
// ============================================
const isGrid = computed(() => {
  return props.oData == null && props.cellInfo != null
})

const isForm = computed(() => {
  return props.oData != null && props.cellInfo == null
})

const logoUrl = computed(() => {
  return `${appInfo.serverUrl || ''}/logo.png`
})

// ============================================
// 狀態變數
// ============================================
const isSelectionPopup = ref(false)
const isSelectionPopupVisible = ref(false)
const dxTagBoxKeys = ref<string[]>([])
const selectionXML = ref('')
const xmlParameter = ref('')
const displayValue = ref('')
const popupUrl = ref('')
const orgPopupUrl = ref('')
const isRootPopup = ref(false)
const isTagPopupVisible = ref(false)
const tagSelectCommand = ref('')
const popupWidth = ref(appInfo.isMobile ? '100%' : '98vw')
const popupHeight = ref(appInfo.isMobile ? '100%' : '98vh')
const FocusIn = ref('')

// ============================================
// 方法
// ============================================
const Message = (key: string): string => {
  // TODO: 實作多語系訊息
  return key
}

const refresh = () => {
  console.log('not implement yet!')
}

const onButtonClick = () => {
  if (!props.isCustom) {
    const bodyRow = props.oBody.find((x) => x.FName == props.fieldName)
    if (bodyRow) {
      selectionXML.value = bodyRow.UDF04 || ''

      if (!isNullOrEmpty(bodyRow.UDF05) && props.isEditing) {
        const filterValue =
          props.oData == null
            ? props.cellInfo?.data?.[bodyRow.UDF05]
            : props.oData[bodyRow.UDF05]
        xmlParameter.value = filterValue || ''
      }
    }
  }
  isSelectionPopupVisible.value = true
}

const onSelectionSave = async (e: string) => {
  const parts = e.split('|')
  const singleMulti = parts[0]
  const keys = parts[1]?.split(',') || []

  if (singleMulti == 's') dxTagBoxKeys.value = []

  for (let i = 0; i < keys.length; i++) {
    if (!dxTagBoxKeys.value.includes(keys[i])) {
      dxTagBoxKeys.value.push(keys[i])
    }
  }

  // 帶入資料功能, 只有單選, 有 SELECTCOMMAND(UDF03) 才有用
  if (!isNullOrEmpty(tagSelectCommand.value) && singleMulti == 's') {
    let ssql = tagSelectCommand.value
    ssql = ssql.replaceAll('{0}', dxTagBoxKeys.value.toString())

    try {
      const res = await apiPost('/api/Func/ExecuteSQL', { ssql })
      if (res.status === 200 && res.data && res.data[0]) {
        const tagSelectedData = res.data[0]
        const keys = Object.keys(tagSelectedData)

        if (isForm.value && props.oData) {
          Object.assign(props.oData, tagSelectedData)
          props.setMultiFieldsValue?.(true)
        } else if (props.cellInfo) {
          // TODO: 實作 Grid 模式下的 cellValue 更新
          // for (let j = 0; j < keys.length; j++) {
          //   props.cellInfo.component.cellValue(
          //     props.cellInfo.row.rowIndex,
          //     keys[j],
          //     tagSelectedData[keys[j]]
          //   )
          // }
        }
      }
    } catch (err) {
      console.error('ExecuteSQL error:', err)
    }
  }

  // TODO: 實作 FocusIn 邏輯
  // if (!isNullOrEmpty(FocusIn.value)) {
  //   setTimeout(() => {
  //     eval(FocusIn.value)
  //   }, 100)
  // }

  props.onValueChanged?.(dxTagBoxKeys.value.toString(), props.fieldName)
  isSelectionPopupVisible.value = false

  // TODO: 實作 tagBox repaint
  // if (tagBox.value) tagBox.value.repaint()
}

const onTagBoxValueChanged = (e: any) => {
  dxTagBoxKeys.value = e.value || []
  props.onValueChanged?.(dxTagBoxKeys.value.toString(), props.fieldName)
}

const onTagItemClick = (key: string) => {
  if (!isNullOrEmpty(orgPopupUrl.value)) {
    popupUrl.value = orgPopupUrl.value

    if (popupUrl.value.includes('{0}')) {
      popupUrl.value = popupUrl.value.replaceAll('{0}', key)
    } else {
      const o = props.dataSource.find((x) => x.value == key)
      if (o) {
        popupUrl.value = o[popupUrl.value] || ''
      }
    }
    isTagPopupVisible.value = true
  }
}

const onSelectionPopupShown = () => {
  isSelectionPopup.value = true
}

const onSelectionPopupHidden = () => {
  isSelectionPopup.value = false
}

const onCloseSelectionPopup = () => {
  isSelectionPopupVisible.value = false
}

const onTagPopupShown = () => {
  isRootPopup.value = true
}

const onTagPopupHidden = () => {
  isRootPopup.value = false
}

const onCloseTagPopup = () => {
  isTagPopupVisible.value = false
}

// ============================================
// Watch
// ============================================
watch(
  dxTagBoxKeys,
  (v, q) => {
    if (v.toString() != q.toString()) {
      // TODO: 實作 FocusIn 邏輯
      // if (!isNullOrEmpty(FocusIn.value)) {
      //   setTimeout(() => {
      //     eval(FocusIn.value)
      //   }, 100)
      // }
    }
  },
  { deep: true }
)

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  if (!isNullOrEmpty(props.value)) {
    const values = (props.value || '').split(',')
    for (let i = 0; i < values.length; i++) {
      dxTagBoxKeys.value.push(values[i].toLowerCase())
      const oo = props.dataSource.find(
        (r) => r.value.toUpperCase() == values[i].toUpperCase()
      )
      if (oo) displayValue.value += oo.display + ', '
    }

    displayValue.value = displayValue.value.substring(
      0,
      displayValue.value.length - 2
    )
  }

  if (!props.isCustom) {
    const bodyRow = props.oBody.find((x) => x.FName == props.fieldName)
    if (bodyRow) {
      selectionXML.value = bodyRow.UDF04 || ''

      if (!isNullOrEmpty(bodyRow.UDF02)) {
        orgPopupUrl.value = bodyRow.UDF02
      }

      if (!isNullOrEmpty(bodyRow.UDF03)) {
        tagSelectCommand.value = bodyRow.UDF03
      }

      if (!isNullOrEmpty(bodyRow.FocusIn)) {
        FocusIn.value = bodyRow.FocusIn
      }
    }
  } else {
    selectionXML.value = props.XMLName
  }
})

// ============================================
// Expose
// ============================================
defineExpose({
  refresh
})
</script>

<style scoped>
</style>

