<template>
  <div :id="chartDiv">
    <DxLoadPanel
      :hide-on-outside-click="false"
      :visible="loading"
      :shading="true"
      :show-pane="true"
      shading-color="transparent"
      :message="loadingMessage"
    />

    <DxChart
      :ref="widgetID"
      :data-source="oData"
      :palette="palette"
      :title="chartTitle"
      :common-series-settings="commonSeriesSettings"
      :size="chartSize"
      @point-click="onPointClick"
    >
      <DxSeries
        v-for="(series, index) in seriesConfig"
        :key="index"
        :value-field="series.valueField"
        :argument-field="series.argumentField"
        :name="series.name"
        :type="series.type"
      />
      <DxLegend :visible="legendVisible" />
      <DxExport :enabled="exportEnabled" />
      <DxTooltip :enabled="tooltipEnabled" />
    </DxChart>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from 'vue'
import DxLoadPanel from 'devextreme-vue/load-panel'
import {
  DxChart,
  DxSeries,
  DxLegend,
  DxExport,
  DxTooltip,
} from 'devextreme-vue/chart'
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
const widgetID = ref(getNewGuid() + 'chart')
const chartDiv = ref('chart_' + getNewGuid())
const palette = ref('Harmony Light')
const chartTitle = ref('')
const commonSeriesSettings = ref<any>({})
const chartSize = ref({ height: 400 })
const legendVisible = ref(true)
const exportEnabled = ref(true)
const tooltipEnabled = ref(true)
const seriesConfig = ref<Array<{
  valueField: string
  argumentField: string
  name: string
  type: string
}>>([])

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

const onPointClick = (e: any) => {
  // TODO: 實作點擊事件處理
  console.log('Point clicked:', e)
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
  //   chartTitle.value = props.XMLCaption || oHead.value.GVHeader || ''
  //   // 根據 oBody 設定 series
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

onBeforeUnmount(() => {
  // 清理資源
})
</script>

<style lang="scss" scoped>
</style>

