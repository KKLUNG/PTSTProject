import { getCurrentInstance } from 'vue'

/**
 * 控件基礎共用方法 Composable
 * 提供所有控件共用的工具函數
 */
export function useControlBase() {
  const instance = getCurrentInstance()
  const appContext = instance?.appContext
  const appInfo = appContext?.config.globalProperties.$appInfo
  const cssVariable = appContext?.config.globalProperties.$cssVariable || {}

  /**
   * 檢查是否為空值
   */
  const isNullOrEmpty = (str: any): boolean => {
    if (str === '' || str == null || str == 'null' || str == 'undefined' || str == undefined)
      return true
    else
      return false
  }

  /**
   * 檢查是否為日期物件
   */
  const isDateObject = (value: any): boolean => {
    return Object.prototype.toString.call(value) === '[object Date]'
  }

  /**
   * 日期轉字串
   */
  const dateToString = (d: Date | null, format?: string): string => {
    if (!d || !isDateObject(d))
      return ""

    if (format == undefined)
      format = 'yyyy/MM/dd'

    if (format.indexOf('yyyy') >= 0) {
      format = format.replace(/yyyy/g, d.getFullYear().toString())
    }
    if (format.indexOf('yy') >= 0) {
      format = format.replace(/yy/g, d.getFullYear().toString().substr(2, 2))
    }
    if (format.indexOf('MM') >= 0) {
      format = format.replace(/MM/g, parseInt(String(d.getMonth() + 1 + 100)).toString().substr(1, 2))
    }
    if (format.indexOf('M') >= 0) {
      format = format.replace(/M/g, parseInt(String(d.getMonth() + 1)).toString())
    }
    if (format.indexOf('dd') >= 0) {
      format = format.replace(/dd/g, parseInt(String(d.getDate() + 100)).toString().substr(1, 2))
    }
    if (format.indexOf('d') >= 0) {
      format = format.replace(/d/g, parseInt(String(d.getDate())).toString())
    }
    if (format.indexOf('HH') >= 0) {
      format = format.replace(/HH/g, parseInt(String(d.getHours() + 100)).toString().substr(1, 2))
    }
    if (format.indexOf('hh') >= 0) {
      format = format.replace(/hh/g, parseInt(String(d.getHours() + 100)).toString().substr(1, 2))
    }
    if (format.indexOf('H') >= 0) {
      format = format.replace(/H/g, parseInt(String(d.getHours())).toString())
    }
    if (format.indexOf('h') >= 0) {
      format = format.replace(/h/g, parseInt(String(d.getHours())).toString())
    }
    if (format.indexOf('mm') >= 0) {
      format = format.replace(/mm/g, parseInt(String(d.getMinutes() + 100)).toString().substr(1, 2))
    }
    if (format.indexOf('m') >= 0) {
      format = format.replace(/m/g, parseInt(String(d.getMinutes())).toString())
    }
    if (format.indexOf('ss') >= 0) {
      format = format.replace(/ss/g, parseInt(String(d.getSeconds() + 100)).toString().substr(1, 2))
    }
    if (format.indexOf('s') >= 0) {
      format = format.replace(/s/g, parseInt(String(d.getSeconds())).toString())
    }
    return format
  }

  /**
   * 字串轉日期
   */
  const stringToDate = (s: string | null | undefined): Date | null => {
    if (isNullOrEmpty(s))
      return null

    const t = String(s).replace("T", " ").replace(/\//g, "-").split(/[- :]/)
    let d: Date | null = null
    if (t.length > 3)
      d = new Date(parseInt(t[0]), parseInt(t[1]) - 1, parseInt(t[2]), parseInt(t[3] || '0'), parseInt(t[4] || '0'), parseInt(t[5] || '0'))
    else
      d = new Date(parseInt(t[0]), parseInt(t[1]) - 1, parseInt(t[2]))
    return d
  }

  /**
   * 取得欄位格式
   */
  const getFieldFormat = (formatStr: string | null | undefined): any => {
    if (isNullOrEmpty(formatStr))
      return { displayFormat: undefined }

    try {
      return JSON.parse(formatStr)
    } catch {
      return { displayFormat: formatStr }
    }
  }

  /**
   * 產生新的 GUID
   */
  const getNewGuid = (): string => {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
      const r = (Math.random() * 16) | 0
      const v = c == 'x' ? r : (r & 0x3) | 0x8
      return v.toString(16)
    })
  }

  /**
   * 格式化數字（千分位）
   */
  const formatComma = (num: string | number): string => {
    if (isNullOrEmpty(num)) return ''
    const numStr = String(num)
    return numStr.replace(/\B(?=(\d{3})+(?!\d))/g, ',')
  }

  /**
   * 下載檔案
   */
  const downloadFileByUrl = async (url: string, fileName: string): Promise<void> => {
    try {
      // TODO: 實作下載檔案邏輯
      // 可以使用 apiGetBlobFile 或直接建立 <a> 標籤下載
      const link = document.createElement('a')
      link.href = url
      link.download = fileName || 'download'
      link.style.display = 'none'
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
    } catch (err) {
      console.error('Download file error:', err)
    }
  }

  /**
   * 檢查是否可用 Google Doc 開啟
   */
  const openFileWithGoogleDoc = (url: string): boolean => {
    // TODO: 實作檢查邏輯
    const googleDocExtensions = ['.doc', '.docx', '.xls', '.xlsx', '.ppt', '.pptx']
    const ext = url.toLowerCase().substring(url.lastIndexOf('.'))
    return googleDocExtensions.includes(ext)
  }

  /**
   * 取得系統配置
   */
  const getSystemConfig = (key: string): string => {
    // TODO: 實作系統配置獲取邏輯
    return ''
  }

  return {
    isNullOrEmpty,
    isDateObject,
    dateToString,
    stringToDate,
    getFieldFormat,
    getNewGuid,
    formatComma,
    downloadFileByUrl,
    openFileWithGoogleDoc,
    getSystemConfig,
    appInfo,
    cssVariable
  }
}

