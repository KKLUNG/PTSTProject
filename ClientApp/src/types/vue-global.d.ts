// Vue 3 全域屬性型別定義

import type { ComponentCustomProperties } from 'vue'
import type { Router } from 'vue-router'

// 定義記憶體儲存介面
export interface MemoryStorage {
  [key: string]: any
}

// 定義應用資訊介面 (使用寬鬆型別)
export type AppInfo = Record<string, any>

// 定義 API 函數型別
export type ApiFunction = (...args: any[]) => Promise<any>

// 擴充 Vue 元件實例屬性
declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    // Debug
    debug: any
    
    // 應用資訊
    $appInfo: AppInfo
    
    // API 函數
    apiGet: ApiFunction
    apiPost: ApiFunction
    apiSSO: ApiFunction
    apiFile: ApiFunction
    apiDeleteFile: ApiFunction
    apiGetBlobFile: ApiFunction
    
    // DevExtreme 對話框
    alert: (message: string, title?: string, buttonText?: string, callback?: Function) => void
    alertThen: any
    confirm: any
    notify: any
    
    // Memory Storage
    $ms: MemoryStorage
    $footerTabs: any[]
    
    // 語音合成
    $speechBot: SpeechSynthesis
    
    // CSS 變數
    $cssVariable: any
    
    // Event Bus
    $bus: any
  }
}

export {}

