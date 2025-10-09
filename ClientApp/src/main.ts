import 'devextreme/dist/css/dx.common.css';
import './themes/generated/theme.base.css';
import './themes/generated/theme.additional.css';
import cssVariable from "@/css/scssToFunction.scss";


import { createApp } from 'vue'
import App from './App.vue'
import 'devextreme/dist/css/dx.light.css'
import themes from 'devextreme/ui/themes'
import appInfo from "./utils/app-info";


// App.prototype（Vue 2）
// app.config.globalProperties（Vue 3）
// App.prototype.$appInfo = appInfo;

themes.initialized(() => {
  const app = createApp(App)
  app.config.globalProperties.$appInfo = appInfo
  app.config.globalProperties.$cssVariable = cssVariable
  app.mount('#app')
})