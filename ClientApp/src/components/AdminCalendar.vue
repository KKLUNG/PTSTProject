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

    <DxScheduler
      v-if="oHead.KeyField"
      :ref="widgetID"
      :data-source="{
        store: {
          type: 'array',
          data: oData,
          key: oHead.KeyField,
        },
      }"
      :current-date="currentDate"
      :views="views"
      :start-day-hour="startDayHour"
      :end-day-hour="endDayHour"
      :first-day-of-week="firstDayOfWeek"
      :height="schedulerHeight"
      :cell-duration="cellDuration"
      :groups="groups"
      :cross-scrolling-enabled="crossScrollingEnabled"
      :recurrence-edit-mode="recurrenceEditMode"
      @appointment-added="onAppointmentAdded"
      @appointment-updated="onAppointmentUpdated"
      @appointment-deleted="onAppointmentDeleted"
    >
      <DxResource
        v-for="resource in resources"
        :key="resource.fieldExpr"
        :field-expr="resource.fieldExpr"
        :data-source="resource.dataSource"
        :label="resource.label"
        :allow-multiple="resource.allowMultiple"
      />
    </DxScheduler>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxLoadPanel from 'devextreme-vue/load-panel'
import {
  DxScheduler,
  DxResource,
} from 'devextreme-vue/scheduler'
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
const widgetID = ref(getNewGuid() + 'calendar')
const currentDate = ref(new Date())
const views = ref(['day', 'week', 'month'])
const startDayHour = ref(8)
const endDayHour = ref(18)
const firstDayOfWeek = ref(0)
const schedulerHeight = ref('600px')
const cellDuration = ref(30)
const groups = ref<string[]>([])
const crossScrollingEnabled = ref(false)
const recurrenceEditMode = ref<'series' | 'occurrence'>('series')
const resources = ref<any[]>([])

// ============================================
// Computed
// ============================================
const Caption = computed(() => {
  return props.XMLCaption || oHead.value.GVHeader || ''
})

const toolbarClass = computed(() => {
  return oHead.value.UCIcon
    ? `icon dx-icon-${oHead.value.UCIcon} toolbarIcon`
    : 'icon dx-icon-calendar toolbarIcon'
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

const onAppointmentAdded = (e: any) => {
  // TODO: 實作新增預約邏輯
  console.log('Appointment added:', e)
}

const onAppointmentUpdated = (e: any) => {
  // TODO: 實作更新預約邏輯
  console.log('Appointment updated:', e)
}

const onAppointmentDeleted = (e: any) => {
  // TODO: 實作刪除預約邏輯
  console.log('Appointment deleted:', e)
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

