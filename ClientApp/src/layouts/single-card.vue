<template>
  <dx-scroll-view height="100%" width="100%" class="with-footer single-card">
    <div
      v-if="!appInfo.showVideo"
      class="hero"
      :style="heroStyle"
    >
      <div class="dx-card" :style="cardStyle">
        <slot />
        <span style="color: gray">{{ appVersion }}</span>
      </div>
    </div>
    <div v-else class="hero">
      <div class="dx-card" :style="cardStyle">
        <slot />
        <span style="color: gray">version:{{ appVersion }}</span>
      </div>
    </div>
  </dx-scroll-view>
</template>

 <script setup lang="ts">
import { computed, getCurrentInstance, onBeforeUnmount, onMounted, ref } from "vue"
import DxScrollView from "devextreme-vue/scroll-view"

const instance = getCurrentInstance()
const appInfo = instance?.appContext.config.globalProperties.$appInfo ?? {}
const cssVariable = instance?.appContext.config.globalProperties.$cssVariable ?? {}

const appVersion = appInfo.appVersion ?? ""
const showVideo = appInfo.showVideo ?? false
const isCordova = appInfo.isCordova ?? false

// 在 Vite 中，public 目錄下的檔案會自動從根路徑提供
// 使用絕對路徑 /images/ 而不是相對路徑
const herobackgroundimage = isCordova 
  ? `images/heroBackground.png?v${appVersion}`
  : `/images/heroBackground.png?v${appVersion}`
const herobackgroundimage1 = isCordova
  ? `images/heroBackground1.png?v${appVersion}`
  : `/images/heroBackground1.png?v${appVersion}`
const herobackgroundimage2 = isCordova
  ? `images/heroBackground2.png?v${appVersion}`
  : `/images/heroBackground2.png?v${appVersion}`
const backgroundIndex = ref(0)
const timerId = ref<number | null>(null)

const heroStyle = computed(() => {
  const images = [herobackgroundimage, herobackgroundimage1, herobackgroundimage2]
  return {
    backgroundImage: `url(${images[backgroundIndex.value]})`,
  }
})

const cardStyle = computed(() => {
  const bgColor = cssVariable?.baseLoginBackgroundColor || "rgba(255, 255, 255, 0.8)"
  return {
    backgroundColor: bgColor,
  }
})

// 切換背景圖片
const toggleBackground = () => {
  if (backgroundIndex.value === 0) {
    backgroundIndex.value = 1
  } else if (backgroundIndex.value === 1) {
    backgroundIndex.value = 2
  } else {
    backgroundIndex.value = 0
  }
}

onMounted(() => {
  // 調試：印出背景圖片路徑
  console.log('herobackgroundimage:', herobackgroundimage)
  console.log('herobackgroundimage1:', herobackgroundimage1)
  console.log('herobackgroundimage2:', herobackgroundimage2)
  console.log('appVersion:', appVersion)
  console.log('isCordova:', isCordova)
  console.log('window.location.pathname:', window.location.pathname)
  
  // 預載入背景圖片
  const image1 = new Image()
  image1.src = herobackgroundimage
  const image2 = new Image()
  image2.src = herobackgroundimage1
  const image3 = new Image()
  image3.src = herobackgroundimage2
  console.log('image1:', image1)
  console.log('image2:', image2)
  console.log('image3:', image3)

  // 如果不是 Cordova 且不顯示影片，則每 5 秒切換背景
  if (!isCordova && !showVideo) {
    timerId.value = window.setInterval(toggleBackground, 5000)
  }
})

onBeforeUnmount(() => {
  if (timerId.value) {
    clearInterval(timerId.value)
  }
})
</script>

<style lang="scss">
.single-card {
  .dx-card {
    width: 330px;
    margin: auto auto;
    padding: 40px;

    .screen-x-small & {
      width: 100%;
      height: 100%;
      border-radius: 0;
      box-shadow: none;
      margin: 0;
      border: 0;
      flex-grow: 1;
    }
  }
}

.hero {
  /* Sizing */
  width: 100vw;
  height: 100vh;

  /* Flexbox stuff */
  display: flex;
  justify-content: center;
  align-items: center;

  /* Text styles */
  text-align: center;
  color: white;

  /* Background styles */
  background-size: cover;
  background-position: center center;
  background-repeat: no-repeat;
  background-attachment: fixed;

  /* Transitions*/
  transition: 1.5s;
  transition-timing-function: ease-in-out;
}
</style>

