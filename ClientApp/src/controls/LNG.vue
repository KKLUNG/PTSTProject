<template>
  <div style="display: flex" v-show="!isEditing">
    <div v-show="langSet == 'zhTW'">
      繁tw:<u>{{ lang_zhTW }}</u>
    </div>
    <div v-show="langSet == 'zhCN'">
      簡cn:<u>{{ lang_zhCN }}</u>
    </div>
    <div v-show="langSet == 'enUS'">
      英en:<u>{{ lang_enUS }}</u>
    </div>
    <div v-show="langSet == 'vi'">
      越vi:<u>{{ lang_vi }}</u>
    </div>
    <div v-show="langSet == 'th'">
      泰th:<u>{{ lang_th }}</u>
    </div>
    <div v-show="langSet == 'id'">
      印id:<u>{{ lang_id }}</u>
    </div>
    <div v-show="langSet == 'kh'">
      柬kh:<u>{{ lang_kh }}</u>
    </div>
  </div>
  <div v-show="isEditing">
    <DxAccordion
      ref="accordionRef"
      :data-source="headerJson"
      :collapsible="true"
      :multiple="false"
      :selected-index="defaultSelectedIndex"
      :animation-duration="animationDuration"
      id="accordion-container"
      item-title-template="itemTitleTemplate"
    >
      <template #itemTitleTemplate="{ data }">
        <div class="formButtonArea1">
          <div style="font-size: 0.7em">{{ data.header }}</div>
        </div>
      </template>
      <template #item="{}">
        <div>
          <div style="display: flex; align-items: center">
            <div>繁tw:</div>
            <div style="width: 100%">
              <DxTextBox
                v-model:value="lang_zhTW"
                @value-changed="onTextBoxValueChange"
              />
            </div>
          </div>
          <div style="display: flex; align-items: center">
            <div>簡cn:</div>
            <div style="width: 100%">
              <DxTextBox
                v-model:value="lang_zhCN"
                @value-changed="onTextBoxValueChange"
              />
            </div>
          </div>
          <div style="display: flex; align-items: center">
            <div>英en:</div>
            <div style="width: 100%">
              <DxTextBox
                v-model:value="lang_enUS"
                @value-changed="onTextBoxValueChange"
              />
            </div>
          </div>
          <div style="display: flex; align-items: center">
            <div>越vi:</div>
            <div style="width: 100%">
              <DxTextBox
                v-model:value="lang_vi"
                @value-changed="onTextBoxValueChange"
              />
            </div>
          </div>
          <div style="display: flex; align-items: center">
            <div>泰th:</div>
            <div style="width: 100%">
              <DxTextBox
                v-model:value="lang_th"
                @value-changed="onTextBoxValueChange"
              />
            </div>
          </div>
          <div style="display: flex; align-items: center">
            <div>印id:</div>
            <div style="width: 100%">
              <DxTextBox
                v-model:value="lang_id"
                @value-changed="onTextBoxValueChange"
              />
            </div>
          </div>
          <div style="display: flex; align-items: center">
            <div>柬kh:</div>
            <div style="width: 100%">
              <DxTextBox
                v-model:value="lang_kh"
                @value-changed="onTextBoxValueChange"
              />
            </div>
          </div>
          <DxButton icon="refresh" hint="還原" @click="onClear" />
        </div>
      </template>
    </DxAccordion>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import DxAccordion from 'devextreme-vue/accordion'
import DxTextBox from 'devextreme-vue/text-box'
import DxButton from 'devextreme-vue/button'
import { useControlBase } from '@/composables/useControlBase'

interface Props {
  isEditing?: boolean
  value?: string | null
  onValueChanged?: (value: any, fieldName: string) => void
  fieldName?: string
}

const props = withDefaults(defineProps<Props>(), {
  isEditing: true,
  value: null,
  onValueChanged: () => () => {},
  fieldName: ''
})

const { isNullOrEmpty, appInfo } = useControlBase()

const jsonValue = ref<string>('')
const jsonObj = ref<Record<string, string>>({})
const lang_zhTW = ref<string>('')
const lang_zhCN = ref<string>('')
const lang_enUS = ref<string>('')
const lang_vi = ref<string>('')
const lang_th = ref<string>('')
const lang_id = ref<string>('')
const lang_kh = ref<string>('')
const langSet = ref<string>(appInfo?.language || 'zhTW')
const headerJson = ref([{ header: "Language Set" }])
const animationDuration = ref<number>(300)
const defaultSelectedIndex = ref<number>(-1)
const accordionRef = ref()

const onTextBoxValueChange = () => {
  jsonObj.value["zhTW"] = lang_zhTW.value
  jsonObj.value["zhCN"] = lang_zhCN.value
  jsonObj.value["enUS"] = lang_enUS.value
  jsonObj.value["vi"] = lang_vi.value
  jsonObj.value["th"] = lang_th.value
  jsonObj.value["id"] = lang_id.value
  jsonObj.value["kh"] = lang_kh.value

  jsonValue.value = JSON.stringify(jsonObj.value)
  props.onValueChanged?.(jsonValue.value, props.fieldName || '')
}

const onClear = () => {
  lang_zhTW.value = ""
  lang_zhCN.value = ""
  lang_enUS.value = ""
  lang_vi.value = ""
  lang_th.value = ""
  lang_id.value = ""
  lang_kh.value = ""
  jsonObj.value["zhTW"] = lang_zhTW.value
  jsonObj.value["zhCN"] = lang_zhCN.value
  jsonObj.value["enUS"] = lang_enUS.value
  jsonObj.value["vi"] = lang_vi.value
  jsonObj.value["th"] = lang_th.value
  jsonObj.value["id"] = lang_id.value
  jsonObj.value["kh"] = lang_kh.value

  jsonValue.value = JSON.stringify(jsonObj.value)
  props.onValueChanged?.(jsonValue.value, props.fieldName || '')
}

onMounted(() => {
  jsonObj.value = !isNullOrEmpty(props.value)
    ? JSON.parse(props.value || '{}')
    : {}
  lang_zhTW.value = jsonObj.value["zhTW"] || ''
  lang_zhCN.value = jsonObj.value["zhCN"] || ''
  lang_enUS.value = jsonObj.value["enUS"] || ''
  lang_vi.value = jsonObj.value["vi"] || ''
  lang_th.value = jsonObj.value["th"] || ''
  lang_id.value = jsonObj.value["id"] || ''
  lang_kh.value = jsonObj.value["kh"] || ''
})
</script>

<style lang="scss" scoped>
</style>

