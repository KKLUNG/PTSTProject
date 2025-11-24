<template>
  <div>
    <DxPopup
      ref="rootPopup"
      :width="'90%'"
      :height="'90%'"
      title="XMLEditor"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :visible="isRootPopupVisible"
      @hidden="onPopupHidden"
    >
      <template #title>
        <div class="toolbarArea" style="padding: 5px">
          <div class="leftToolbar">
            <img :src="logoUrl" width="120px" />
          </div>
          <div class="rightToolbar">
            <DxButton
              icon="close"
              @click="onClosePopup"
            />
          </div>
        </div>
      </template>

      <DxScrollView width="100%" height="100%">
        <div>
          <!-- TODO: 需要實作 CMSFrame 組件 -->
          <div v-if="isRootPopup" style="padding: 20px;">
            <p>CMSFrame 組件尚未實作</p>
            <p>URL: {{ popupUrl }}</p>
          </div>
        </div>
      </DxScrollView>
    </DxPopup>

    <footer class="footer">
      <div
        style="
          display: flex;
          justify-content: space-between;
          align-items: center;
        "
      >
        <div>
          Copyright © {{ new Date().getFullYear() }}
          {{ appInfo.title }}.<br />
          All trademarks or registered trademarks are property of their
          respective owners.
        </div>
        <div style="display: flex">
          <div v-if="!appInfo.isWarningOnHeader" style="background-color: red; color: white">
            {{ Message('sys_footer') }}
          </div>
          <div v-if="appInfo.isShowHelp">
            <i
              class="icon dx-icon-help"
              style="color: blue; font-size: 16px; cursor: pointer"
              @click="onHelpClick"
            />
          </div>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import DxPopup from 'devextreme-vue/popup'
import DxButton from 'devextreme-vue/button'
import DxScrollView from 'devextreme-vue/scroll-view'
// TODO: 需要實作 CMSFrame 組件
// import CMSFrame from '@/views/CMSFrame.vue'
import appInfo from '@/utils/app-Info'
import { apiGet } from '@/utils/api-util'

const route = useRoute()

// ============================================
// 狀態變數
// ============================================
const rootPopup = ref<InstanceType<typeof DxPopup> | null>(null)
const isRootPopup = ref(false)
const isRootPopupVisible = ref(false)
const popupUrl = ref('')

// ============================================
// Computed
// ============================================
const logoUrl = computed(() => {
  return appInfo.titleLogo || '/logo.png'
})

const routeMenuGuid = computed(() => {
  return route.params.menuGuid as string | undefined
})

// ============================================
// 方法
// ============================================
const onClosePopup = () => {
  isRootPopupVisible.value = false
  isRootPopup.value = false
}

const onPopupHidden = () => {
  isRootPopup.value = false
}

const onHelpClick = () => {
  if (!routeMenuGuid.value) return
  
  popupUrl.value = `/AdminFormView/3/Form_CMSMenuHelp.xml;${routeMenuGuid.value}`
  isRootPopupVisible.value = true
  isRootPopup.value = true
}

const Message = (key: string): string => {
  // TODO: 實作多語系訊息
  return key
}
</script>

<style lang="scss" scoped>
@use "../themes/bia-custom.scss" as *;

.footer {
  display: block;
  color: rgba(0, 0, 0, 0.7);
  padding: 0px 10px 5px 10px;
}

.toolbarArea {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.leftToolbar,
.rightToolbar {
  display: flex;
  align-items: center;
}
</style>

