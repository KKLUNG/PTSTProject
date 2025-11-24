<template>
  <div style="flex-direction: column">
    <div class="lableStyle" :style="{ width: lblWidth }">
      <div class="labelAlign">{{ caption }}</div>
    </div>
    <div class="labelLine"></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { sizes } from '@/utils/media-query'
import { useControlBase } from '@/composables/useControlBase'

interface Props {
  value?: string | null
  fieldName?: string
  oBody?: any[]
}

const props = withDefaults(defineProps<Props>(), {
  value: null,
  fieldName: '',
  oBody: () => []
})

const { isNullOrEmpty, appInfo } = useControlBase()

const caption = ref<string>('')
const isLargeScreen = sizes()["screen-large"]

const lblWidth = computed(() => {
  if (appInfo?.isMobile) {
    return window.screen.availWidth / 3 + "px"
  } else if (isLargeScreen) {
    return window.screen.availWidth / 8 + "px"
  } else {
    return "100%"
  }
})

onMounted(() => {
  if (props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.fieldName)
    if (o.length > 0) {
      // TODO: 實作 getFieldCaption 方法
      if (isNullOrEmpty(props.value)) {
        caption.value = o[0].FCaption || ''
      } else {
        caption.value = props.value || ''
      }
    }
  }
})
</script>

<style lang="scss" scoped>
@use "@/themes/generated/variables.base.scss" as *;

.lableStyle {
  line-height: 2em;
  padding: 5px 5px 0px 5px;
  background-color: $base-accent;
  color: $base-bg;
  border-radius: 7px 7px 0px 0px;
}

.labelAlign {
  justify-content: center;
  align-items: center;
}

.labelLine {
  background-color: $base-accent;
  width: 100%;
  height: 3px;
  flex-direction: column;
}
</style>

