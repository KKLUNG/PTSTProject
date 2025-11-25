<template>
  <div class="container">
    <DxLoadPanel
      :hide-on-outside-click="false"
      :visible="loading"
      :shading="true"
      :show-pane="true"
      shading-color="transparent"
      :message="loadingMessage"
    />
    <DxPopup
      ref="rootPopup"
      width="150px"
      height="150px"
      :title="$appInfo.title"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :visible="hasNewApp"
    >
      <div style="text-align: center">
        <br /><br />
        Download progress <br />{{ downloadProgress }}
      </div>
    </DxPopup>
    <div class="row" v-if="!isLargeScreen && !$appInfo.isMobile">
      <div class="col-12">
        <div class="content-block">
          <span style="font-size: 1.5rem">{{ menuTitle }}</span>
        </div>
      </div>
    </div>
    <div class="row" v-if="oTop.length > 0">
      <div class="col-12">
        <div class="content-block">
          <div class="dx-card" v-for="item in oTop" :key="item.MenusGuid + '1'">
            <component
              :is="item.MenusControl"
              :XMLName="item.MenusXMLName"
              :XMLParameter="item.MenusParameter"
              :XMLCaption="
                getFieldCaption(item.MenusTitle, item.MenusTitleLang)
              "
              :pageKey="pageKey"
            ></component>
          </div>
        </div>
      </div>
    </div>
    <div class="content-block">
      <div class="row">
        <div :class="oLeftCol" v-if="oLeft.length > 0">
          <div class="dx-card" v-for="item in oLeft" :key="item.MenusGuid + '2'">
            <component
              :is="item.MenusControl"
              :XMLName="item.MenusXMLName"
              :XMLParameter="item.MenusParameter"
              :XMLCaption="
                getFieldCaption(item.MenusTitle, item.MenusTitleLang)
              "
              :pageKey="pageKey"
            ></component>
          </div>
        </div>
        <div :class="oCenterCol" v-if="oCenter.length > 0">
          <div class="dx-card" v-for="item in oCenter" :key="item.MenusGuid + '3'">
            <component
              :is="item.MenusControl"
              :XMLName="item.MenusXMLName"
              :XMLParameter="item.MenusParameter"
              :XMLCaption="
                getFieldCaption(item.MenusTitle, item.MenusTitleLang)
              "
              :pageKey="pageKey"
            ></component>
          </div>
        </div>
        <div :class="oRightCol" v-if="oRight.length > 0">
          <div class="dx-card" v-for="item in oRight" :key="item.MenusGuid + '4'">
            <component
              :is="item.MenusControl"
              :XMLName="item.MenusXMLName"
              :XMLParameter="item.MenusParameter"
              :XMLCaption="
                getFieldCaption(item.MenusTitle, item.MenusTitleLang)
              "
              :pageKey="pageKey"
            ></component>
          </div>
        </div>
      </div>
    </div>
    <div class="row" v-if="oBottom.length > 0">
      <div class="col-12">
        <div class="content-block">
          <div class="dx-card" v-for="item in oBottom" :key="item.MenusGuid + '5'">
            <component
              :is="item.MenusControl"
              :XMLName="item.MenusXMLName"
              :XMLParameter="item.MenusParameter"
              :XMLCaption="
                getFieldCaption(item.MenusTitle, item.MenusTitleLang)
              "
              :pageKey="pageKey"
            ></component>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, getCurrentInstance } from 'vue'
import { useRoute } from 'vue-router'
import { sizes } from '../utils/media-query'
import DxLoadPanel from 'devextreme-vue/load-panel'
import DxPopup from 'devextreme-vue/popup'
import { apiGet } from '../utils/api-util'

// ============================================
// Vue 實例和路由
// ============================================
const { appContext } = getCurrentInstance()!
const route = useRoute()

// ============================================
// 全域屬性
// ============================================
const appInfo = appContext.config.globalProperties.$appInfo
const showAlert = appContext.config.globalProperties.alert as (
  message: string, title?: string, buttonText?: string, f?: () => void
) => void

// ============================================
// 狀態變數
// ============================================
const isLargeScreen = ref(sizes()['screen-large'])
const menuTitle = ref('')
const menuGuid = ref('')
const loading = ref(false)
const loadingMessage = ref('Loading....')
const hasNewApp = ref(false)
const downloadProgress = ref(0)
const menuData = ref<any[]>([])
const controlCount = ref(0)

// 位置分組
const oTop = ref<any[]>([])
const oLeft = ref<any[]>([])
const oCenter = ref<any[]>([])
const oRight = ref<any[]>([])
const oBottom = ref<any[]>([])

// 欄位寬度
const oLeftCol = ref('col-4')
const oCenterCol = ref('col-4')
const oRightCol = ref('col-4')

// 生成 pageKey
function getNewGuid(): string {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
    const r = Math.random() * 16 | 0
    const v = c === 'x' ? r : (r & 0x3 | 0x8)
    return v.toString(16)
  })
}

const pageKey = ref(getNewGuid())

// ============================================
// 方法
// ============================================

/**
 * 檢查值是否為空
 */
function isNullOrEmpty(value: any): boolean {
  return value === null || value === undefined || value === ''
}

/**
 * 取得 localStorage 項目
 */
function getItem(key: string): string | null {
  return window.localStorage.getItem(key)
}

/**
 * 設定 localStorage 項目
 */
function setItem(key: string, value: string): void {
  window.localStorage.setItem(key, value)
}

/**
 * 取得多語言標題
 */
function getFieldCaption(title: string, langJson: string): string {
  try {
    if (!langJson) return title
    
    const language = appInfo.language || 'zhTW'
    const langDict = JSON.parse(langJson)
    if (langDict && langDict[language]) {
      return langDict[language]
    }
  } catch {
    // 解析失敗，返回原始標題
  }
  return title
}

/**
 * 設定位置欄位寬度
 */
function setPosition() {
  // 左中右 (都有)
  if (oLeft.value.length > 0 && oCenter.value.length > 0 && oRight.value.length > 0) {
    oLeftCol.value = 'col-4'
    oCenterCol.value = 'col-4'
    oRightCol.value = 'col-4'
    return
  }

  // 中右
  if (oLeft.value.length === 0 && oCenter.value.length > 0 && oRight.value.length > 0) {
    oLeftCol.value = ''
    oCenterCol.value = 'col-8'
    oRightCol.value = 'col-4'
    return
  }

  // 左中
  if (oLeft.value.length > 0 && oCenter.value.length > 0 && oRight.value.length === 0) {
    oLeftCol.value = 'col-4'
    oCenterCol.value = 'col-8'
    oRightCol.value = ''
    return
  }

  // 左右
  if (oLeft.value.length > 0 && oCenter.value.length === 0 && oRight.value.length > 0) {
    oLeftCol.value = 'col-6'
    oCenterCol.value = ''
    oRightCol.value = 'col-6'
    return
  }

  // 單一欄位
  if (oLeft.value.length > 0 && oCenter.value.length === 0 && oRight.value.length === 0) {
    oLeftCol.value = 'col-12'
    oCenterCol.value = ''
    oRightCol.value = ''
    return
  }
  
  if (oLeft.value.length === 0 && oCenter.value.length > 0 && oRight.value.length === 0) {
    oLeftCol.value = ''
    oCenterCol.value = 'col-12'
    oRightCol.value = ''
    return
  }
  
  if (oLeft.value.length === 0 && oCenter.value.length === 0 && oRight.value.length > 0) {
    oLeftCol.value = ''
    oCenterCol.value = ''
    oRightCol.value = 'col-12'
    return
  }
}

/**
 * 載入選單項目
 */
async function funcCMSMenus(menuGuidParam: string) {
  try {
    const para = { MenuGuid: menuGuidParam }
    const response = await apiGet('/api/CMS/GetCMSMenus', para)
    
    if (response.status === 200 && response.data) {
      const data = typeof response.data === 'string' 
        ? JSON.parse(response.data) 
        : response.data
      
      return { status: 200, data }
    } else {
      return { status: 204, data: [] }
    }
  } catch (error) {
    console.error('❌ 載入選單項目失敗:', error)
    throw error
  }
}

/**
 * 載入頁面資料
 */
async function loadPageData() {
  loading.value = true
  menuGuid.value = route.params.menuGuid as string
  
  if (!isNullOrEmpty(menuGuid.value)) {
    try {
      // 檢查快取
      const cacheKey = menuGuid.value + '_menu'
      const cachedData = getItem(cacheKey)
      
      let response
      if (!isNullOrEmpty(cachedData)) {
        response = JSON.parse(cachedData)
      } else {
        response = await funcCMSMenus(menuGuid.value)
        if (response.status === 200) {
          setItem(cacheKey, JSON.stringify(response))
        }
      }

      if (response.status === 200) {
        controlCount.value = response.data.length
        menuData.value = response.data
        
        if (menuData.value.length > 0) {
          menuTitle.value = menuData.value[0].MenuTitle || ''
          
          // 依照位置分組
          oTop.value = menuData.value.filter((rs: any) => rs.MenusPosition === '1')
          oLeft.value = menuData.value.filter((rs: any) => rs.MenusPosition === '2')
          oCenter.value = menuData.value.filter((rs: any) => rs.MenusPosition === '3')
          oRight.value = menuData.value.filter((rs: any) => rs.MenusPosition === '4')
          oBottom.value = menuData.value.filter((rs: any) => rs.MenusPosition === '5')
          
          setPosition()
        }
      }
      
      if (response.status === 204) {
        loading.value = false
      }
    } catch (err) {
      showAlert(String(err), appInfo.title)
    }
  }
  
  loading.value = false
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  loadPageData()
  
  // 監聽視窗大小變化
  window.addEventListener('resize', () => {
    isLargeScreen.value = sizes()['screen-large']
  })
})
</script>

<style lang="scss" scoped>
@use "sass:math";
$gutter-width: 0px;
$grid-num: 12;

@mixin pad {
  @media (max-width: 768px) {
    @content;
  }
}

* {
  box-sizing: border-box;
}

.container {
  margin: auto;
  padding: 0;
}

.row {
  display: flex;
  flex-wrap: wrap;
}

%col {
  padding-left: math.div($gutter-width, 2);
  padding-right: math.div($gutter-width, 2);
}

@for $i from 1 through $grid-num {
  .col-#{$i} {
    @extend %col;
    width: 100% * math.div($i, $grid-num);
    @include pad {
      width: 100%;
    }
  }
}

.content-block {
  margin: 2px 2px;
}

.dx-card {
  background: white;
  border-radius: 4px;
  padding: 10px;
  margin-bottom: 10px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12);
}
</style>
