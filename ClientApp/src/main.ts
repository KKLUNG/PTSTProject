import 'devextreme/dist/css/dx.common.css';
import './themes/generated/theme.base.css';
import './themes/generated/theme.additional.css';
import cssVariable from "@/css/scssToFunction.scss";


import { createApp } from 'vue'
import App from './App.vue'
import 'devextreme/dist/css/dx.light.css'
import themes from 'devextreme/ui/themes'
import appInfo from "./utils/app-info";
import { custom } from 'devextreme/ui/dialog';


declare module '@vue/runtime-core' {
  interface ComponentCustomProperties {
    alert: (message: string, title?: string, buttonText?: string, f?: () => void) => void;
  }
}


// App.prototype（Vue 2）
// app.config.globalProperties（Vue 3）
// App.prototype.$appInfo = appInfo;



themes.initialized(() => {
  const app = createApp(App)
  app.config.globalProperties.alert = (message: string, title?: string, buttonText?: string, f?: () => void) => {
    custom({
      title,
      messageHtml: message,
      buttons: [{
        text: !buttonText ? 'OK' : buttonText,
        onClick: (e) => ({ buttonText: e.component.option('text') })
      }]
    }).show().then(f);
  };
  app.config.globalProperties.$appInfo = appInfo
  app.config.globalProperties.$cssVariable = cssVariable
  app.mount('#app')
})