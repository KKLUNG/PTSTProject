<template>
  <hr class="hrStyle" :style="cssStyle" />
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useControlBase } from '@/composables/useControlBase'

interface Props {
  fieldName?: string
  oBody?: any[]
}

const props = withDefaults(defineProps<Props>(), {
  fieldName: '',
  oBody: () => []
})

const { isNullOrEmpty } = useControlBase()

const cssStyle = ref<string>('')

onMounted(() => {
  if (props.oBody && props.oBody.length > 0) {
    const o = props.oBody.filter((x: any) => x.FName == props.fieldName)
    if (o.length > 0) {
      cssStyle.value = isNullOrEmpty(o[0].UDF01) ? "" : o[0].UDF01
    }
  }
})
</script>

<style lang="scss" scoped>
@use "@/themes/generated/variables.base.scss" as *;

.hrStyle {
  border-top: 5px solid $base-accent;
}
</style>

