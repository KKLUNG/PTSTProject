import 'devextreme/dist/css/dx.common.css'
import './themes/generated/theme.base.css'
import './themes/generated/theme.additional.css'

// ============================================
// Vue 3 核心
// ============================================
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'

// ============================================
// DevExtreme 配置
// ============================================
import config from 'devextreme/core/config'
import { locale, loadMessages } from 'devextreme/localization'
import { alert, confirm, custom } from 'devextreme/ui/dialog'
import notify from 'devextreme/ui/notify'

// 設定 DevExtreme
config({
    // licenseKey: 'YOUR_LICENSE_KEY_HERE', // 如果有授權，在這裡設定
    editorStylingMode: 'underlined', // 或 'outlined' | 'filled'
})

// ============================================
// 工具函數與常數
// ============================================
import appInfo from '@/utils/app-Info'
import { apiGet, apiPost, apiSSO, apiFile, apiDeleteFile, apiGetBlobFile } from '@/utils/api-util'
import * as debug from './debug'

// ============================================
// 型別定義
// ============================================
interface MemoryStorage {
  [key: string]: any
}

// ============================================
// 建立 Vue 應用實例
// ============================================
const app = createApp(App)

// ============================================
// 使用插件
// ============================================
app.use(router)
app.use(store)
// app.use(vueEsign) // 需要先安裝: npm install vue-esign

// ============================================
// 全域屬性設定 (Vue 3 方式)
// ============================================
app.config.globalProperties.debug = debug
app.config.globalProperties.$appInfo = appInfo
app.config.globalProperties.apiGet = apiGet
app.config.globalProperties.apiPost = apiPost
app.config.globalProperties.apiFile = apiFile
app.config.globalProperties.apiDeleteFile = apiDeleteFile
app.config.globalProperties.apiSSO = apiSSO
app.config.globalProperties.apiGetBlobFile = apiGetBlobFile

// 自訂 alert 函數
app.config.globalProperties.alertThen = alert
app.config.globalProperties.alert = (
  message: string,
  title?: string,
  buttonText?: string,
  f?: Function
) => {
  custom({
    title: title || '',
    messageHtml: message,
    buttons: [
      {
        text: buttonText || 'OK',
        onClick: (e: any) => {
          return { buttonText: e.component.option('text') }
        },
      },
    ],
  })
    .show()
    .then(f)
}

app.config.globalProperties.confirm = confirm
app.config.globalProperties.notify = notify

// Memory Storage
app.config.globalProperties.$ms = {} as MemoryStorage
app.config.globalProperties.$footerTabs = []
app.config.globalProperties.$speechBot = window.speechSynthesis

// ============================================
// 設定 DevExtreme 語系
// ============================================
// TODO: 載入語系檔案
// import zhMessages from './dxMessage_zhTW.json'
// loadMessages(zhMessages)
locale('zh-TW')

// ============================================
// 掛載應用
// ============================================
app.mount('#app')

console.log('✅ PTSDProject 應用程式已啟動')
console.log('📦 Vue 版本:', app.version)