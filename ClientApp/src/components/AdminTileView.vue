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

    <DxPopup
      ref="rootPopup"
      width="100vw"
      height="100vh"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :full-screen="appInfo.isMobile"
      :visible="isRootPopupVisible"
      @hidden="onRootPopupHidden"
    >
      <template #title>
        <div class="toolbarArea" style="padding: 5px">
          <div class="leftToolbar">
            <img :src="logoUrl" width="120px" />
          </div>
          <div class="rightToolbar">
            <DxButton icon="close" @click="onCloseRootPopup" />
          </div>
        </div>
      </template>
      <DxScrollView
        width="100%"
        height="100%"
        :bounce-enabled="false"
        :use-native="appInfo.isMobile ? false : appInfo.isNativeScrollBar"
        show-scrollbar="always"
      >
        <div>
          <!-- TODO: 需要實作 CMSFrame 組件 -->
          <div v-if="isRootPopup" style="padding: 20px;">
            <p>CMSFrame 組件尚未實作</p>
            <p>URL: {{ popupUrl }}</p>
          </div>
        </div>
      </DxScrollView>
    </DxPopup>

    <div>
      <div
        :class="appInfo.isMobile ? 'webPartHeaderTransparent' : 'webPartHeader'"
        v-if="oHead.HasHeader == '1'"
      >
        <div class="webPartCaption">
          <div><i :class="toolbarClass" /></div>
          <div v-html="Caption"></div>
        </div>
      </div>
      <hr v-if="oHead.HasHeader == '1'" style="border: 0px" />

      <div
        :class="backgroundCSS"
        style="margin-top: 0px; width: 100%"
        id="child"
      >
        <DxTileView
          ref="tileView"
          :data-source="{
            store: {
              type: 'array',
              data: oData,
              key: oHead.KeyField,
            },
          }"
          :width="contentWidth"
          :height="contentHeight"
          show-scrollbar="always"
          :base-item-height="baseItemHeight"
          :base-item-width="baseItemWidth"
          :item-margin="itemMargin"
          :direction="direction"
          @item-click="onItemClick"
        >
          <template #item="{ data: item }">
            <div class="tile-item">
              <div class="tile-title">{{ item[oHead.TitleField] }}</div>
              <div class="tile-content">{{ item[oHead.ContentField] }}</div>
            </div>
          </template>
        </DxTileView>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import DxLoadPanel from 'devextreme-vue/load-panel'
import DxPopup from 'devextreme-vue/popup'
import DxButton from 'devextreme-vue/button'
import DxScrollView from 'devextreme-vue/scroll-view'
import DxTileView from 'devextreme-vue/tile-view'
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
const rootPopup = ref<InstanceType<typeof DxPopup> | null>(null)
const isRootPopup = ref(false)
const isRootPopupVisible = ref(false)
const popupUrl = ref('')
const tileView = ref<InstanceType<typeof DxTileView> | null>(null)
const contentWidth = ref('100%')
const contentHeight = ref('600px')
const baseItemHeight = ref(200)
const baseItemWidth = ref(300)
const itemMargin = ref(10)
const direction = ref<'horizontal' | 'vertical'>('horizontal')

// ============================================
// Computed
// ============================================
const logoUrl = computed(() => {
  return appInfo.titleLogo || '/logo.png'
})

const Caption = computed(() => {
  return props.XMLCaption || oHead.value.GVHeader || ''
})

const toolbarClass = computed(() => {
  return oHead.value.UCIcon
    ? `icon dx-icon-${oHead.value.UCIcon} toolbarIcon`
    : 'icon dx-icon-tiles toolbarIcon'
})

const backgroundCSS = computed(() => {
  return isNullOrEmpty(props.XMLParameter)
    ? 'responsive-paddings2 tile-background'
    : 'responsive-paddings1'
})

// ============================================
// 方法
// ============================================
const isNullOrEmpty = (value: any): boolean => {
  return value === null || value === undefined || value === ''
}

const onItemClick = (e: any) => {
  // TODO: 實作項目點擊邏輯
  console.log('Tile item clicked:', e)
  // 可以開啟彈窗顯示詳細資訊
  // popupUrl.value = `/AdminFormView/3/${props.XMLName};${e.itemData[oHead.value.KeyField]}`
  // isRootPopupVisible.value = true
  // isRootPopup.value = true
}

const onCloseRootPopup = () => {
  isRootPopupVisible.value = false
  isRootPopup.value = false
}

const onRootPopupHidden = () => {
  isRootPopup.value = false
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
.tile-item {
  padding: 10px;
  background: white;
  border-radius: 4px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.tile-title {
  font-weight: bold;
  margin-bottom: 5px;
}

.tile-content {
  color: #666;
  font-size: 0.9em;
}
</style>

