<template>
  <div :style="cssCustom">
    <DxDataGrid
      ref="FIMGrid"
      :data-source="FIMDatasource"
      @row-removed="onRowRemoved"
      :show-borders="showLine"
      :show-column-lines="showLine"
      :show-row-lines="showLine"
      :hover-state-enabled="true"
      :word-wrap-enabled="false"
      :allow-column-resizing="true"
      :allow-column-reordering="true"
      :column-auto-width="true"
      :height="gvHeight"
      date-serialization-format="yyyy-MM-ddTHH:mm:ss"
      :column-min-width="50"
      :show-column-headers="showHeader"
      :visible="isVisible"
    >
      <DxEditing
        :allow-updating="false"
        :allow-adding="false"
        :allow-deleting="isEditing"
        mode="row"
        :select-text-on-edit-start="true"
        start-edit-action="click"
        :use-icons="true"
      />
      <DxPaging :enabled="false" :page-size="10" />
      <DxScrolling mode="standard" />

      <DxColumn
        v-if="isEditing"
        width="50px"
        :caption="Message('sys_dxColumnCaption')"
        :buttons="['delete']"
        type="buttons"
        :fixed="true"
        fixed-position="left"
      />
      <DxColumn data-field="FileGuid" caption="FileGuid" :visible="false" />
      <DxColumn
        data-field="FileName"
        :caption="Message('control_FIM_FileName')"
        cell-template="TemplateFileName"
      />
      <DxColumn
        :visible="showFileSize"
        data-field="FileSize"
        width="135px"
        editor-type="textBox"
        alignment="right"
        :caption="Message('control_FIM_FileSize')"
      />
      <DxColumn
        :visible="showUploadUser"
        data-field="ModifiedUserName"
        width="155px"
        editor-type="textBox"
        :caption="Message('control_FIM_ModifiedUserName')"
      />
      <DxColumn
        :visible="showUploadDate"
        data-field="ModifiedDate"
        width="155px"
        data-type="datetime"
        :editor-options="{ type: 'datetime', displayFormat: 'yyyy-MM-dd' }"
        :caption="Message('control_FIM_ModifiedDate')"
        sort-order="desc"
      />
      <DxColumn data-field="FileUrl" caption="FileUrl" :visible="false" />
      <DxColumn data-field="ModifiedUserID" :visible="false" caption="Upload User" />

      <DxToolbar v-if="isEditing">
        <DxItem
          location="after"
          name="uploadForm"
          :visible="true"
          widget="dxButton"
          :options="uploadForm"
        />
      </DxToolbar>

      <template #TemplateFileName="{ data: cellInfo }">
        <div :style="cssPOHStyle" @click="() => onFileClick(cellInfo)">
          <div :style="cssPOHStyle">{{ cellInfo.value }}</div>
        </div>
      </template>
    </DxDataGrid>

    <DxLoadPanel
      :hide-on-outside-click="false"
      :visible="loading"
      :shading="true"
      :show-pane="true"
      shading-color="transparent"
    />

    <DxPopup
      ref="FIMPopup"
      :title="appInfo.title"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :visible="isFIMPopupVisible"
      :width="appInfo.isMobile ? '100%' : popupWidth"
      :height="appInfo.isMobile ? '100%' : popupHeight"
      :full-screen="appInfo.isMobile"
      @hidden="onFIMPopupHidden"
    >
      <template #title>
        <div class="toolbarArea" style="padding: 5px">
          <div class="leftToolbar">
            <img :src="logoUrl" width="120px" />
          </div>
          <div class="rightToolbar">
            <DxButton icon="close" @click="onCloseFIMPopup" />
          </div>
        </div>
      </template>
      <DxScrollView width="100%" height="100%" :tab-index="-1">
        <div>
          <DxFileUploader
            ref="oCustomDxFileUploader"
            :multiple="true"
            :upload-url="uploadUrl"
            :upload-headers="uploadHeaders"
            upload-mode="useButtons"
            @value-changed="onChanged"
            @files-uploaded="onFilesUploaded"
            @progress="onProgress"
            @uploaded="onUploaded"
            :accept="accept"
            :allowed-file-extensions="allowFileExtensionArray"
            :max-file-size="maxFileSize"
          />
        </div>
      </DxScrollView>
    </DxPopup>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import {
  DxDataGrid,
  DxPaging,
  DxScrolling,
  DxColumn,
  DxEditing,
  DxToolbar,
  DxItem,
} from 'devextreme-vue/data-grid'
import DxFileUploader from 'devextreme-vue/file-uploader'
import DxPopup from 'devextreme-vue/popup'
import DxButton from 'devextreme-vue/button'
import DxScrollView from 'devextreme-vue/scroll-view'
import DxLoadPanel from 'devextreme-vue/load-panel'
import { useControlBase } from '@/composables/useControlBase'
import auth from '@/utils/auth'
import { apiPost, apiDeleteFile } from '@/utils/api-util'
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
  controlParent?: any
}

const props = withDefaults(defineProps<Props>(), {
  isCustom: false,
  isEditing: true,
  value: null,
  onValueChanged: () => {},
  fieldName: '',
  oBody: () => [],
  oData: null,
  controlParent: null
})

// ============================================
// 使用共用方法
// ============================================
const { isNullOrEmpty, getFieldFormat, dateToString, formatComma, downloadFileByUrl, openFileWithGoogleDoc } = useControlBase()

// ============================================
// Computed
// ============================================
const isForm = computed(() => {
  return props.oData != null
})

const uploadUrl = computed(() => {
  return `${appInfo.serverUrl || ''}/api/Func/FileUpload?UploadPath=${uploadPath.value}`
})

const logoUrl = computed(() => {
  return `${appInfo.serverUrl || ''}/logo.png`
})

const cssCustom = computed(() => {
  return {
    '--ccolor': 'transparent'
  }
})

const isVisible = computed(() => {
  return props.isEditing || FIMDatasource.value.length > 0
})

const showLine = computed(() => {
  return props.isEditing || isForm.value
})

// ============================================
// 狀態變數
// ============================================
const FIMDatasource = ref<any[]>([])
const uploadPath = ref('/upload')
const allowFileExtensionArray = ref<string[]>([])
const allowFileExtension = ref('')
const accept = ref('')
const maxFileSize = ref(52428800)
const gvHeight = ref('inherit')
const popupWidth = ref('100%')
const popupHeight = ref('100%')
const showFileSize = ref(true)
const showUploadUser = ref(true)
const showUploadDate = ref(true)
const files = ref<any[]>([])
const loading = ref(false)
const cssPOHStyle = ref(
  'overflow:hidden;white-space:nowarp;text-overflow:ellipsis;cursor:pointer;text-decoration: underline;'
)
const ServerUrl = ref<string | null>(null)
const showHeader = ref(true)
const isDeleteServerFile = ref(true)
const isFIMPopupVisible = ref(false)

const uploadHeaders = computed(() => ({
  Authorization: `Bearer ${auth.getToken()}`
}))

const uploadForm = computed(() => ({
  icon: 'plus',
  hint: Message('sys_add'),
  type: 'normal',
  onClick: openUploadForm
}))

// ============================================
// 方法
// ============================================
const Message = (key: string): string => {
  // TODO: 實作多語系訊息
  return key
}

const refresh = () => {
  if (props.controlParent?.oData) {
    FIMDatasource.value = isNullOrEmpty(props.controlParent.oData[props.fieldName])
      ? []
      : JSON.parse(props.controlParent.oData[props.fieldName] || '[]')
  }
}

const onRowRemoved = (e: any) => {
  props.onValueChanged?.(JSON.stringify(FIMDatasource.value), props.fieldName)
  if (isDeleteServerFile.value) {
    apiDeleteFile(e.data.FileUrl).catch((err) => {
      console.error('Delete file error:', err)
    })
  }
}

const onChanged = (e: any) => {
  files.value = e.value || []
}

const onProgress = () => {
  loading.value = true
}

const onUploaded = (e: any) => {
  const fileIndex = FIMDatasource.value.length
  const oFile: any = {}
  oFile['FileName'] = e.file.name
  const arrFileName = e.request.responseText.split('/')
  const f = arrFileName[arrFileName.length - 1]
  const FileGuid = f.split('.')[0]
  oFile['FileSubName'] = f.split('.')[1]
  oFile['FileGuid'] = FileGuid
  oFile['FileUrl'] = e.request.responseText
  const today = new Date()
  oFile['ModifiedDate'] = dateToString(today, 'yyyy-MM-ddTHH:mm:ss')
  oFile['ModifiedUserID'] = appInfo.userInfo.userGuid
  oFile['ModifiedUserName'] = appInfo.userInfo.userName
  oFile['FileSize'] =
    formatComma((Math.round((e.file.size / 1024.0) * 100) / 100).toString()) + ' KB'
  oFile['TimeStamp'] = new Date().getTime()
  FIMDatasource.value.push(oFile)
}

const onFilesUploaded = () => {
  loading.value = false
  props.onValueChanged?.(JSON.stringify(FIMDatasource.value), props.fieldName)
  // TODO: 實作 grid refresh
  // fg.value?.refresh()
  isFIMPopupVisible.value = false
  // TODO: 重置 FileUploader
  // fu.value?.reset()
  files.value = []
}

const onFileClick = (cellInfo: any) => {
  const fileUrl = `${ServerUrl.value || appInfo.serverUrl}${cellInfo.data.FileUrl}`
  if (appInfo.isCordova) {
    if (openFileWithGoogleDoc(fileUrl)) {
      const viewerUrl =
        'https://docs.google.com/viewer?url=' +
        encodeURIComponent(fileUrl.toLowerCase()) +
        '&embedded=true'
      ;(window as any).cordova.InAppBrowser.open(
        viewerUrl,
        '_blank',
        'location=no,toolbar=yes,hardwareback=yes'
      )
    } else {
      const viewerUrl = `${appInfo.serverUrl}/pdfjs2023/web/viewer.html?file=${fileUrl}`
      ;(window as any).cordova.InAppBrowser.open(
        viewerUrl,
        '_blank',
        'location=no,toolbar=yes,hardwareback=yes'
      )
    }
  } else {
    if (ServerUrl.value != appInfo.serverUrl) {
      window.open(fileUrl, '_blank')
    } else {
      downloadFileByUrl(fileUrl, cellInfo.data.FileName)
    }
  }
}

const openUploadForm = () => {
  files.value = []
  // TODO: 重置 FileUploader
  // fu.value?.reset()
  isFIMPopupVisible.value = true
}

const onCloseFIMPopup = () => {
  isFIMPopupVisible.value = false
}

const onFIMPopupHidden = () => {
  isFIMPopupVisible.value = false
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  FIMDatasource.value = isNullOrEmpty(props.value) ? [] : JSON.parse(props.value || '[]')
  
  const bodyRow = props.oBody.find((x) => x.FName == props.fieldName)
  if (!bodyRow) return

  const oformat = getFieldFormat(bodyRow.Format)
  uploadPath.value = oformat.uploadPath == undefined ? '/upload/' : oformat.uploadPath

  allowFileExtension.value = isNullOrEmpty(bodyRow.UDF01) ? '' : bodyRow.UDF01.trim()
  maxFileSize.value = isNullOrEmpty(bodyRow.UDF02)
    ? maxFileSize.value
    : parseInt(bodyRow.UDF02)

  if (isNullOrEmpty(allowFileExtension.value)) accept.value = '*'
  else accept.value = allowFileExtension.value.replace(/;/g, ',')
  if (!isNullOrEmpty(allowFileExtension.value)) {
    allowFileExtension.value = allowFileExtension.value.replace(/;/g, ',')
  }
  allowFileExtensionArray.value = isNullOrEmpty(allowFileExtension.value)
    ? []
    : allowFileExtension.value.split(',')

  if (!isNullOrEmpty(bodyRow.UDF03)) {
    const captionSwitch = bodyRow.UDF03.replaceAll(';', ',').split(',')
    if (captionSwitch.length > 0)
      showFileSize.value = captionSwitch[0] == '1'
    if (captionSwitch.length > 1)
      showUploadUser.value = captionSwitch[1] == '1'
    if (captionSwitch.length > 2)
      showUploadDate.value = captionSwitch[2] == '1'
  }

  if (!isNullOrEmpty(bodyRow.UDF04)) {
    gvHeight.value = bodyRow.UDF04
  }

  if (!isNullOrEmpty(bodyRow.UDF05)) {
    popupWidth.value = popupHeight.value = bodyRow.UDF05
  }

  if (!isNullOrEmpty(bodyRow.UDF06)) ServerUrl.value = bodyRow.UDF06
  else ServerUrl.value = appInfo.serverUrl

  if (!isNullOrEmpty(bodyRow.UDF07)) {
    isDeleteServerFile.value = bodyRow.UDF07 == '1'
  } else {
    isDeleteServerFile.value = false
  }

  if (!props.isEditing && !isForm.value) {
    showHeader.value = false
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
.dx-datagrid-header-panel {
  background-color: var(--ccolor);
}

.dx-toolbar {
  background-color: var(--ccolor);
}
</style>

