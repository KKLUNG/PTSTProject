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
          <div class="dx-card" v-for="item in oTop" :key="item.MenusGuid">
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
          <div class="dx-card" v-for="item in oLeft" :key="item.MenusGuid">
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
          <div class="dx-card" v-for="item in oCenter" :key="item.MenusGuid">
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
          <div class="dx-card" v-for="item in oRight" :key="item.MenusGuid">
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
          <div class="dx-card" v-for="item in oBottom" :key="item.MenusGuid">
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
import DxLoadPanel from 'devextreme-vue/load-panel'
import DxPopup from 'devextreme-vue/popup'

// ============================================
// Vue 實例和路由
// ============================================
const route = useRoute()
const { appContext } = getCurrentInstance()!

// ============================================
// 全域屬性
// ============================================
const appInfo = appContext.config.globalProperties.$appInfo

// ============================================
// 狀態變數
// ============================================
const loading = ref(false)
const loadingMessage = ref('Loading...please Wait')
const hasNewApp = ref(false)
const downloadProgress = ref('')
const isLargeScreen = ref(window.innerWidth > 960)
const menuTitle = ref('Home Page')
const pageKey = ref('')

// 菜單項目（按位置分類）
const oTop = ref<any[]>([])
const oLeft = ref<any[]>([])
const oCenter = ref<any[]>([])
const oRight = ref<any[]>([])
const oBottom = ref<any[]>([])

// ============================================
// Computed 屬性
// ============================================
// 計算欄位寬度
const oLeftCol = computed(() => {
  const hasCenter = oCenter.value.length > 0
  const hasRight = oRight.value.length > 0
  if (hasCenter && hasRight) return 'col-lg-4 col-md-12'
  if (hasCenter || hasRight) return 'col-lg-6 col-md-12'
  return 'col-12'
})

const oCenterCol = computed(() => {
  const hasLeft = oLeft.value.length > 0
  const hasRight = oRight.value.length > 0
  if (hasLeft && hasRight) return 'col-lg-4 col-md-12'
  if (hasLeft || hasRight) return 'col-lg-6 col-md-12'
  return 'col-12'
})

const oRightCol = computed(() => {
  const hasLeft = oLeft.value.length > 0
  const hasCenter = oCenter.value.length > 0
  if (hasLeft && hasCenter) return 'col-lg-4 col-md-12'
  if (hasLeft || hasCenter) return 'col-lg-6 col-md-12'
  return 'col-12'
})

// ============================================
// 輔助函數
// ============================================
/**
 * 獲取欄位標題（支持多語言）
 */
const getFieldCaption = (title: string, titleLang: string): string => {
  // TODO: 實作多語言支援
  // 目前先返回 title，之後可以根據 titleLang 和當前語言返回對應的文字
  try {
    if (titleLang) {
      const langObj = JSON.parse(titleLang)
      const currentLang = appInfo.language || 'zhTW'
      return langObj[currentLang] || title
    }
  } catch (e) {
    // 如果 JSON 解析失敗，返回原始 title
  }
  return title
}

/**
 * 載入首頁菜單資料
 */
const loadMenuData = async () => {
  loading.value = true
  try {
    // TODO: 實作從 API 載入菜單資料
    // 目前使用模擬資料
    
    // 範例：從 route params 獲取頁面 GUID
    const pageGuid = route.params.guid as string || appInfo.homeGuid
    
    // TODO: 呼叫 API 獲取菜單資料
    // const response = await apiGet(`/api/cms/GetHomePageMenus?pageGuid=${pageGuid}`)
    // const menuData = response.data
    
    // 暫時使用空陣列
    oTop.value = []
    oLeft.value = []
    oCenter.value = []
    oRight.value = []
    oBottom.value = []
    
    console.log('📋 CMSHomePage: 載入菜單資料', { pageGuid })
  } catch (error) {
    console.error('❌ 載入菜單資料失敗:', error)
  } finally {
    loading.value = false
  }
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  // 設定 pageKey（用於組件間通訊）
  pageKey.value = `homepage_${Date.now()}`
  
  // 載入菜單資料
  loadMenuData()
  
  // 監聽視窗大小變化
  window.addEventListener('resize', () => {
    isLargeScreen.value = window.innerWidth > 960
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

.col-lg-4 {
  @extend %col;
  width: 33.333333%;
  @include pad {
    width: 100%;
  }
}

.col-lg-6 {
  @extend %col;
  width: 50%;
  @include pad {
    width: 100%;
  }
}

.col-lg-8 {
  @extend %col;
  width: 66.666667%;
  @include pad {
    width: 100%;
  }
}

.col-md-12 {
  @extend %col;
  width: 100%;
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


