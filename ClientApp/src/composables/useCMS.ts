/**
 * CMS API Composable
 * 封裝所有 CMS 相關的 API 呼叫
 */

import { ref } from 'vue'
import { apiGet, apiPost } from '../utils/api-util'

export function useCMS() {
  const loading = ref(false)
  const error = ref<string | null>(null)

  /**
   * 處理 API 回應 (自動解析 JSON 字串)
   */
  const parseResponse = (data: any) => {
    if (typeof data === 'string') {
      try {
        return JSON.parse(data)
      } catch {
        return data
      }
    }
    return data
  }

  /**
   * 通用錯誤處理
   */
  const handleError = (err: any): string => {
    console.error('CMS API Error:', err)
    if (typeof err === 'string') {
      return err
    }
    return '操作失敗，請稍後再試'
  }

  // ============================================
  // 使用者管理
  // ============================================

  /**
   * 取得使用者個人資料
   */
  const getUserProfile = async (userGuid: string) => {
    loading.value = true
    error.value = null
    try {
      const response = await apiGet('/api/CMS/GetUserProfile', { UserGuid: userGuid })
      if (response.status === 200) {
        const data = parseResponse(response.data)
        return Array.isArray(data) ? data[0] : data
      }
      return null
    } catch (err) {
      error.value = handleError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * 取得使用者清單
   */
  const getUserList = async (params: {
    DeptGuid?: string
    Keyword?: string
    PageSize?: number
    PageIndex?: number
  } = {}) => {
    loading.value = true
    error.value = null
    try {
      const response = await apiGet('/api/CMS/GetUserList', {
        DeptGuid: params.DeptGuid || '',
        Keyword: params.Keyword || '',
        PageSize: params.PageSize || 50,
        PageIndex: params.PageIndex || 1
      })
      if (response.status === 200) {
        return parseResponse(response.data)
      }
      return []
    } catch (err) {
      error.value = handleError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * 更新使用者資料
   */
  const updateUserProfile = async (userData: {
    UserGuid: string
    UserName: string
    UserTitle?: string
    UserEmail?: string
    UserCellPhone?: string
  }) => {
    loading.value = true
    error.value = null
    try {
      const response = await apiPost('/api/CMS/UpdateUserProfile', userData)
      if (response.status === 200) {
        return response.data
      }
      return null
    } catch (err) {
      error.value = handleError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  // ============================================
  // 部門管理
  // ============================================

  /**
   * 取得部門樹狀結構
   */
  const getDeptTree = async () => {
    loading.value = true
    error.value = null
    try {
      const response = await apiGet('/api/CMS/GetDeptTree', {})
      if (response.status === 200) {
        const depts = parseResponse(response.data)
        return buildDeptTree(depts)
      }
      return []
    } catch (err) {
      error.value = handleError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * 組裝部門樹狀結構
   */
  const buildDeptTree = (items: any[], parentGuid: string = '00000000-0000-0000-0000-000000000000') => {
    return items
      .filter(item => item.DeptPGuid === parentGuid)
      .map(item => ({
        ...item,
        children: buildDeptTree(items, item.DeptGuid)
      }))
  }

  /**
   * 取得部門成員
   */
  const getDeptMembers = async (deptGuid: string) => {
    loading.value = true
    error.value = null
    try {
      const response = await apiGet('/api/CMS/GetDeptMembers', { DeptGuid: deptGuid })
      if (response.status === 200) {
        return parseResponse(response.data)
      }
      return []
    } catch (err) {
      error.value = handleError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  // ============================================
  // 角色權限
  // ============================================

  /**
   * 取得角色清單
   */
  const getRoles = async () => {
    loading.value = true
    error.value = null
    try {
      const response = await apiGet('/api/CMS/GetRoles', {})
      if (response.status === 200) {
        return parseResponse(response.data)
      }
      return []
    } catch (err) {
      error.value = handleError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * 取得使用者的角色
   */
  const getUserRoles = async (userGuid: string) => {
    loading.value = true
    error.value = null
    try {
      const response = await apiGet('/api/CMS/GetUserRoles', { UserGuid: userGuid })
      if (response.status === 200) {
        return parseResponse(response.data)
      }
      return []
    } catch (err) {
      error.value = handleError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * 分配角色給使用者
   */
  const assignUserRole = async (userGuid: string, roleGuid: string) => {
    loading.value = true
    error.value = null
    try {
      const response = await apiPost('/api/CMS/AssignUserRole', {
        UserGuid: userGuid,
        RoleGuid: roleGuid
      })
      if (response.status === 200) {
        return response.data
      }
      return null
    } catch (err) {
      error.value = handleError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * 移除使用者角色
   */
  const removeUserRole = async (userGuid: string, roleGuid: string) => {
    loading.value = true
    error.value = null
    try {
      const response = await apiPost('/api/CMS/RemoveUserRole', {
        UserGuid: userGuid,
        RoleGuid: roleGuid
      })
      if (response.status === 200) {
        return response.data
      }
      return null
    } catch (err) {
      error.value = handleError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  // ============================================
  // 系統代碼
  // ============================================

  /**
   * 取得系統代碼
   */
  const getCMSCodes = async (codesType: string) => {
    loading.value = true
    error.value = null
    try {
      const response = await apiGet('/api/CMS/GetCMSCodes', { CodesType: codesType })
      if (response.status === 200) {
        return parseResponse(response.data)
      }
      return []
    } catch (err) {
      error.value = handleError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * 取得所有代碼類型
   */
  const getCodesTypes = async () => {
    loading.value = true
    error.value = null
    try {
      const response = await apiGet('/api/CMS/GetCodesTypes', {})
      if (response.status === 200) {
        return parseResponse(response.data)
      }
      return []
    } catch (err) {
      error.value = handleError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  // ============================================
  // 系統日誌
  // ============================================

  /**
   * 取得事件日誌
   */
  const getEventLog = async (params: {
    StartDate?: string
    EndDate?: string
    EventCode?: string
    PageSize?: number
    PageIndex?: number
  } = {}) => {
    loading.value = true
    error.value = null
    try {
      const response = await apiGet('/api/CMS/GetEventLog', {
        StartDate: params.StartDate || '',
        EndDate: params.EndDate || '',
        EventCode: params.EventCode || '',
        PageSize: params.PageSize || 50,
        PageIndex: params.PageIndex || 1
      })
      if (response.status === 200) {
        return parseResponse(response.data)
      }
      return []
    } catch (err) {
      error.value = handleError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * 記錄事件日誌
   */
  const logEvent = async (eventData: {
    EventCode: string
    EventName: string
    Parameter?: string
    CreatedUserID?: string
  }) => {
    loading.value = true
    error.value = null
    try {
      const response = await apiPost('/api/CMS/LogEvent', eventData)
      if (response.status === 200) {
        return response.data
      }
      return null
    } catch (err) {
      error.value = handleError(err)
      throw err
    } finally {
      loading.value = false
    }
  }

  /**
   * 記錄選單存取日誌
   */
  const setMenuLog = async (menuGuid: string, userGuid: string) => {
    try {
      const response = await apiPost('/api/CMS/SetCMSMenuLog', {
        MenuGuid: menuGuid,
        UserGuid: userGuid
      })
      return response.data
    } catch (err) {
      console.error('記錄選單日誌失敗:', err)
      // 選單日誌失敗不應影響主流程，所以不 throw
      return null
    }
  }

  return {
    loading,
    error,
    // 使用者管理
    getUserProfile,
    getUserList,
    updateUserProfile,
    // 部門管理
    getDeptTree,
    getDeptMembers,
    // 角色權限
    getRoles,
    getUserRoles,
    assignUserRole,
    removeUserRole,
    // 系統代碼
    getCMSCodes,
    getCodesTypes,
    // 系統日誌
    getEventLog,
    logEvent,
    setMenuLog
  }
}

