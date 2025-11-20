<template>
  <div class="admin-grid-form">
    <DxLoadPanel
      :visible="loading"
      :shading="true"
      message="載入中..."
    />

    <div class="grid-container">
      <h3 v-if="xmlCaption">{{ xmlCaption }}</h3>
      
      <!-- 簡化版：顯示 XMLName 和基本說明 -->
      <div class="placeholder-content">
        <div class="info-box">
          <p><strong>組件名稱:</strong> AdminGridForm</p>
          <p><strong>XML 名稱:</strong> {{ xmlName }}</p>
          <p><strong>XML 參數:</strong> {{ xmlParameter || '(無)' }}</p>
          <p><strong>標題:</strong> {{ xmlCaption }}</p>
        </div>

        <div class="note">
          <p>📋 <strong>說明</strong>: 這是 AdminGridForm 的簡化佔位組件</p>
          <p>完整版本需要從 Bia 專案移植，包含:</p>
          <ul>
            <li>動態表單載入 (基於 XML 定義)</li>
            <li>網格編輯 (CRUD 操作)</li>
            <li>進階篩選和查詢</li>
            <li>匯入/匯出功能</li>
            <li>自訂工具列</li>
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

console.log('📋 AdminGridForm (簡化版) 已載入:', props.xmlName)
</script>

<style scoped lang="scss">
.admin-grid-form {
  padding: 20px;
  background: white;
  border-radius: 8px;
  min-height: 200px;
}

.grid-container {
  h3 {
    margin-bottom: 15px;
    color: #333;
    border-bottom: 2px solid #337ab7;
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
    background: #fff3cd;
    border: 1px solid #ffc107;
    padding: 15px;
    border-radius: 5px;

    p {
      margin: 5px 0;
      color: #856404;
    }

    ul {
      margin: 10px 0 0 20px;
      color: #856404;

      li {
        margin: 5px 0;
      }
    }
  }
}
</style>

