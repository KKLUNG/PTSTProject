<template>
  <div class="admin-form-view">
    <DxLoadPanel
      :visible="loading"
      :shading="true"
      message="載入中..."
    />

    <div class="form-container">
      <h3 v-if="xmlCaption">{{ xmlCaption }}</h3>
      
      <!-- 簡化版：顯示 XMLName 和基本說明 -->
      <div class="placeholder-content">
        <div class="info-box">
          <p><strong>組件名稱:</strong> AdminFormView</p>
          <p><strong>XML 名稱:</strong> {{ xmlName }}</p>
          <p><strong>XML 參數:</strong> {{ xmlParameter || '(無)' }}</p>
          <p><strong>標題:</strong> {{ xmlCaption }}</p>
        </div>

        <div class="note">
          <p>📝 <strong>說明</strong>: 這是 AdminFormView 的簡化佔位組件</p>
          <p>完整版本需要從 Bia 專案移植，包含:</p>
          <ul>
            <li>動態表單載入 (基於 XML 定義)</li>
            <li>表單檢視 (唯讀模式)</li>
            <li>表單編輯 (新增/修改)</li>
            <li>欄位驗證</li>
            <li>工作流整合</li>
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

console.log('📝 AdminFormView (簡化版) 已載入:', props.xmlName)
</script>

<style scoped lang="scss">
.admin-form-view {
  padding: 20px;
  background: white;
  border-radius: 8px;
  min-height: 200px;
}

.form-container {
  h3 {
    margin-bottom: 15px;
    color: #333;
    border-bottom: 2px solid #28a745;
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
    background: #d1ecf1;
    border: 1px solid #17a2b8;
    padding: 15px;
    border-radius: 5px;

    p {
      margin: 5px 0;
      color: #0c5460;
    }

    ul {
      margin: 10px 0 0 20px;
      color: #0c5460;

      li {
        margin: 5px 0;
      }
    }
  }
}
</style>

