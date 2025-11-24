<template>
  <div>
    <div v-if="isEditing">
      <DxHtmlEditor
        :read-only="!isEditing"
        v-show="isEditing && radioGroupValue == '0'"
        width="100%"
        :height="height"
        v-model:value="markup"
        @value-changed="setValue"
      >
        <DxMediaResizing :enabled="true" />
        <DxImageUpload :tabs="['file', 'url']" file-upload-mode="base64" />
        <DxToolbar v-if="!hideToolBar" :multiline="false">
          <DxItem name="undo" />
          <DxItem name="redo" />
          <DxItem name="separator" />
          <DxItem :accepted-values="sizeValues" name="size" />
          <DxItem :accepted-values="fontValues" name="font" />
          <DxItem name="separator" />
          <DxItem name="bold" />
          <DxItem name="italic" />
          <DxItem name="strike" />
          <DxItem name="underline" />
          <DxItem name="separator" />
          <DxItem name="alignLeft" />
          <DxItem name="alignCenter" />
          <DxItem name="alignRight" />
          <DxItem name="alignJustify" />
          <DxItem name="separator" />
          <DxItem name="orderedList" />
          <DxItem name="bulletList" />
          <DxItem name="separator" />
          <DxItem :accepted-values="headerValues" name="header" />
          <DxItem name="separator" />
          <DxItem name="color" />
          <DxItem name="background" />
          <DxItem name="separator" />
          <DxItem name="link" />
          <DxItem name="image" />
          <DxItem name="separator" />
          <DxItem name="clear" />
          <DxItem name="codeBlock" />
          <DxItem name="blockquote" />
          <DxItem name="separator" />
          <DxItem name="insertTable" />
          <DxItem name="deleteTable" />
          <DxItem name="insertRowAbove" />
          <DxItem name="insertRowBelow" />
          <DxItem name="deleteRow" />
          <DxItem name="insertColumnLeft" />
          <DxItem name="insertColumnRight" />
          <DxItem name="deleteColumn" />
        </DxToolbar>
      </DxHtmlEditor>
    </div>
    <div v-if="!isEditing">
      <div
        :class="markupClass"
        v-html="markup"
        v-show="!isEditing"
        @click="onItemClick"
      />
    </div>
    <DxPopup
      ref="rootPopup"
      width="100%"
      height="100%"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :visible="isRootPopupVisible"
      :full-screen="appInfo.isMobile"
      @hidden="onRootPopupHidden"
    >
      <template #title>
        <div class="toolbarArea" style="padding: 5px">
          <div class="leftToolbar">
            <img :src="logoUrl" width="120px" />
          </div>
          <div class="rightToolbar">
            <DxButton icon="close" @click="onCloseRootPopup" />
          </div>
        </div>
      </template>
      <DxScrollView width="100%" height="100vh">
        <div>
          <!-- TODO: 需要實作 CMSFrame 組件 -->
          <div v-if="isRootPopup && !isEditing" style="padding: 20px;">
            <p>CMSFrame 組件尚未實作</p>
            <p>URL: {{ popupUrl }}</p>
          </div>
        </div>
      </DxScrollView>
    </DxPopup>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  DxHtmlEditor,
  DxToolbar,
  DxMediaResizing,
  DxImageUpload,
  DxItem,
} from 'devextreme-vue/html-editor'
import DxPopup from 'devextreme-vue/popup'
import DxButton from 'devextreme-vue/button'
import DxScrollView from 'devextreme-vue/scroll-view'
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
  fieldName?: string
  oBody?: any[]
  oData?: any
  cellInfo?: any
}

const props = withDefaults(defineProps<Props>(), {
  isCustom: false,
  isEditing: true,
  value: null,
  onValueChanged: () => {},
  fieldName: '',
  oBody: () => [],
  oData: null,
  cellInfo: null
})

// ============================================
// 使用共用方法
// ============================================
const { isNullOrEmpty, getFieldFormat } = useControlBase()

// ============================================
// Computed
// ============================================
const isForm = computed(() => {
  return props.oData != null && props.cellInfo == null
})

const logoUrl = computed(() => {
  return `${appInfo.serverUrl || ''}/logo.png`
})

// ============================================
// 狀態變數
// ============================================
const popupUrl = ref('')
const isRootPopup = ref(false)
const isRootPopupVisible = ref(false)
const markup = ref('')
const sizeValues = ref([
  '8pt',
  '10pt',
  '12pt',
  '14pt',
  '16pt',
  '18pt',
  '20pt',
  '22pt',
  '24pt',
  '36pt',
  '48pt',
  '72pt',
])
const fontValues = ref([
  'Arial',
  'Courier New',
  'Georgia',
  'Impact',
  'Lucida Console',
  'Tahoma',
  'Times New Roman',
  'Verdana',
  '微軟正黑體',
  '標楷體',
  '新細明體',
  '細明體',
])
const headerValues = ref([false, 1, 2, 3, 4, 5])
const radioGroupValue = ref('0')
const markupClass = ref('options')
const hideToolBar = ref(false)
const height = ref('100%')

// ============================================
// 方法
// ============================================
const refresh = () => {
  console.log('not implement yet!')
}

const setValue = () => {
  props.onValueChanged?.(markup.value, props.fieldName)
}

const onItemClick = () => {
  if (!isNullOrEmpty(popupUrl.value)) {
    isRootPopupVisible.value = true
    isRootPopup.value = true
  }
}

const onCloseRootPopup = () => {
  isRootPopupVisible.value = false
  isRootPopup.value = false
}

const onRootPopupHidden = () => {
  isRootPopup.value = false
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  markup.value = props.value || ''

  const bodyRow = props.oBody.find((x) => x.FName == props.fieldName)
  if (bodyRow) {
    markupClass.value = bodyRow.UDF01 == '1' ? 'options' : ''
    hideToolBar.value = bodyRow.UDF02 == '1'

    if (!isNullOrEmpty(bodyRow.Format)) {
      const o = getFieldFormat(bodyRow.Format)
      if (o.height != undefined) height.value = o.height
    }

    if (isForm.value && props.oData) {
      popupUrl.value = props.oData[bodyRow.UDF05] || ''
    } else if (props.cellInfo) {
      popupUrl.value = props.cellInfo.data?.[bodyRow.UDF05] || ''
    }
  }
})

// ============================================
// Expose
// ============================================
defineExpose({
  refresh
})
</script>

<style lang="scss" scoped>
.dx-htmleditor-content img {
  vertical-align: middle;
  padding-right: 10px;
}

.options {
  padding: 0px;
  margin: -10px -15px -10px -15px;
}

.caption {
  font-size: 18px;
  font-weight: 500;
}

.option {
  margin-top: 10px;
}
</style>

