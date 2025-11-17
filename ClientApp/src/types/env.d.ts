import { alert, confirm } from 'devextreme/ui/dialog'
import notify from 'devextreme/ui/notify'

// 聲明 Vue 模組支援預設匯出
declare module 'vue' {
  import Vue from 'vue/types/vue'
  export default Vue
}

// 聲明 JavaScript 和其他文件模組
declare module '*.js' {
  const content: any
  export default content
}

declare module '*.ts' {
  const content: any
  export default content
}

declare module '*.scss' {
  const content: any
  export default content
}

declare module 'vue-esign' {
  const content: any
  export default content
}

// 明確宣告專案中的模組
declare module './store' {
  const content: any
  export default content
}

declare module './debug' {
  const content: any
  export default content
}

declare module '@/utils/app-info' {
  const content: any
  export default content
}

declare module '@/utils/api-util' {
  export function apiGet(url: string, params?: any): Promise<any>
  export function apiPost(url: string, data?: any): Promise<any>
  export function apiSSO(url: string, params?: any): Promise<any>
  export function apiFile(url: string, formData: FormData): Promise<any>
  export function apiDeleteFile(url: string, params?: any): Promise<any>
  export function apiGetBlobFile(url: string, params?: any): Promise<Blob>
}

declare module '@/css/scssToFunction.module.scss' {
  const content: {
    baseAccent: string
    baseTextColor: string
    baseBg: string
    baseBorderColor: string
    baseBorderRadius: string
    baseWebPartIcon: string
    baseWebpartHeaderColor: string
    baseWebpartHeaderBackgroundColor: string
    baseGVHeaderColor: string
    baseGvHeaderBackgroundColor: string
    baseHyperLinkColor: string
    baseLoginBackgroundColor: string
    baseFvHeaderColor: string
    baseFvTextEditorColor: string
    [key: string]: string
  }
  export default content
}

declare module '@/types/vue-shims' {
  export interface MemoryStorage {
    [key: string]: any
  }
}

// 定義 Memory Storage 的型別
interface MemoryStorage {
  [key: string]: any
}

// 定義 Footer Tabs 的型別
interface FooterTab {
  // 根據您的實際使用情況定義
  [key: string]: any
}

// 擴展 Vue 實例的型別
declare module 'vue/types/vue' {
  interface Vue {
    // API 方法
    apiGet: (url: string, params?: any) => Promise<any>
    apiPost: (url: string, data?: any) => Promise<any>
    apiFile: (url: string, formData: FormData) => Promise<any>
    apiDeleteFile: (url: string, params?: any) => Promise<any>
    apiSSO: (url: string, params?: any) => Promise<any>
    apiGetBlobFile: (url: string, params?: any) => Promise<Blob>
    
    // 全域屬性
    $appInfo: any // 可以進一步定義 appInfo 的型別
    $cssVariable: any
    $ms: MemoryStorage
    $footerTabs: FooterTab[]
    $bus: Vue
    $speechBot: SpeechSynthesis
    
    // 對話框方法
    alert: (message: string, title?: string, buttonText?: string, callback?: Function) => void
    alertThen: typeof alert
    confirm: typeof confirm
    notify: typeof notify
    
    // Debug
    debug: any
  }
}