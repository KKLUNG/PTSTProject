<template>
  <div class="admin-frame">
    <DxLoadPanel :visible="loading" :shading="true" message="載入中..." />

    <div class="frame-container">
      <h3 v-if="xmlCaption">{{ xmlCaption }}</h3>
      
      <div class="placeholder-content">
        <div class="info-box">
          <p><strong>組件名稱:</strong> AdminFrame</p>
          <p><strong>XML 名稱:</strong> {{ xmlName }}</p>
          <p><strong>XML 參數:</strong> {{ xmlParameter || '(無)' }}</p>
          <p><strong>標題:</strong> {{ xmlCaption }}</p>
        </div>

        <div class="note">
          <p>🖼️ <strong>說明</strong>: 這是 AdminFrame 的簡化佔位組件</p>
          <p>完整版本需要從 Bia 專案移植</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import DxLoadPanel from 'devextreme-vue/load-panel'

interface Props {
  xmlName?: string
  xmlParameter?: string
  xmlCaption?: string
  pageKey?: string
}

const props = withDefaults(defineProps<Props>(), {
  xmlName: '',
  xmlParameter: '',
  xmlCaption: '',
  pageKey: ''
})

const loading = ref(false)
console.log('🖼️ AdminFrame (簡化版) 已載入:', props.xmlName)
</script>

<style scoped lang="scss">
.admin-frame {
  padding: 20px;
  background: white;
  border-radius: 8px;
  min-height: 200px;
}

.frame-container h3 {
  margin-bottom: 15px;
  color: #333;
  border-bottom: 2px solid #17a2b8;
  padding-bottom: 10px;
}

.placeholder-content {
  .info-box {
    background: #f8f9fa;
    padding: 15px;
    border-radius: 5px;
    margin-bottom: 15px;

    p {
      margin: 5px 0;
      color: #555;
    }
  }

  .note {
    background: #d1ecf1;
    border: 1px solid #17a2b8;
    padding: 15px;
    border-radius: 5px;

    p {
      margin: 5px 0;
      color: #0c5460;
    }
  }
}
</style>

