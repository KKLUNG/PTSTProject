<template>
  <div>
    <DxLoadPanel
      :hide-on-outside-click="false"
      :visible="loading"
      :shading="true"
      :show-pane="true"
      shading-color="transparent"
    />
    
    <DxPopup
      width="100%"
      height="100%"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :visible="folderPopupShow"
      :full-screen="appInfo.isMobile"
      @hidden="onFolderPopupHidden"
    >
      <template #title>
        <div class="toolbarArea" style="padding: 5px">
          <div class="leftToolbar">
            <img :src="logoUrl" width="120px" />
          </div>
          <div class="rightToolbar">
            <DxButton icon="close" @click="onCloseFolderPopup" />
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
          <div v-if="folderPopupShow" style="padding: 20px;">
            <p>CMSFrame 組件尚未實作</p>
            <p>URL: {{ popupUrl }}</p>
          </div>
        </div>
      </DxScrollView>
    </DxPopup>

    <div class="webPartHeader">
      <div class="webPartCaption">
        <div><i class="icon dx-icon-search toolbarIcon" /></div>
        <div>全文檢索 Full text search</div>
      </div>
    </div>
    <hr style="border: 0px" />
    <br />
    <br />
    <div class="responsive-paddings1">
      <div class="formButtonArea1">
        <div class="formButton" style="width: 70%">
          <DxTextBox
            :show-clear-button="true"
            placeholder="at least 2 characters"
            v-model:value="searchText"
            width="100%"
            @enter-key="onSearchClick"
          />
        </div>
        <div class="formButton" style="width: 30%; text-align: left">
          <DxButton
            icon="search"
            text="Search"
            type="default"
            :styling-mode="appInfo.formButtonStylingMode || 'contained'"
            @click="onSearchClick"
          />
        </div>
      </div>
    </div>
    <br />
    <br />
    <div class="responsive-paddings1">
      <DxDataGrid
        id="gridContainer"
        :data-source="dataSource"
        :column-auto-width="false"
        :show-borders="false"
        :show-column-headers="false"
        :word-wrap-enabled="true"
      >
        <DxColumn width="100%" data-field="CatalogItemID" />
        <DxPaging :page-size="10" :enabled="true" />
        <DxPager
          :show-page-size-selector="true"
          :allowed-page-sizes="[10, 20, 50]"
          :show-info="true"
        />
        <template #dataRowTemplate="{ data: rowInfo }">
          <div>
            <tbody style="width: 100%">
              <tr>
                <td>
                  <a
                    style="color: black"
                    @click="() => {
                      loading = false
                      folderPopupShow = true
                      popupUrl = rowInfo.data.popupUrl
                    }"
                  >
                    {{ rowInfo.data.CaptionAll }}
                  </a>
                </td>
              </tr>
              <tr>
                <td style="font-size: 14px; color: darkblue">
                  <!-- TODO: 需要實作 HDD 控件 -->
                  <div>
                    File: {{ rowInfo.data.FileUrl }}
                    <br />
                    Subject: {{ rowInfo.data.DocSubject }}
                  </div>
                </td>
              </tr>
              <tr>
                <td style="color: gray">
                  {{ rowInfo.data.DocDate }} {{ rowInfo.data.CreatedUserName }} —
                  <span v-html="rowInfo.data.Characterization"></span>
                </td>
              </tr>
            </tbody>
            <hr />
          </div>
        </template>
      </DxDataGrid>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import DxLoadPanel from 'devextreme-vue/load-panel'
import DxPopup from 'devextreme-vue/popup'
import DxButton from 'devextreme-vue/button'
import DxScrollView from 'devextreme-vue/scroll-view'
import DxTextBox from 'devextreme-vue/text-box'
import {
  DxDataGrid,
  DxColumn,
  DxPager,
  DxPaging,
} from 'devextreme-vue/data-grid'
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
const dataSource = ref<any>(null)
const searchText = ref('')
const folderPopupShow = ref(false)
const popupUrl = ref('')

// ============================================
// Computed
// ============================================
const logoUrl = computed(() => {
  return appInfo.titleLogo || '/logo.png'
})

// ============================================
// 方法
// ============================================
const isNullOrEmpty = (value: any): boolean => {
  return value === null || value === undefined || value === ''
}

const onSearchClick = async () => {
  if (isNullOrEmpty(searchText.value)) {
    alert('請輸入查詢字串')
    return
  }

  if (searchText.value.length < 2) {
    alert('請至少輸入2個字')
    return
  }

  loading.value = true
  try {
    const res = await apiPost('/api/func/GetFullTextSearch', {
      keyWords: searchText.value,
      UserGuid: appInfo.userInfo.userGuid,
    })

    if (res.data && res.data.length === 0) {
      alert('No data')
      dataSource.value = null
    } else {
      dataSource.value = res.data
    }
  } catch (err) {
    console.error(err)
    alert('Search error')
  } finally {
    loading.value = false
  }
}

const onCloseFolderPopup = () => {
  folderPopupShow.value = false
}

const onFolderPopupHidden = () => {
  folderPopupShow.value = false
}
</script>

<style lang="scss" scoped>
</style>

