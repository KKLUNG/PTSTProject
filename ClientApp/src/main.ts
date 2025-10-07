import { createApp } from 'vue'
import App from './App.vue'
import 'devextreme/dist/css/dx.light.css'
import themes from 'devextreme/ui/themes'

themes.initialized(() => {
  createApp(App).mount('#app')
})

 