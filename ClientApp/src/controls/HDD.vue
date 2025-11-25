<template>
  <div>
    <DxPopup
      ref="HDDPopup"
      width="100%"
      height="100%"
      title="Document"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :visible="HDDPopupShow"
      :full-screen="appInfo.isMobile"
      @hidden="onHDDPopupHidden"
    >
      <template #title>
        <div class="toolbarArea" style="padding: 5px">
          <div class="leftToolbar">
            <img :src="logoUrl" width="120px" />
          </div>
          <div class="rightToolbar">
            <DxButton icon="close" @click="onCloseHDDPopup" />
          </div>
        </div>
      </template>
      <DxScrollView width="100%" height="100%">
        <div>
          <embed :src="fileSrc" width="100%" :height="iframeHeight" />
        </div>
      </DxScrollView>
    </DxPopup>

    <DxPopup
      ref="VDOPopup"
      width="100%"
      height="100%"
      title="Document"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :visible="VDOPopupShow"
      :full-screen="appInfo.isMobile"
      @shown="onVDOPopupShown"
      @hidden="onVDOPopupHidden"
    >
      <template #title>
        <div class="toolbarArea" style="padding: 5px">
          <div class="leftToolbar">
            <img :src="logoUrl" width="120px" />
          </div>
          <div class="rightToolbar">
            <DxButton icon="close" @click="onCloseVDOPopup" />
          </div>
        </div>
      </template>
      <DxScrollView width="100%" height="100%">
        <div>
          <video v-if="VDOPopupShow" width="100%" :height="iframeHeight" controls>
            <source :src="appInfo.serverUrl + fileUrl" type="video/mp4" />
            Your browser does not support the video tag.
          </video>
        </div>
      </DxScrollView>
    </DxPopup>

    <div v-if="!isEditing">
      <a v-if="!isNullOrEmpty(fileUrl)" @click="onFileClick">
        <span :style="hyperLinkStyle">
          <u v-html="docSubject"></u>
        </span>
      </a>
    </div>
    <div v-else style="display: flex; align-items: center">
      <div style="width: 140px">
        <DxFileUploader
          ref="oHDDUpload"
          :multiple="false"
          :upload-url="uploadUrl"
          :upload-headers="uploadHeaders"
          upload-mode="instantly"
          labelText=""
          @uploaded="onUploaded"
          @upload-started="onUploadStarted"
          @upload-aborted="onUploadAborted"
          :accept="accept"
          :allowed-file-extensions="allowFileExtensionArray"
          :max-file-size="maxFileSize"
        />
      </div>
      <div>
        <a v-show="!isNullOrEmpty(fileUrl)" @click="onFileClick">
          <span :style="hyperLinkStyle">{{ fileName }}</span>
        </a>
      </div>

      <div>
        <DxButton
          v-show="isEditing"
          icon="trash"
          :hint="hinttrash"
          @click="onDelete"
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxFileUploader from 'devextreme-vue/file-uploader'
import DxPopup from 'devextreme-vue/popup'
import DxButton from 'devextreme-vue/button'
import DxScrollView from 'devextreme-vue/scroll-view'
import { useControlBase } from '@/composables/useControlBase'
import auth from '@/utils/auth'
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
  setUploadStatus?: (status: string, fieldName: string) => void
  setMultiFieldsValue?: (data?: any, fieldName?: string) => void
  fieldName?: string
  oData?: any
  cellInfo?: any
  controlParent?: any
  propDocSubject?: string
  propCatalogItemID?: string
  propOriginalName?: string
  propACL?: string
}

const props = withDefaults(defineProps<Props>(), {
  isCustom: false,
  isEditing: true,
  value: null,
  onValueChanged: () => {},
  setUploadStatus: () => {},
  setMultiFieldsValue: () => {},
  fieldName: '',
  oData: null,
  cellInfo: null,
  controlParent: null,
  propDocSubject: '',
  propCatalogItemID: '',
  propOriginalName: '',
  propACL: '99'
})

// ============================================
// 使用共用方法
// ============================================
const { isNullOrEmpty, downloadFileByUrl } = useControlBase()

// ============================================
// Computed
// ============================================
const isGrid = computed(() => {
  return props.oData == null && props.cellInfo != null
})

const isForm = computed(() => {
  return props.oData != null && props.cellInfo == null
})

const uploadUrl = computed(() => {
  return `${appInfo.serverUrl || ''}/api/Func/FileUpload?UploadPath=${uploadPath.value}`
})

const logoUrl = computed(() => {
  return `${appInfo.serverUrl || ''}/logo.png`
})

const hyperLinkStyle = computed(() => {
  // TODO: 從 cssVariable 獲取樣式
  return 'cursor: pointer; text-decoration: underline;'
})

// ============================================
// 狀態變數
// ============================================
const fileName = ref('')
const fileUrl = ref('')
const catalogItemID = ref(appInfo.emptyGuid || '00000000-0000-0000-0000-000000000000')
const originalName = ref('')
const originalSubName = ref('')
const fileSize = ref(0)
const docSubject = ref('')
const acl = ref('99')
const HDDPopupShow = ref(false)
const VDOPopupShow = ref(false)
const fileSrc = ref('')
const iframeHeight = ref(800)
const loading = ref(false)
const hinttrash = ref('刪除')
const uploadPath = ref('/upload/cmsresource/')
const accept = ref('*')
const allowFileExtensionArray = ref([
  '.doc',
  '.docx',
  '.rtf',
  '.odt',
  '.txt',
  '.xls',
  '.xlsb',
  '.xlsx',
  '.csv',
  '.ods',
  '.ppt',
  '.pptx',
  '.odp',
  '.vsd',
  '.vsdx',
  '.pdf',
  '.xps',
  '.msg',
  '.mp4',
])
const maxFileSize = ref(152428800)

const uploadHeaders = computed(() => ({
  Authorization: `Bearer ${auth.getToken()}`
}))

// ============================================
// 方法
// ============================================
const onFileClick = async () => {
  if ((fileUrl.value || '').toLowerCase().includes('.jpg')) {
    if (appInfo.isCordova) {
      ;(window as any).cordova.InAppBrowser.open(fileUrl.value, '_blank')
    } else {
      window.open(fileUrl.value, '_blank')
    }
    return
  }

  if (fileUrl.value == 'URL') {
    // TODO: 實作 $bus.emit
    // const pageKey = props.controlParent?.pageKey
    // $bus.emit('CatalogItemID' + pageKey, catalogItemID.value.toUpperCase())
    return
  }

  loading.value = true
  window.sessionStorage.setItem('HDDFileUrl', `${appInfo.serverUrl}${fileUrl.value}`)
  window.sessionStorage.setItem('HDDOriginalFileName', originalName.value)
  window.sessionStorage.setItem('HDDACLID', appInfo.isCordova ? '0' : acl.value)

  // TODO: 實作 ExecuteCMSCommand
  // ExecuteCMSCommand('SetCMSResourceLog.xml', catalogItemID.value)

  const viewerUrl = `${appInfo.serverUrl}/pdfjs2023/web/viewer.html?file=`
  const subName = (fileUrl.value || '').split('.').pop() || ''

  const supportedFormats = [
    'jpg',
    'jpeg',
    'png',
    'bmp',
    'pdf',
    'doc',
    'docx',
    'rtf',
    'odt',
    'txt',
    'xls',
    'xlsb',
    'xlsx',
    'xlsm',
    'csv',
    'ods',
    'ppt',
    'pptx',
    'odp',
    'vsd',
    'vsdx',
    'xps',
    'msg',
  ]

  if (supportedFormats.includes(subName.toLowerCase())) {
    try {
      const res = await apiPost('/api/func/GetCMSResourceFileUrl', {
        originalFileUrl: fileUrl.value,
      })
      if (res.status === 200) {
        const randomParam = new Date().getTime()
        loading.value = false
        HDDPopupShow.value = true
        fileSrc.value = `${viewerUrl}${appInfo.serverUrl}${res.data}?${randomParam}`
      }
    } catch (err) {
      loading.value = false
      console.error('GetCMSResourceFileUrl error:', err)
    }
  } else if (subName.includes('mp4') || subName.includes('wmv')) {
    VDOPopupShow.value = true
  } else {
    if (appInfo.isCordova) {
      ;(window as any).cordova.InAppBrowser.open(
        `${appInfo.serverUrl}${fileUrl.value}`,
        '_blank'
      )
    } else {
      downloadFileByUrl(`${appInfo.serverUrl}${fileUrl.value}`, originalName.value)
    }
  }
}

const onUploadAborted = () => {
  props.setUploadStatus?.('0', props.fieldName)
}

const onUploaded = (e: any) => {
  props.setUploadStatus?.('0', props.fieldName)
  const arrFileName = e.file.name.split('.')
  fileUrl.value = e.request.responseText
  props.onValueChanged?.(fileUrl.value, props.fieldName)
  fileName.value = e.file.name

  if (isForm.value && props.oData) {
    props.oData['OriginalName'] = e.file.name
    props.oData['OriginalSubName'] = arrFileName[arrFileName.length - 1]
    props.oData['DocSize'] = e.file.size
    props.setMultiFieldsValue?.()
  } else if (props.cellInfo) {
    // TODO: 實作 Grid 模式下的 cellValue 更新
    // props.cellInfo.component.cellValue(...)
  }

  fileUrl.value = `${appInfo.serverUrl}${fileUrl.value}`
}

const onDelete = () => {
  fileUrl.value = ''
  fileName.value = ''
  props.setMultiFieldsValue?.({}, props.fieldName)
  
  if (isForm.value && props.oData) {
    props.oData['OriginalName'] = null
    props.oData['OriginalSubName'] = null
    props.oData['DocSize'] = null
  } else if (props.cellInfo) {
    // TODO: 實作 Grid 模式下的 cellValue 更新
  }

  props.onValueChanged?.(fileUrl.value, props.fieldName)
  // TODO: 重置 FileUploader
  // fu.value?.reset()
}

const onUploadStarted = () => {
  props.setUploadStatus?.('1', props.fieldName)
}

const onCloseHDDPopup = () => {
  HDDPopupShow.value = false
}

const onHDDPopupHidden = () => {
  HDDPopupShow.value = false
}

const onVDOPopupShown = () => {
  VDOPopupShow.value = true
}

const onVDOPopupHidden = () => {
  VDOPopupShow.value = false
}

const onCloseVDOPopup = () => {
  VDOPopupShow.value = false
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  if (isNullOrEmpty(props.propDocSubject)) {
    // xml 來的
    if (isGrid.value && props.cellInfo) {
      originalName.value = props.cellInfo.data?.['OriginalName'] || ''
      originalSubName.value = props.cellInfo.data?.['OriginalSubName'] || ''
      fileSize.value = props.cellInfo.data?.['DocSize'] || 0
      docSubject.value = props.cellInfo.data?.['DocSubject'] || ''
      catalogItemID.value = props.cellInfo.data?.['CatalogItemID'] || catalogItemID.value
    } else if (props.cellInfo && !isGrid.value) {
      // isList
      originalName.value = props.cellInfo['OriginalName'] || ''
      originalSubName.value = props.cellInfo['OriginalSubName'] || ''
      fileSize.value = props.cellInfo['DocSize'] || 0
      docSubject.value = props.cellInfo['DocSubject'] || ''
      catalogItemID.value = props.cellInfo['CatalogItemID'] || catalogItemID.value
    } else if (isForm.value && props.oData) {
      originalName.value = props.oData['OriginalName'] || ''
      originalSubName.value = props.oData['OriginalSubName'] || ''
      fileSize.value = props.oData['DocSize'] || 0
      docSubject.value = props.oData['DocSubject'] || ''
      catalogItemID.value = props.oData['CatalogItemID'] || catalogItemID.value
    }
    fileUrl.value = props.value || ''
    fileName.value = originalName.value
  } else {
    // from full text search (ReadOnly)
    catalogItemID.value = props.propCatalogItemID
    fileUrl.value = props.value || ''
    docSubject.value = props.propDocSubject
    originalName.value = props.propOriginalName
    acl.value = props.propACL
  }
  iframeHeight.value = document.body.clientHeight - 80
})

// ============================================
// Expose
// ============================================
defineExpose({
  refresh: () => {
    console.log('not implement yet!')
  }
})
</script>

<style scoped>
</style>

