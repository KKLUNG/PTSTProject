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

    <div>
      <DxTreeList
        :data-source="{
          store: {
            type: 'array',
            data: oData,
            key: oHead.KeyField,
          },
        }"
        :ref="widgetID"
        :root-value="rootGuid"
        :allow-column-resizing="true"
        :show-row-lines="false"
        :show-borders="false"
        :auto-expand-all="false"
        :key-expr="oHead.KeyField"
        :parent-id-expr="oHead.ParentField"
        :focused-row-enabled="oHead.HasFocus == '1'"
        :focused-row-index="oHead.HasFocus == '1' ? 0 : -1"
        :word-wrap-enabled="oHead.HasWordWarp == '1'"
        :height="gridHeight"
        :column-auto-width="oHead.ColumnAutoWidth == '1'"
        :column-hiding-enabled="oHead.AutoHideColumn == '1'"
      >
        <DxScrolling
          :mode="scrollingMode"
          :use-native="appInfo.isMobile ? false : appInfo.isNativeScrollBar"
          show-scrollbar="always"
        />
        <DxPaging
          :page-size="oHead.PageSize"
          :enabled="oHead.HasPaging == '1'"
        />
        <DxPager
          :show-page-size-selector="oHead.HasPaging == '1'"
          :allowed-page-sizes="pageSizes"
          :show-info="true"
        />
        <DxSearchPanel :visible="oHead.HasSearch == '1'" />

        <DxColumn
          v-for="(item, index) in oBody"
          :key="index + 'TC'"
          :data-field="item.FName"
          :caption="item.FCaption"
          :width="item.FWidth"
          :visible="item.FShowGV == '1'"
        />
      </DxTreeList>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxLoadPanel from 'devextreme-vue/load-panel'
import {
  DxTreeList,
  DxColumn,
  DxScrolling,
  DxPaging,
  DxPager,
  DxSearchPanel,
} from 'devextreme-vue/tree-list'
import appInfo from '@/utils/app-Info'
import { apiPost } from '@/utils/api-util'

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
// 狀態變數
// ============================================
const loading = ref(false)
const loadingMessage = ref('載入中...')
const oHead = ref<any>({})
const oBody = ref<any[]>([])
const oData = ref<any[]>([])
const widgetID = ref(getNewGuid() + 'treeList')
const rootGuid = ref('')
const gridHeight = ref('600px')
const scrollingMode = ref<'standard' | 'virtual' | 'infinite'>('standard')
const pageSizes = ref([10, 20, 50, 100])

// ============================================
// 方法
// ============================================
const isNullOrEmpty = (value: any): boolean => {
  return value === null || value === undefined || value === ''
}

const getNewGuid = (): string => {
  return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
    const r = (Math.random() * 16) | 0
    const v = c == 'x' ? r : (r & 0x3) | 0x8
    return v.toString(16)
  })
}

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  rootGuid.value = props.XMLParameter.split(';')[0]?.toLowerCase() || ''

  if (isNullOrEmpty(rootGuid.value)) {
    alert('rootGuid undefined on CMSMenus')
    loading.value = false
    return
  }

  loading.value = true

  // TODO: 實作 funcAll 方法來載入表單定義和資料
  // Promise.all([
  //   funcAll(props.XMLName, keyParameter, parentParameter, props.XMLParameter)
  // ]).then((res) => {
  //   oHead.value = JSON.parse(res[0].data.head)[0]
  //   oBody.value = JSON.parse(res[0].data.body)
  //   // 載入資料
  //   loadData()
  // })

  // 暫時設定預設值
  loading.value = false
})

const loadData = async () => {
  // TODO: 實作資料載入邏輯
  // const para: any = {}
  // para['XMLName'] = props.XMLName
  // para['KeyParameter'] = props.XMLParameter
  // para['UserGuid'] = appInfo.userInfo.userGuid
  // para['UserId'] = appInfo.userInfo.userId
  // para['Lang'] = appInfo.language
  //
  // const res = await apiPost('/api/cms/GetDataByXMLName', para)
  // if (res.status === 200) {
  //   oData.value = res.data
  // } else {
  //   oData.value = []
  // }
  // loading.value = false
}
</script>

<style lang="scss" scoped>
</style>

