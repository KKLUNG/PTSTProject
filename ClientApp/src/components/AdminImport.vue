<template>
  <div>
    <DxLoadPanel
      :hide-on-outside-click="false"
      :visible="loading"
      :shading="true"
      :show-pane="true"
      shading-color="transparent"
      :message="loadingMessage"
    />
    
    <DxPopup
      ref="debugPopup"
      width="100%"
      height="100%"
      title="XMLEditor"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :visible="isDebugPopupVisible"
      :full-screen="appInfo.isMobile"
      @hidden="onDebugPopupHidden"
    >
      <template #title>
        <div class="toolbarArea" style="padding: 5px">
          <div class="leftToolbar">
            <img :src="logoUrl" width="120px" />
          </div>
          <div class="rightToolbar">
            <DxButton icon="close" @click="onCloseDebugPopup" />
          </div>
        </div>
      </template>

      <DxScrollView
        width="100%"
        height="100%"
        :bounce-enabled="false"
        :use-native="appInfo.isMobile ? false : appInfo.isNativeScrollBar"
        show-scrollbar="always"
      >
        <div>
          <!-- TODO: 需要實作 CMSFrame 組件 -->
          <div v-if="isDebugPopup" style="padding: 20px;">
            <p>CMSFrame 組件尚未實作</p>
            <p>XML: {{ debugXML }}</p>
          </div>
        </div>
      </DxScrollView>
    </DxPopup>

    <div>
      <div
        :class="appInfo.isMobile ? 'webPartHeaderTransparent' : 'webPartHeader'"
        v-if="isNullOrEmpty(oHead.GVHeader)"
      >
        <div class="webPartCaption">
          <div><i :class="toolbarClass" /></div>
          <div
            v-if="appInfo.isDebug"
            v-html="Caption"
            style="cursor: pointer"
            :title="XMLName"
            @click="onShowDebugPopup"
          ></div>
          <div v-else v-html="Caption"></div>
        </div>

        <div class="webPartButton">
          <DxButton
            v-if="
              (oHead.FormType == '61' && oHead.HasPrint != '1') ||
              oHead.FormType == '60'
            "
            icon="refresh"
            hint="清除"
            type="normal"
            styling-mode="text"
            @click="onUndo"
          />
          <DxButton
            v-if="oHead.FormType == '61' && oHead.HasPrint != '1'"
            icon="upload"
            hint="上傳"
            type="normal"
            styling-mode="text"
            @click="importData"
          />
        </div>
      </div>

      <hr style="border: 1px solid" />
      <div v-if="!isNullOrEmpty(oHead.ContentReady)">
        <DxHtmlEditor
          :read-only="true"
          width="100%"
          height="300px"
          v-model:value="oHead.ContentReady"
        />
      </div>
      <div class="responsive-paddings1">
        <DxFileUploader
          :ref="widgetID"
          select-button-text="Select"
          accept="*"
          :max-file-size="maxSize"
          :multiple="isMultiple"
          :upload-url="uploadUrl"
          :upload-headers="uploadHeaders"
          :upload-mode="uploadMode"
          labelText=""
          @files-uploaded="onFilesUploaded"
          @upload-error="onUploadError"
          @uploaded="onUploaded"
        />
      </div>
      <div v-if="oHead.FormType == '61' && oHead.HasPrint != '1'">
        <div v-for="item in excelSheetName" :key="item.key">
          <DxDataGrid
            v-show="tmpData[item.sheetName] != undefined"
            :data-source="tmpData[item.sheetName]"
            :show-borders="true"
            :column-hiding-enabled="false"
            :allow-column-resizing="true"
            :column-auto-width="true"
            :show-column-lines="true"
          />
          <div style="height: 20px"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxLoadPanel from 'devextreme-vue/load-panel'
import DxPopup from 'devextreme-vue/popup'
import DxButton from 'devextreme-vue/button'
import DxScrollView from 'devextreme-vue/scroll-view'
import DxFileUploader from 'devextreme-vue/file-uploader'
import DxDataGrid from 'devextreme-vue/data-grid'
import DxHtmlEditor from 'devextreme-vue/html-editor'
import appInfo from '@/utils/app-Info'
import { apiPost } from '@/utils/api-util'
import auth from '@/utils/auth'

// ============================================
// Props
// ============================================
interface Props {
  XMLName?: string
  XMLParameter?: string
  XMLCaption?: string
  pageKey?: string
}

const props = withDefaults(defineProps<Props>(), {
  XMLName: '',
  XMLParameter: '',
  XMLCaption: '',
  pageKey: ''
})

// ============================================
// 輔助函數
// ============================================
const getNewGuid = (): string => {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
    const r = (Math.random() * 16) | 0
    const v = c == 'x' ? r : (r & 0x3) | 0x8
    return v.toString(16)
  })
}

// ============================================
// 狀態變數
// ============================================
const debugPopup = ref<InstanceType<typeof DxPopup> | null>(null)
const isDebugPopup = ref(false)
const isDebugPopupVisible = ref(false)
const loading = ref(false)
const loadingMessage = ref('loading...please wait...')
const oHead = ref<any>({})
const oBody = ref<any[]>([])
const maxSize = ref(51200000)
const widgetID = ref(getNewGuid() + 'upload')
const uploadPath = ref('')
const uploadedFiles = ref<string[]>([])
const isMultiple = ref(false)
const tmpData = ref<any>({})
const uploadUniKey = ref('')
const excelSheetName = ref<Array<{ key: string; sheetName: string }>>([])

// ============================================
// Computed
// ============================================
const logoUrl = computed(() => {
  return appInfo.titleLogo || '/logo.png'
})

const Caption = computed(() => {
  // TODO: 實作 getFieldCaption
  return props.XMLCaption || oHead.value.FVHeader || ''
})

const toolbarClass = computed(() => {
  return oHead.value.UCIcon
    ? `icon dx-icon-${oHead.value.UCIcon} toolbarIcon`
    : 'icon dx-icon-bookmark toolbarIcon'
})

const debugXML = computed(() => {
  if (oHead.value.FormType == '60')
    return `/AdminGridForm/3/Code_CMSForm_CustomImport.xml;${props.XMLName}`
  else
    return `/AdminGridForm|AdminGridForm/3|3/Code_CMSForm_ExcelImport.xml;${props.XMLName}|Code_CMSFormField_ExcelImport.xml`
})

const uploadUrl = computed(() => {
  const baseUrl = appInfo.serverUrl || ''
  const apiPath = oHead.value.SelectionXML || 'CMS/AdminImport'
  return `${baseUrl}/api/${apiPath}?XMLName=${props.XMLName}&UserGuid=${appInfo.userInfo.userGuid}&UploadUniKey=${uploadUniKey.value}`
})

const uploadMode = computed(() => {
  return oHead.value.FormType == '60' ? 'useButtons' : 'instantly'
})

const uploadHeaders = computed(() => ({
  Authorization: `Bearer ${auth.getToken()}`
}))

// ============================================
// 方法
// ============================================
const isNullOrEmpty = (value: any): boolean => {
  return value === null || value === undefined || value === ''
}

const onShowDebugPopup = () => {
  isDebugPopupVisible.value = true
  isDebugPopup.value = true
}

const onCloseDebugPopup = () => {
  isDebugPopupVisible.value = false
  isDebugPopup.value = false
}

const onDebugPopupHidden = () => {
  isDebugPopup.value = false
}

const onUploaded = (e: any) => {
  if (!isNullOrEmpty(e.request?.responseText) || e.request?.status == '200') {
    if (oHead.value.FormType == '60') {
      uploadedFiles.value.push(e.request.responseText)
    } else {
      if (oHead.value.HasPrint != '1') {
        tmpData.value = JSON.parse(e.request.responseText)
      } else {
        importData()
      }
    }
  } else {
    alert('no data uploaded!!, please check your source file.')
  }
}

const onFilesUploaded = (e: any) => {
  if (oHead.value.FormType == '60') {
    // TODO: 實作 UpdateCommand eval
    // eval(oHead.value.UpdateCommand)
  }
}

const onUploadError = (e: any) => {
  alert(e.error?.responseText || e.error || 'Upload error')
}

const onUndo = () => {
  uploadedFiles.value = []
  tmpData.value = {}
  // TODO: 重置 FileUploader
}

const importData = async () => {
  loading.value = true
  try {
    const res = await apiPost('/api/cms/AdminImportData', {
      XMLName: props.XMLName,
      UserGuid: appInfo.userInfo.userGuid,
      UserId: appInfo.userInfo.userId,
      UploadUniKey: uploadUniKey.value,
      XMLParameter: props.XMLParameter,
    })

    if (res.status === 200) {
      // TODO: 實作 showNotify
      // showNotify('sys_updSuccess', 'success', appInfo.notifyDelay)

      if (!isNullOrEmpty(oHead.value.UpdateCommand)) {
        let cmd = oHead.value.UpdateCommand
        cmd = cmd
          .replaceAll('{UploadUniKey}', uploadUniKey.value)
          .replaceAll('{XMLName}', props.XMLName)
          .replaceAll('{UserGuid}', appInfo.userInfo.userGuid || '')
        // TODO: 實作 eval 或改用安全的方式執行命令
        // eval(cmd)
      }
    }
  } catch (err) {
    console.error(err)
    alert(JSON.stringify(err))
  } finally {
    loading.value = false
  }
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  uploadUniKey.value = getNewGuid()
  loading.value = true

  // TODO: 實作 funcAll 方法來載入表單定義
  // Promise.all([
  //   funcAll(props.XMLName, keyParameter, parentParameter, props.XMLParameter)
  // ]).then((res) => {
  //   oHead.value = JSON.parse(res[0].data.head)[0]
  //   oBody.value = JSON.parse(res[0].data.body)
  //   uploadPath.value = oHead.value.InsertCommand
  //   isMultiple.value = oHead.value.FormType == '60' ? oHead.value.HasInsert == '1' : false
  //
  //   if (oHead.value.FormType == '61') {
  //     const ary = oHead.value.DeleteCommand.split('^')
  //     for (let i = 0; i < ary.length; i++) {
  //       excelSheetName.value.push({
  //         key: getNewGuid(),
  //         sheetName: ary[i]
  //       })
  //     }
  //   }
  //   loading.value = false
  // })

  // 暫時設定預設值
  loading.value = false
})
</script>

<style lang="scss" scoped>
</style>

