<template>
  <div :style="cssStyle">{{ showText }}</div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
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

const { isNullOrEmpty } = useControlBase()

const showText = ref<string>('')
const cssStyle = ref<string>('')

onMounted(() => {
  if (props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.fieldName)
    if (o.length > 0) {
      cssStyle.value = isNullOrEmpty(o[0].UDF01) ? "" : o[0].UDF01
      const isFromDB = !isNullOrEmpty(o[0].UDF02) && o[0].UDF02 == '1'
      if (isFromDB) {
        showText.value = props.value || ''
      } else {
        // TODO: 實作 getFieldCaption 方法
        showText.value = o[0].FCaption || ''
      }
    }
  }
})
</script>

<style lang="scss" scoped>
</style>

