<template>
  <div>
    <RootPopup
      ref="TTTPopupRef"
      :title="appInfo?.title || ''"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :visible="false"
      :width="appInfo?.isMobile ? '100%' : popupWidth"
      :height="appInfo?.isMobile ? '100%' : popupHeight"
      :full-screen="appInfo?.isMobile ? true : false"
      @hidden="() => { popupInstance?.hide() }"
    >
      <template #titleTemplate>
        <div class="toolbarArea" style="padding: 5px">
          <div class="leftToolbar">
            <img :src="logoUrl" width="120px" />
          </div>
          <div class="rightToolbar">
            <DxButton
              icon="icon dx-icon-close"
              @click="() => { popupInstance?.hide() }"
            />
          </div>
        </div>
      </template>
      <DxScrollView width="100%" height="100%" :tab-index="-1">
        <div v-html="displayText"></div>
      </DxScrollView>
    </RootPopup>
    <div v-if="isEditing">
      <DxTextArea
        :auto-resize-enabled="true"
        v-model:value="textValue"
        :max-height="height"
        :placeholder="placeholder"
        @value-changed="onTextValueChanged"
        :read-only="!isEditing"
        @key-down="onKeyDown"
        :tab-index="base_controlIndex"
      />
    </div>
    <div v-else :style="styleDivWidth">
      <div v-if="isClickWithPopup"
           :style="readOnlyStyle"
           @click="popupTextArea">
        {{ textValue }}
      </div>
      <div v-else :style="htmlStyle" v-html="displayText"></div>
      <hr v-show="isShowLine" style="border-bottom: 0.5px dotted rgba(255, 255, 255, .42);" />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxTextArea from 'devextreme-vue/text-area'
import DxButton from 'devextreme-vue/button'
import { DxPopup as RootPopup } from 'devextreme-vue/popup'
import { DxScrollView } from 'devextreme-vue/scroll-view'
import { useControlBase } from '@/composables/useControlBase'

interface Props {
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  fieldName?: string
  oBody?: any[]
  controlParent?: any
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: true,
  value: null,
  onValueChanged: () => () => {},
  fieldName: '',
  oBody: () => [],
  controlParent: null
})

const { isNullOrEmpty, appInfo, getFieldFormat } = useControlBase()

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

const TTTPopupRef = ref()
const popupInstance = computed(() => {
  return TTTPopupRef.value?.instance
})

const displayText = ref<string>('')
const textValue = ref<string>('')
const height = ref<string | undefined>(undefined)
const placeholder = ref<string>('')
const popupWidth = ref<string>('640px')
const popupHeight = ref<string>('480px')
const isClickWithPopup = ref<boolean>(false)
const logoUrl = ref<string>('../images/logo.png')

const readOnlyStyle = computed(() => {
  if (isClickWithPopup.value && isGrid.value && props.controlParent?.oHead?.HasWordWarp != '1') {
    return "overflow:hidden;white-space:nowrap;text-overflow:ellipsis;cursor:pointer;text-decoration: underline;"
  } else {
    return height.value == undefined
      ? {}
      : {
          "max-height": height.value,
          width: styleDivWidth.value,
          overflow: "auto",
        }
  }
})

const htmlStyle = computed(() => {
  if (isClickWithPopup.value && isGrid.value && props.controlParent?.oHead?.HasWordWarp != '1') {
    return "overflow:hidden;white-space:nowrap;text-overflow:ellipsis;cursor:pointer;text-decoration: underline;"
  } else {
    return isNullOrEmpty(displayText.value) 
      ? { "height": '24px' } 
      : height.value == undefined
        ? {}
        : {
            maxHeight: height.value,
            width: styleDivWidth.value,
            overflow: "auto",
          }
  }
})

const styleDivWidth = computed(() => {
  if (props.controlParent?.oHead?.FormMode == "form") return "width:55%"
  else return "width:40%"
})

const isShowLine = computed(() => {
  return isGrid.value ? false : true
})

const onTextValueChanged = () => {
  displayText.value = textValue.value.replace(/(?:\r\n|\r|\n)/g, "<br />")
  props.onValueChanged?.(textValue.value, props.fieldName || '')
}

const onKeyDown = (e: any) => {
  if (e.event.keyCode === 13) {
    e.event.stopPropagation()
  }
}

const popupTextArea = () => {
  if (isClickWithPopup.value && !isShowLine.value) {
    popupInstance.value?.show()
  }
}

onMounted(() => {
  if (props.oBody && props.oBody.length > 0) {
    const row = props.oBody.filter((x: any) => x["FName"] == props.fieldName)
    if (row.length > 0) {
      placeholder.value = row[0].Placeholder || ''
      
      if (!isNullOrEmpty(row[0].Format)) {
        const o = getFieldFormat(row[0].Format)
        if (o.height != undefined) {
          height.value = o.height
        } else {
          height.value = undefined
        }
      }
      
      isClickWithPopup.value = row[0].UDF01 == "1" ? true : false
    }
  }

  textValue.value = props.value || ''
  displayText.value = isNullOrEmpty(textValue.value)
    ? ""
    : textValue.value.replace(/(?:\r\n|\r|\n)/g, "<br />")
})
</script>

<style lang="scss" scoped>
</style>

