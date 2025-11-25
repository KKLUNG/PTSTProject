<template>
  <div style="display: flex; align-items: center">
    <div v-if="isEditing" style="width: 140px">
      <DxFileUploader
        ref="oCustomDxFileUploader"
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

    <!-- 開檔用 download -->
    <div
      v-show="showStyle == '2' && !isNullOrEmpty(fileUrl)"
      :style="cssPOHStyle"
      @click="onFileClick"
    >
      <div :style="cssPOHStyle">{{ fileName }}</div>
    </div>

    <!-- 開檔用 icon HyperLink -->
    <div v-if="showStyle == '3' && !isNullOrEmpty(fileUrl)">
      <a @click="onHyperlinkClick">
        <i class="dx-icon-file" :style="fileIconStyle" />
      </a>
    </div>

    <!-- FTP開檔用 download -->
    <div
      v-show="showStyle == '6' && !isNullOrEmpty(fileUrl)"
      :style="cssPOHStyle"
      @click="onFTPFileClick"
    >
      <div :style="cssPOHStyle">{{ fileName }}</div>
    </div>

    <!-- VDO -->
    <div>
      <video
        v-show="showStyle == '4' && !isNullOrEmpty(fileUrl)"
        :src="fileUrl"
        width="100%"
        height="450"
        controls
        controlslist="nodownload"
        oncontextmenu="return false;"
      />
    </div>

    <!-- PIC -->
    <DxPopup
      v-if="showStyle == '1'"
      ref="picPopup"
      width="90%"
      height="90%"
      title="XMLEditor"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :visible="isPicPopup"
      @hidden="onPicPopupHidden"
    >
      <template #title>
        <div class="toolbarArea" style="padding: 5px">
          <div class="leftToolbar">
            <img :src="logoUrl" width="120px" />
          </div>
          <div class="rightToolbar">
            <DxButton icon="close" @click="onClosePicPopup" />
          </div>
        </div>
      </template>
      <DxScrollView width="100%" height="100%">
        <div style="width: inherit">
          <img :src="fileUrl" style="width: 100%" />
        </div>
      </DxScrollView>
    </DxPopup>

    <div
      class="image-container"
      v-show="showStyle == '1' && !isNullOrEmpty(fileUrl)"
      :style="imageStyle"
    >
      <div
        class="user-image"
        @click="onPicClick"
        :style="{
          backgroundImage:
            'url(' +
            (isNullOrEmpty(fileUrl)
              ? appInfo.serverUrl + '/upload/noimage.png'
              : fileUrl) +
            ')',
        }"
      ></div>
    </div>

    <div v-if="isEditing">
      <DxButton
        v-show="!isNullOrEmpty(fileUrl)"
        icon="trash"
        :hint="hinttrash"
        @click="onDelete"
      />
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
  setUploadStatus?: (status: string, fieldName: string) => void
  setMultiFieldsValue?: () => void
  fieldName?: string
  oBody?: any[]
  oData?: any
  cellInfo?: any
  multiFields?: string[]
}

const props = withDefaults(defineProps<Props>(), {
  isCustom: false,
  isEditing: true,
  value: null,
  onValueChanged: () => {},
  setUploadStatus: () => {},
  setMultiFieldsValue: () => {},
  fieldName: '',
  oBody: () => [],
  oData: null,
  cellInfo: null,
  multiFields: () => []
})

// ============================================
// 使用共用方法
// ============================================
const {
  isNullOrEmpty,
  getFieldFormat,
  downloadFileByUrl,
  openFileWithGoogleDoc
} = useControlBase()

// ============================================
// Computed
// ============================================
const isForm = computed(() => {
  return props.oData != null && props.cellInfo == null
})

const uploadUrl = computed(() => {
  return `${appInfo.serverUrl || ''}/api/Func/FileUpload?UploadPath=${uploadPath.value}`
})

const logoUrl = computed(() => {
  return `${appInfo.serverUrl || ''}/logo.png`
})

// ============================================
// 狀態變數
// ============================================
const fileUrl = ref<string | undefined>(undefined)
const fileName = ref<string | undefined>(undefined)
const orgFileUrl = ref<string | null>(null)
const orgFileName = ref('')
const showStyle = ref('')
const uploadPath = ref('/upload')
const accept = ref('*')
const allowFileExtensionArray = ref<string[]>([])
const allowFileExtension = ref('')
const maxFileSize = ref(52428800)
const imageStyle = ref('width:80px;height:80px')
const isPicPopup = ref(false)
const focusIn = ref('')
const fileDisplayName = ref('')
const isDeleteServerFile = ref(true)
const fileIconStyle = ref('font-size: 4em')
const cssPOHStyle = ref(
  'overflow:hidden;white-space:nowarp;text-overflow:ellipsis;cursor:pointer;text-decoration: underline;'
)
const otherServerUrl = ref('')
const hinttrash = ref('刪除')

const uploadHeaders = computed(() => ({
  Authorization: `Bearer ${auth.getToken()}`
}))

// ============================================
// 方法
// ============================================
const onUploadAborted = () => {
  props.setUploadStatus?.('0', props.fieldName)
}

const onUploaded = (e: any) => {
  props.setUploadStatus?.('0', props.fieldName)
  const arrFileName = e.file.name.split('.')
  
  fileUrl.value = orgFileUrl.value = e.request.responseText
  props.onValueChanged?.(fileUrl.value, props.fieldName)
  fileName.value = e.file.name

  if (!isNullOrEmpty(fileDisplayName.value)) {
    const fn = isNullOrEmpty(props.value) ? e.file.name : props.value
    const fileSubName = fn.split('.')[fn.split('.').length - 1]
    fileName.value = `${fileDisplayName.value}.${fileSubName.toLowerCase()}`
  } else {
    fileName.value = e.file.name
  }

  if (props.multiFields.length > 0) {
    if (isForm.value && props.oData) {
      if (props.multiFields.length >= 1)
        props.oData[props.multiFields[0]] = e.file.name
      if (props.multiFields.length >= 2)
        props.oData[props.multiFields[1]] = arrFileName[arrFileName.length - 1]
      if (props.multiFields.length >= 3)
        props.oData[props.multiFields[2]] = e.file.size
    } else if (props.cellInfo) {
      // TODO: 實作 Grid 模式下的 cellValue 更新
      // props.cellInfo.component.cellValue(...)
    }
  }

  fileUrl.value = `${appInfo.serverUrl}${fileUrl.value}`

  // TODO: 實作 FocusIn 邏輯
  // if (!isNullOrEmpty(focusIn.value) && isForm.value) {
  //   eval(focusIn.value)
  // }
}

const onDelete = () => {
  fileUrl.value = undefined
  fileName.value = undefined

  if (isForm.value && props.oData) {
    if (props.multiFields.length >= 1) props.oData[props.multiFields[0]] = null
    if (props.multiFields.length >= 2) props.oData[props.multiFields[1]] = null
    if (props.multiFields.length >= 3) props.oData[props.multiFields[2]] = null
  } else if (props.cellInfo) {
    // TODO: 實作 Grid 模式下的 cellValue 更新
  }

  props.onValueChanged?.(fileUrl.value, props.fieldName)
  
  if (isDeleteServerFile.value && orgFileUrl.value) {
    apiDeleteFile(orgFileUrl.value).catch((err) => {
      console.error('Delete file error:', err)
    })
  }
  
  orgFileUrl.value = null
  fileUrl.value = undefined
}

const onUploadStarted = () => {
  props.setUploadStatus?.('1', props.fieldName)
}

const onFileClick = () => {
  if (appInfo.isCordova) {
    if (openFileWithGoogleDoc(fileUrl.value || '')) {
      const viewerUrl =
        'https://docs.google.com/viewer?url=' +
        encodeURIComponent((fileUrl.value || '').toLowerCase()) +
        '&embedded=true'
      ;(window as any).cordova.InAppBrowser.open(
        viewerUrl,
        '_blank',
        'location=no,toolbar=yes,hardwareback=yes'
      )
    } else {
      const viewerUrl = `${appInfo.serverUrl}/pdfjs2023/web/viewer.html?file=${fileUrl.value}`
      ;(window as any).cordova.InAppBrowser.open(
        viewerUrl,
        '_blank',
        'location=no,toolbar=yes,hardwareback=yes'
      )
    }
  } else {
    if (!isNullOrEmpty(otherServerUrl.value)) {
      window.open(fileUrl.value || '', '_blank')
    } else {
      downloadFileByUrl(fileUrl.value || '', fileName.value || '')
    }
  }
}

const onHyperlinkClick = () => {
  if (appInfo.isCordova) {
    if (openFileWithGoogleDoc(fileUrl.value || '')) {
      const viewerUrl =
        'http://docs.google.com/viewer?url=' +
        encodeURIComponent((fileUrl.value || '').toLowerCase()) +
        '&embedded=true'
      ;(window as any).cordova.InAppBrowser.open(
        viewerUrl,
        '_blank',
        'location=no,toolbar=yes,hardwareback=yes'
      )
    } else if ((fileUrl.value || '').toLowerCase().includes('.pdf')) {
      const viewerUrl = `${appInfo.serverUrl}/pdfjs2023/web/viewer.html?file=${fileUrl.value}`
      ;(window as any).cordova.InAppBrowser.open(
        viewerUrl,
        '_blank',
        'location=no,toolbar=yes,hardwareback=yes'
      )
    } else {
      ;(window as any).cordova.InAppBrowser.open(
        fileUrl.value || '',
        '_blank',
        'location=no,toolbar=yes,hardwareback=yes'
      )
    }
  } else {
    const link = document.createElement('a')
    link.setAttribute('href', fileUrl.value || '')
    link.setAttribute('target', '_blank')
    link.style.display = 'none'
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
  }
}

const onPicClick = () => {
  if (!isNullOrEmpty(fileUrl.value)) {
    if (appInfo.isCordova) {
      ;(window as any).cordova.InAppBrowser.open(
        fileUrl.value || '',
        '_blank',
        'location=no,toolbar=yes,hardwareback=yes'
      )
    } else {
      isPicPopup.value = true
    }
  }
}

const onClosePicPopup = () => {
  isPicPopup.value = false
}

const onPicPopupHidden = () => {
  isPicPopup.value = false
}

const onFTPFileClick = async () => {
  const realFileName = (fileName.value || '').split('/').pop() || ''
  try {
    const res = await apiPost('/api/func/GetFTPFile', {
      FileName: fileName.value,
      UserGuid: appInfo.userInfo.userGuid,
    })
    if (res.status === 200) {
      // TODO: 實作 downloadNetworkFile
      // downloadNetworkFile(realFileName, res.data)
    }
  } catch (err) {
    console.error('GetFTPFile error:', err)
  }
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  const bodyRow = props.oBody.find((x) => x.FName == props.fieldName)
  if (!bodyRow) return

  focusIn.value = bodyRow.FocusIn || ''
  fileDisplayName.value = bodyRow.UDF05 || ''

  if (!isNullOrEmpty(bodyRow.UDF07)) {
    isDeleteServerFile.value = bodyRow.UDF07 == '1'
  } else {
    isDeleteServerFile.value = false
  }

  const oformat = getFieldFormat(bodyRow.Format)
  uploadPath.value = oformat.uploadPath == undefined ? '/upload/' : oformat.uploadPath

  allowFileExtension.value = isNullOrEmpty(bodyRow.UDF01) ? '' : bodyRow.UDF01.trim()
  maxFileSize.value = isNullOrEmpty(bodyRow.UDF02)
    ? maxFileSize.value
    : parseInt(bodyRow.UDF02)

  imageStyle.value = bodyRow.UDF03 || imageStyle.value

  if (props.multiFields.length != 0) {
    fileName.value = orgFileName.value =
      props.oData == null
        ? props.cellInfo?.data?.[props.multiFields[0]] || ''
        : props.oData[props.multiFields[0]] || ''
    showStyle.value = '2'
  } else {
    if (bodyRow.FControl?.toUpperCase() == 'PIC') showStyle.value = '1'
    else if (bodyRow.FControl?.toUpperCase() == 'VDO') showStyle.value = '4'
    else {
      if (isNullOrEmpty(fileDisplayName.value) && bodyRow.UDF06 == '1') {
        showStyle.value = '6'
        fileName.value = props.value || ''
      } else if (isNullOrEmpty(fileDisplayName.value) && bodyRow.UDF06 != '1') {
        showStyle.value = '3'
      } else {
        showStyle.value = '2'
        if (!isNullOrEmpty(props.value)) {
          const fileSubName = (props.value || '').split('.')[(props.value || '').split('.').length - 1]
          fileName.value = `${fileDisplayName.value}.${fileSubName.toLowerCase()}`
        }
      }
    }
  }

  otherServerUrl.value = bodyRow.UDF04 || ''

  switch (showStyle.value) {
    case '1':
      accept.value = 'image/*'
      if (isNullOrEmpty(allowFileExtension.value)) {
        allowFileExtension.value = '.jpg,.jpeg,.gif,.png'
      } else {
        allowFileExtension.value = allowFileExtension.value.replace(';', ',')
      }
      break
    case '2':
    case '3':
      if (isNullOrEmpty(allowFileExtension.value)) accept.value = '*'
      else accept.value = allowFileExtension.value.replace(/;/g, ',')
      if (!isNullOrEmpty(allowFileExtension.value)) {
        allowFileExtension.value = allowFileExtension.value.replace(/;/g, ',')
      }
      break
    case '4':
      accept.value = 'video/*'
      if (isNullOrEmpty(allowFileExtension.value)) {
        allowFileExtension.value = '.mp4,.wmv'
      } else {
        allowFileExtension.value = accept.value.replace(';', ',')
      }
      break
  }

  allowFileExtensionArray.value = isNullOrEmpty(allowFileExtension.value)
    ? []
    : allowFileExtension.value.split(',')

  orgFileUrl.value = props.value

  fileUrl.value =
    props.value == null
      ? undefined
      : (props.value || '').indexOf('data:image') > -1
      ? props.value
      : (isNullOrEmpty(otherServerUrl.value)
          ? appInfo.serverUrl
          : otherServerUrl.value) + props.value
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

<style lang="scss" scoped>
.image-container {
  .user-image {
    width: 100%;
    height: 100%;
    background-size: cover;
    background-position: center;
    cursor: pointer;
  }
}
</style>

