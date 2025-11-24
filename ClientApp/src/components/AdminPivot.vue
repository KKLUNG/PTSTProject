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

    <div class="webPartHeader" v-if="!isNullOrEmpty(Caption)">
      <div class="webPartCaption">
        <div><i :class="toolbarClass" /></div>
        <div v-html="Caption"></div>
      </div>
    </div>

    <hr style="border: 0px" />

    <DxPivotGrid
      :ref="widgetID"
      :data-source="{
        store: {
          type: 'array',
          data: oData,
        },
      }"
      :allow-sorting="allowSorting"
      :allow-filtering="allowFiltering"
      :show-borders="showBorders"
      :height="pivotHeight"
    >
      <DxFieldChooser :enabled="fieldChooserEnabled" />
      <DxExport :enabled="exportEnabled" />
    </DxPivotGrid>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxLoadPanel from 'devextreme-vue/load-panel'
import {
  DxPivotGrid,
  DxFieldChooser,
  DxExport,
} from 'devextreme-vue/pivot-grid'
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
const widgetID = ref(getNewGuid() + 'pivot')
const allowSorting = ref(true)
const allowFiltering = ref(true)
const showBorders = ref(true)
const pivotHeight = ref('600px')
const fieldChooserEnabled = ref(true)
const exportEnabled = ref(true)

// ============================================
// Computed
// ============================================
const Caption = computed(() => {
  return props.XMLCaption || oHead.value.GVHeader || ''
})

const toolbarClass = computed(() => {
  return oHead.value.UCIcon
    ? `icon dx-icon-${oHead.value.UCIcon} toolbarIcon`
    : 'icon dx-icon-pivot toolbarIcon'
})

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
  loading.value = true

  // TODO: 實作 funcAll 方法來載入表單定義和資料
  // Promise.all([
  //   funcAll(props.XMLName, keyParameter, parentParameter, props.XMLParameter)
  // ]).then((res) => {
  //   oHead.value = JSON.parse(res[0].data.head)[0]
  //   oBody.value = JSON.parse(res[0].data.body)
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

