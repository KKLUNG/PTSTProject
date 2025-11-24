<template>
  <div>
    <DxLoadPanel
      :hide-on-outside-click="false"
      :visible="loading"
      :shading="true"
      :show-pane="true"
      shading-color="white"
      width="100%"
      height="100%"
      :message="loadingMessage"
    />

    <hr style="border: 0px" />
    <DxGantt
      ref="gantt"
      :task-list-width="600"
      height="90vh"
      :scale-type="scaleType"
      :task-title-position="taskTitlePosition"
      :start-date-range="startDateRange"
      :end-date-range="endDateRange"
      :show-dependencies="oHead.HasInsert == '1'"
      @task-edit-dialog-showing="onTaskEditDialogShowing"
      @task-dbl-click="onTaskDblClick"
      @task-inserted="onTaskInserted"
      @task-inserting="onTaskInserting"
      @task-updated="onTaskUpdated"
      @task-deleted="onTaskDeleted"
    >
      <DxTasks :data-source="tasks" />
      <DxDependencies :data-source="dependencies" />
      <DxResources :data-source="resources" />
      <DxResourceAssignments :data-source="resourceAssignments" />
      <DxToolbar />
      <DxValidation :auto-update-parent-tasks="true" />
    </DxGantt>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxLoadPanel from 'devextreme-vue/load-panel'
import {
  DxGantt,
  DxTasks,
  DxDependencies,
  DxResources,
  DxResourceAssignments,
  DxToolbar,
  DxValidation,
} from 'devextreme-vue/gantt'
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
const tasks = ref<any[]>([])
const dependencies = ref<any[]>([])
const resources = ref<any[]>([])
const resourceAssignments = ref<any[]>([])
const gantt = ref<InstanceType<typeof DxGantt> | null>(null)
const scaleType = ref<'auto' | 'minutes' | 'hours' | 'days' | 'weeks' | 'months' | 'quarters' | 'years'>('days')
const taskTitlePosition = ref<'inside' | 'outside' | 'none'>('inside')
const startDateRange = ref<Date | null>(null)
const endDateRange = ref<Date | null>(null)

// ============================================
// 方法
// ============================================
const isNullOrEmpty = (value: any): boolean => {
  return value === null || value === undefined || value === ''
}

const onTaskEditDialogShowing = (e: any) => {
  // TODO: 實作任務編輯對話框顯示邏輯
  console.log('Task edit dialog showing:', e)
}

const onTaskDblClick = (e: any) => {
  // TODO: 實作任務雙擊邏輯
  console.log('Task double clicked:', e)
}

const onTaskInserted = (e: any) => {
  // TODO: 實作任務新增邏輯
  console.log('Task inserted:', e)
}

const onTaskInserting = (e: any) => {
  // TODO: 實作任務新增前驗證
  console.log('Task inserting:', e)
}

const onTaskUpdated = (e: any) => {
  // TODO: 實作任務更新邏輯
  console.log('Task updated:', e)
}

const onTaskDeleted = (e: any) => {
  // TODO: 實作任務刪除邏輯
  console.log('Task deleted:', e)
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
  //   tasks.value = res.data.tasks || []
  //   dependencies.value = res.data.dependencies || []
  //   resources.value = res.data.resources || []
  //   resourceAssignments.value = res.data.resourceAssignments || []
  // }
  // loading.value = false
}
</script>

<style lang="scss" scoped>
</style>

