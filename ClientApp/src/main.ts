import 'devextreme/dist/css/dx.common.css'
import './themes/generated/theme.base.css'
import './themes/generated/theme.additional.css'

// ============================================
// DevExtreme Themes 初始化（必須在 Vue 之前）
// ============================================
import themes from 'devextreme/ui/themes'

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
// Themes 和 CSS 變數
// ============================================
// 在 Vite 中，使用 .module.scss 才能正確處理 :export
import cssVariable from '@/css/scssToFunction.module.scss'

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
app.config.globalProperties.$cssVariable = cssVariable
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
// 註冊控件組件
// ============================================
// 基礎輸入控件
import CHK from '@/controls/CHK.vue'
import TX2 from '@/controls/TX2.vue'
import TXC from '@/controls/TXC.vue'
import TTT from '@/controls/TTT.vue'
import SWI from '@/controls/SWI.vue'
import RBO from '@/controls/RBO.vue'
import CHT from '@/controls/CHT.vue'
import TIM from '@/controls/TIM.vue'
import DA2 from '@/controls/DA2.vue'
import DT2 from '@/controls/DT2.vue'
import CHO from '@/controls/CHO.vue'
import CHH from '@/controls/CHH.vue'
import TI2 from '@/controls/TI2.vue'
import DAM from '@/controls/DAM.vue'
import DM2 from '@/controls/DM2.vue'
import DD2 from '@/controls/DD2.vue'
import DDT from '@/controls/DDT.vue'
import LBL from '@/controls/LBL.vue'
import LBH from '@/controls/LBH.vue'
import LBT from '@/controls/LBT.vue'
import LBD from '@/controls/LBD.vue'
import LNG from '@/controls/LNG.vue'
import CBM from '@/controls/CBM.vue'
import CBT from '@/controls/CBT.vue'
import CBP from '@/controls/CBP.vue'
import CBG from '@/controls/CBG.vue'
// 表單/容器控件
import FRM from '@/controls/FRM.vue'
import TAB from '@/controls/TAB.vue'
// 數據展示控件
import GRD from '@/controls/GRD.vue'
// 文件/媒體控件
import FIL from '@/controls/FIL.vue'
import FIM from '@/controls/FIM.vue'
import HDD from '@/controls/HDD.vue'
import HTM from '@/controls/HTM.vue'
// 彈窗控件
import POG from '@/controls/POG.vue'
// TODO: 繼續註冊其他控件 (P2 優先級)
// ... 其他控件

// 註冊基礎控件
app.component('CHK', CHK)
app.component('TX2', TX2)
app.component('TXC', TXC)
app.component('TTT', TTT)
app.component('SWI', SWI)
app.component('RBO', RBO)
app.component('CHT', CHT)
app.component('TIM', TIM)
app.component('DA2', DA2)
app.component('DT2', DT2)
app.component('CHO', CHO)
app.component('CHH', CHH)
app.component('TI2', TI2)
app.component('DAM', DAM)
app.component('DM2', DM2)
app.component('DD2', DD2)
app.component('DDT', DDT)
app.component('LBL', LBL)
app.component('LBH', LBH)
app.component('LBT', LBT)
app.component('LBD', LBD)
app.component('LNG', LNG)
app.component('CBM', CBM)
app.component('CBT', CBT)
app.component('CBP', CBP)
app.component('CBG', CBG)
// 表單/容器控件
app.component('FRM', FRM)
app.component('TAB', TAB)
// 數據展示控件
app.component('GRD', GRD)
// 文件/媒體控件
app.component('FIL', FIL)
app.component('FIM', FIM)
app.component('HDD', HDD)
app.component('HTM', HTM)
// 彈窗控件
app.component('POG', POG)
// TODO: 繼續註冊其他控件 (P2 優先級)
// ... 其他控件

// ============================================
// 註冊 Admin 組件（用於 CMSPage 動態載入）
// ============================================
import AdminFormView from '@/components/AdminFormView.vue'
import AdminGridForm from '@/components/AdminGridForm.vue'
import AdminTab from '@/components/AdminTab.vue'
import AdminFrame from '@/components/AdminFrame.vue'
import AdminTreeList from '@/components/AdminTreeList.vue'
import AdminCalendar from '@/components/AdminCalendar.vue'
import AdminChart from '@/components/AdminChart.vue'
import AdminPivot from '@/components/AdminPivot.vue'
import AdminGantt from '@/components/AdminGantt.vue'
import AdminTileView from '@/components/AdminTileView.vue'
import AdminImport from '@/components/AdminImport.vue'
import AdminFullTextSearch from '@/components/AdminFullTextSearch.vue'
import AdminCustom from '@/components/AdminCustom.vue'
import AdminSSO from '@/components/AdminSSO.vue'
import AdminWFSForm from '@/components/AdminWFSForm.vue'

app.component('AdminFormView', AdminFormView)
app.component('AdminGridForm', AdminGridForm)
app.component('AdminTab', AdminTab)
app.component('AdminFrame', AdminFrame)
app.component('AdminTreeList', AdminTreeList)
app.component('AdminCalendar', AdminCalendar)
app.component('AdminChart', AdminChart)
app.component('AdminPivot', AdminPivot)
app.component('AdminGantt', AdminGantt)
app.component('AdminTileView', AdminTileView)
app.component('AdminImport', AdminImport)
app.component('AdminFullTextSearch', AdminFullTextSearch)
app.component('AdminCustom', AdminCustom)
app.component('AdminSSO', AdminSSO)
app.component('AdminWFSForm', AdminWFSForm)

// ============================================
// 設定 DevExtreme 語系
// ============================================
// TODO: 載入語系檔案
// import zhMessages from './dxMessage_zhTW.json'
// loadMessages(zhMessages)
locale('zh-TW')

// ============================================
// 使用 themes.initialized() 確保 DevExtreme 樣式正確載入
// ============================================
themes.initialized(() => {
  // ============================================
  // 掛載應用
  // ============================================
  app.mount('#app')

  console.log('✅ PTSDProject 應用程式已啟動')
  console.log('📦 Vue 版本:', app.version)
  console.log('🎨 DevExtreme Themes 已初始化')
})