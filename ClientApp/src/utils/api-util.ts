/**
 * API 工具函數
 * 統一管理所有 API 呼叫
 * 從 Bia 專案移植並轉換為 TypeScript
 */

import axios, { AxiosRequestConfig, AxiosResponse, AxiosError } from 'axios'
import auth from './auth'
import appInfo from './app-Info'

// ============================================
// 型別定義
// ============================================

/**
 * API 參數介面
 */
export interface ApiParams {
  [key: string]: any
  XMLName?: string
  CommandName?: string
}

/**
 * API 回應型別
 */
export type ApiResponse<T = any> = AxiosResponse<T>

/**
 * 錯誤訊息格式化
 */
function formatErrorMessage(params: ApiParams, err: any): string {
  const commandName = params.XMLName || params.CommandName || ''
  const errorData = err?.response?.data || ''
  return `${commandName} execute failed!!<br />Error message: ${errorData}`
}

// ============================================
// API GET 請求
// ============================================

/**
 * 發送 GET 請求
 * @param apiUrl API 路徑
 * @param params 查詢參數
 * @returns Promise<AxiosResponse>
 * 
 * @example
 * ```typescript
 * apiGet('/api/user/list', { page: 1, size: 10 })
 *   .then(res => console.log(res.data))
 *   .catch(err => console.error(err))
 * ```
 */
export function apiGet<T = any>(
  apiUrl: string, 
  params?: ApiParams
): Promise<ApiResponse<T>> {
  return new Promise((resolve, reject) => {
    console.log('🔍 Debug apiGet - URL:', apiUrl, 'Token:', auth.getToken());
    const config: AxiosRequestConfig = {
      method: 'GET',
      baseURL: appInfo.serverUrl ?? undefined,
      url: apiUrl,
      responseType: 'json',
      responseEncoding: 'utf8',
      params: params,
      headers: {
        'Access-Control-Allow-Origin': '*',
        'Authorization': 'Bearer ' + auth.getToken(),
        'Content-Type': 'application/json; charset=utf-8',
      }
    }

    axios(config)
      .then((res) => {
        resolve(res)
      })
      .catch((err: AxiosError) => {
        console.error('API GET Error:', err)
        
        if (err.message === 'Network Error') {
          reject('Network Error: Please check your internet connection.')
        } else {
          const errorMessage = formatErrorMessage(params || {}, err)
          reject(errorMessage)
        }
      })
  })
}

// ============================================
// API POST 請求
// ============================================

/**
 * 發送 POST 請求
 * @param apiUrl API 路徑
 * @param params 請求資料
 * @returns Promise<AxiosResponse>
 * 
 * @example
 * ```typescript
 * apiPost('/api/auth/login', { userId: 'admin', password: '123' })
 *   .then(res => console.log(res.data))
 *   .catch(err => console.error(err))
 * ```
 */
export function apiPost<T = any>(
  apiUrl: string, 
  params?: ApiParams
): Promise<ApiResponse<T>> {
  return new Promise((resolve, reject) => {
    const config: AxiosRequestConfig = {
      method: 'POST',
      baseURL: appInfo.serverUrl ?? undefined,
      url: apiUrl,
      responseType: 'json',
      responseEncoding: 'utf8',
      data: params,
      headers: {
        'Access-Control-Allow-Origin': '*',
        'Authorization': 'Bearer ' + auth.getToken(),
        'Content-Type': 'application/json; charset=utf-8',
      }
    }

    axios(config)
      .then((res) => {
        resolve(res)
      })
      .catch((err: AxiosError) => {
        console.error('API POST Error:', err)
        
        if (err.message === 'Network Error') {
          reject('Network Error: Please check your internet connection.')
        } else {
          const errorMessage = formatErrorMessage(params || {}, err)
          reject(errorMessage)
        }
      })
  })
}

// ============================================
// API SSO 請求 (單點登入)
// ============================================

/**
 * 發送 SSO 請求 (不帶 baseURL 和 Authorization)
 * 用於跨系統單點登入
 * @param apiUrl 完整的 API URL
 * @param params 請求資料
 * @returns Promise<AxiosResponse>
 * 
 * @example
 * ```typescript
 * apiSSO('https://other-system.com/api/sso/login', { token: 'xxx' })
 *   .then(res => console.log(res.data))
 *   .catch(err => console.error(err))
 * ```
 */
export function apiSSO<T = any>(
  apiUrl: string, 
  params?: ApiParams
): Promise<ApiResponse<T>> {
  return new Promise((resolve, reject) => {
    const config: AxiosRequestConfig = {
      method: 'POST',
      url: apiUrl,
      responseType: 'json',
      responseEncoding: 'utf8',
      data: params,
      headers: {
        'Content-Type': 'application/json; charset=utf-8',
      }
    }

    axios(config)
      .then((res) => {
        resolve(res)
      })
      .catch((err: AxiosError) => {
        console.error('API SSO Error:', err)
        reject(err)
      })
  })
}

// ============================================
// 檔案相關 API
// ============================================

/**
 * 上傳檔案
 * @param apiUrl API 路徑
 * @param params 包含檔案的 FormData
 * @returns Promise<AxiosResponse>
 * 
 * @example
 * ```typescript
 * const formData = new FormData()
 * formData.append('file', file)
 * apiFile('/api/func/FileUpload', formData)
 *   .then(res => console.log(res.data))
 *   .catch(err => console.error(err))
 * ```
 */
export function apiFile<T = any>(
  apiUrl: string, 
  params: FormData | ApiParams
): Promise<ApiResponse<T>> {
  return new Promise((resolve, reject) => {
    const config: AxiosRequestConfig = {
      method: 'POST',
      baseURL: appInfo.serverUrl ?? undefined,
      url: apiUrl,
      data: params,
      headers: {
        'Authorization': 'Bearer ' + auth.getToken(),
        // Content-Type 會自動設定為 multipart/form-data
      }
    }

    axios(config)
      .then((res) => {
        resolve(res)
      })
      .catch((err: AxiosError) => {
        console.error('API File Error:', err)
        reject(err)
      })
  })
}

/**
 * 刪除檔案
 * @param fileUrl 檔案路徑
 * @returns Promise<AxiosResponse>
 * 
 * @example
 * ```typescript
 * apiDeleteFile('/upload/file.pdf')
 *   .then(res => console.log('File deleted'))
 *   .catch(err => console.error(err))
 * ```
 */
export function apiDeleteFile<T = any>(
  fileUrl: string
): Promise<ApiResponse<T>> {
  const params = {
    FilePath: fileUrl
  }

  return new Promise((resolve, reject) => {
    const config: AxiosRequestConfig = {
      method: 'POST',
      baseURL: appInfo.serverUrl ?? undefined,
      url: '/api/func/FileDelete',
      data: params,
      headers: {
        'Authorization': 'Bearer ' + auth.getToken(),
        'Content-Type': 'application/json; charset=utf-8',
      }
    }

    axios(config)
      .then((res) => {
        resolve(res)
      })
      .catch((err: AxiosError) => {
        console.error('API Delete File Error:', err)
        reject(err)
      })
  })
}

/**
 * 下載檔案 (Blob 格式)
 * 自動觸發瀏覽器下載
 * @param fileUrl 檔案 URL
 * @param fileName 儲存的檔案名稱
 * @returns Promise<void>
 * 
 * @example
 * ```typescript
 * apiGetBlobFile('/upload/report.pdf', 'report.pdf')
 *   .then(() => console.log('Download started'))
 *   .catch(err => console.error(err))
 * ```
 */
export function apiGetBlobFile(
  fileUrl: string, 
  fileName: string
): Promise<void> {
  return new Promise((resolve, reject) => {
    const config: AxiosRequestConfig = {
      url: fileUrl,
      method: 'GET',
      responseType: 'blob',
      headers: {
        'Authorization': 'Bearer ' + auth.getToken(),
      }
    }

    axios(config)
      .then((response) => {
        // 建立 Blob URL
        const url = window.URL.createObjectURL(new Blob([response.data]))
        
        // 建立下載連結
        const link = document.createElement('a')
        link.href = url
        link.setAttribute('download', fileName)
        
        // 觸發下載
        document.body.appendChild(link)
        link.click()
        
        // 清理
        document.body.removeChild(link)
        window.URL.revokeObjectURL(url)
        
        resolve()
      })
      .catch((err: AxiosError) => {
        console.error('API Get Blob File Error:', err)
        reject(err)
      })
  })
}

// ============================================
// 工具函數
// ============================================

/**
 * 動態載入外部 JavaScript 檔案
 * @param src JavaScript 檔案 URL
 * @returns Promise<void>
 * 
 * @example
 * ```typescript
 * loadExtJs('https://cdn.example.com/library.js')
 *   .then(() => console.log('Script loaded'))
 *   .catch(() => console.error('Script load failed'))
 * ```
 */
export function loadExtJs(src: string): Promise<void> {
  return new Promise((resolve, reject) => {
    const script = document.createElement('script')
    script.type = 'text/javascript'
    script.src = src
    
    script.onload = () => {
      resolve()
    }
    
    script.onerror = () => {
      reject(new Error(`Failed to load script: ${src}`))
    }
    
    document.body.appendChild(script)
  })
}

// ============================================
// 預設匯出 (保持與原始檔案相容)
// ============================================

export default {
  apiGet,
  apiPost,
  apiSSO,
  apiFile,
  apiDeleteFile,
  apiGetBlobFile,
  loadExtJs
}

