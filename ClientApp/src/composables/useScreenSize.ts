/**
 * 響應式斷點 Composable
 * 偵測螢幕大小並提供響應式狀態
 */

import { ref, onMounted, onBeforeUnmount } from 'vue'

export function useScreenSize() {
  const screenWidth = ref(window.innerWidth)
  const isXSmall = ref(false)
  const isSmall = ref(false)
  const isMedium = ref(false)
  const isLarge = ref(false)

  const updateSize = () => {
    screenWidth.value = window.innerWidth
    isXSmall.value = screenWidth.value < 576
    isSmall.value = screenWidth.value >= 576 && screenWidth.value < 768
    isMedium.value = screenWidth.value >= 768 && screenWidth.value < 992
    isLarge.value = screenWidth.value >= 992
  }

  onMounted(() => {
    updateSize()
    window.addEventListener('resize', updateSize)
  })

  onBeforeUnmount(() => {
    window.removeEventListener('resize', updateSize)
  })

  return {
    screenWidth,
    isXSmall,
    isSmall,
    isMedium,
    isLarge
  }
}

