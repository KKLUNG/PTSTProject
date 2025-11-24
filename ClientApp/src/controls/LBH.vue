<template>
  <div class="webPartHeader">
    <div class="webPartCaption">
      <div><i class="icon dx-icon-bookmark toolbarIcon" /></div>
      <div>
        {{ caption }}
      </div>
    </div>
    <hr style="border: 0px" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useControlBase } from '@/composables/useControlBase'

interface Props {
  fieldName?: string
  oBody?: any[]
}

const props = withDefaults(defineProps<Props>(), {
  fieldName: '',
  oBody: () => []
})

const { appInfo } = useControlBase()

const caption = ref<string>('')
const captionClass = computed(() => {
  return appInfo?.isMobile ? "formButtonArea1" : "formButtonArea"
})

onMounted(() => {
  if (props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.fieldName)
    if (o.length > 0) {
      // TODO: 實作 getFieldCaption 方法
      caption.value = o[0].FCaption || ''
    }
  }
})
</script>

<style lang="scss" scoped>
@use "@/css/scssToFunction.module.scss" as *;

.formButtonArea {
  background-color: var(--baseWebpartHeaderBackgroundColor);
  color: var(--webpartHeaderColor);
  margin-left: -10px;
  margin-right: -10px;
}

.formButtonArea1 {
  margin-left: -10px;
  margin-right: -10px;
}
</style>

