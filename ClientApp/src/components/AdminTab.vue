<template>
  <div class="admin-tab">
    <DxLoadPanel
      :visible="loading"
      :shading="true"
      message="載入中..."
    />

    <div class="tab-container">
      <h3 v-if="xmlCaption">{{ xmlCaption }}</h3>
      
      <!-- 簡化版：顯示 XMLName 和基本說明 -->
      <div class="placeholder-content">
        <div class="info-box">
          <p><strong>組件名稱:</strong> AdminTab</p>
          <p><strong>XML 名稱:</strong> {{ xmlName }}</p>
          <p><strong>XML 參數:</strong> {{ xmlParameter || '(無)' }}</p>
          <p><strong>標題:</strong> {{ xmlCaption }}</p>
        </div>

        <div class="note">
          <p>📑 <strong>說明</strong>: 這是 AdminTab 的簡化佔位組件</p>
          <p>完整版本需要從 Bia 專案移植，包含:</p>
          <ul>
            <li>動態頁籤載入 (基於 XML 定義)</li>
            <li>多個子組件容器</li>
            <li>頁籤狀態管理</li>
            <li>延遲載入</li>
          </ul>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import DxLoadPanel from 'devextreme-vue/load-panel'

// ============================================
// Props
// ============================================
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

// ============================================
// 狀態變數
// ============================================
const loading = ref(false)

console.log('📑 AdminTab (簡化版) 已載入:', props.xmlName)
</script>

<style scoped lang="scss">
.admin-tab {
  padding: 20px;
  background: white;
  border-radius: 8px;
  min-height: 200px;
}

.tab-container {
  h3 {
    margin-bottom: 15px;
    color: #333;
    border-bottom: 2px solid #6c757d;
    padding-bottom: 10px;
  }
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
    background: #e7e9ed;
    border: 1px solid #6c757d;
    padding: 15px;
    border-radius: 5px;

    p {
      margin: 5px 0;
      color: #495057;
    }

    ul {
      margin: 10px 0 0 20px;
      color: #495057;

      li {
        margin: 5px 0;
      }
    }
  }
}
</style>

