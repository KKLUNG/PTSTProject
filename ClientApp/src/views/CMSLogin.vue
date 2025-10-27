<template>
<div>
    <DxLoadPanel
      :hide-on-outside-click="false"
      :visible="loading"
      :shading="true"
      :show-pane="true"
      shading-color="transparent"
      message="loadingMessage"
    />
    <dx-validation-group ref="vg">
      <div class="login-header">
        <!--<div class="title">{{ title }}</div>-->
        <div class="loginTitle">{{ appInfo.titleLogo }}<br /></div>
        <br />
        <div class="loginSubTitle" v-html="logoVision"></div>
      </div>

      <div class="dx-field">
        <i class="icon dx-icon-user" style="font-size: 1.8em" />
        <dx-text-box
          placeholder="Your account"
          width="100%"
          :value.sync="login"
        >
          <dx-validator>
            <dx-required-rule message="Account is required" />
          </dx-validator>
        </dx-text-box>
      </div>

      <div class="dx-field">
        <i class="icon icon-lock" style="font-size: 1.8em" />
        <dx-text-box
          placeholder="Your password"
          width="100%"
          :mode="passwordMode"
          :value.sync="password"
          @enter-key="onEnterKey"
        >
          <dx-text-box-button
            :options="passwordButton"
            name="password"
            location="after"
          />
          <dx-validator>
            <dx-required-rule message="Password is required" />
          </dx-validator>
        </dx-text-box>
      </div>

      <div class="dx-field" v-if="appInfo.isShowFactorySet">
        <i class="icon dx-icon-globe" style="font-size: 1.8em" />
        <DxSelectBox
          :data-source="factorySet"
          display-expr="display"
          key-expr="value"
          width="100%"
          @value-changed="onSelectedFactory"
        />
      </div>

      <div
        v-show="isCaptcha"
        style="
          justify-content: flex-start;
          align-items: center;
          flex-wrap: wrap;
          display: flex;
        "
      >
        <div class="formButton1">
          <span :style="headerCaptionStyle"
            >{{ randomA }} + {{ randomB }} =
          </span>
        </div>
        <div class="formButton1">
          <DxNumberBox
            placeholder="Your answer"
            width="100px"
            :value.sync="answer"
            :step="0"
            @enter-key="onEnterKey"
          >
          </DxNumberBox>
        </div>
      </div>
      <!-- <div class="dx-field">
      <dx-check-box :value.sync="rememberUser" text="記住我" />
      </div> -->
      <br />
      <div class="dx-field">
        <DxButton
          :disabled="disableButton"
          ref="btnLogin"
          type="default"
          text="Login"
          width="100%"
          @click="onLoginClick"
        />
      </div>
    </dx-validation-group>
    <br />
    <!-- allen 2021.03.04 忘記密碼 -->
    <div class="dx-field" v-if="appInfo.isPasswordChangeAlert">
      <DxButton
        ref="submit"
        type="default"
        text="Forgot password"
        width="100%"
        @click="onForgetPassword"
      />
    </div>
    <img :src="logoUrl" width="120px" />
    <br />
    <div v-show="appInfo.isCordova" style="color: gray">
      v:{{ cordovaAppVersion }}
    </div>
    <DxPopup
      ref="rootPopup"
      :visible="popupVisible"
      :drag-enabled="false"
      :hide-on-outside-click="true"
      :show-title="true"
      :width="400"
      :height="450"
      title-template="title"
      @hidden="
        () => {
          popupVisible = false;
        }
      "
    >
      <template #title>
        <div class="toolbarArea" style="padding: 5px">
          <div class="leftToolbar">
            <img :src="logoUrl" width="120px" />
          </div>
          <div class="rightToolbar">
            <DxButton
              icon="icon dx-icon-close"
              @click="
                () => {
                  popupVisible = false;
                }
              "
            />
          </div>
        </div>
      </template>
      <div class="dx-field">
        <div style="margin: 0px auto">
          <h6>Forgot Password</h6>
        </div>
      </div>
      <div class="dx-field">
        <dx-text-box
          placeholder="Your account"
          width="100%"
          :value.sync="forgotPasswordId"
        >
          <dx-validator>
            <dx-required-rule message="Account is required" />
          </dx-validator>
        </dx-text-box>
      </div>
      <div class="dx-field">
        <dx-text-box
          placeholder="email or cellphone"
          width="100%"
          :value.sync="forgotPasswordEmailOrCellPhone"
        >
          <dx-validator>
            <dx-required-rule message="email or cellphone is required" />
          </dx-validator>
        </dx-text-box>
      </div>

      <div class="dx-field" v-if="appInfo.isShowFactorySet">
        <i class="icon dx-icon-globe" style="font-size: 1.8em" />
        <DxSelectBox
          ref="forgotSelect"
          :data-source="factorySet"
          display-expr="display"
          key-expr="value"
          width="100%"
          @value-changed="onSelectedFactory"
        />
      </div>

      <div
        v-show="isCaptcha"
        style="
          justify-content: flex-start;
          align-items: center;
          flex-wrap: wrap;
          display: flex;
        "
      >
        <div class="formButton1">
          <span :style="headerCaptionStyle"
            >{{ randomA }} + {{ randomB }} =
          </span>
        </div>
        <div class="formButton1">
          <DxNumberBox
            placeholder="Your answer"
            width="100px"
            :value.sync="answer"
          ></DxNumberBox>
        </div>
      </div>
      <br />
      <div class="dx-field">
        <dx-button
          :disabled="disableButton"
          type="default"
          text="Submit"
          width="100%"
          @click="onSendPasswordToEmail"
        />
      </div>
    </DxPopup>
  </div>
</template>

<script setup lang="ts">
import axios from 'axios'
import { ref, nextTick, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import DxButton from 'devextreme-vue/button'
import DxLoadPanel from 'devextreme-vue/load-panel'
import DxPopup from 'devextreme-vue/popup'
import auth from '../utils/auth'
import DxTextBox from "devextreme-vue/text-box";
import DxNumberBox from "devextreme-vue/number-box";
import { DxButton as DxTextBoxButton } from "devextreme-vue/text-box";
import DxValidationGroup from "devextreme-vue/validation-group";
import DxValidator, { DxRequiredRule } from "devextreme-vue/validator";
import DxSelectBox from "devextreme-vue/select-box";
import { getCurrentInstance } from 'vue'
import type dxButton from 'devextreme/ui/button';
import type { ValueChangedEvent } from 'devextreme/ui/select_box';
import type ValidationGroup from 'devextreme/ui/validation_group';
import type dxSelectBox from 'devextreme/ui/select_box'


// ============================================
// Vue 實例和路由
// ============================================
const { appContext } = getCurrentInstance()!
const router = useRouter()
const route = useRoute()

// ============================================
// 全域屬性
// ============================================
const appInfo = appContext.config.globalProperties.$appInfo
const cssVariable = appContext.config.globalProperties.$cssVariable || {}
const $ms = appContext.config.globalProperties.$ms
const showAlert = appContext.config.globalProperties.alert as (
  message: string, title?: string, buttonText?: string, f?: () => void
) => void
const alertThen = appContext.config.globalProperties.alertThen
const $speechBot = appContext.config.globalProperties.$speechBot

// ============================================
// 狀態變數
// ============================================
let loginCount = 0;
const popupVisible = ref(false);
const title = appInfo.title;
const login = ref('');
const password = ref('');
const rememberUser = ref(false);
const loading = ref(false);
const loadingMessage = ref('Loading...please Wait');
const logoUrl = "logo.png";
const randomAnswer = ref(0);
const randomA = ref(0);
const randomB = ref(0);
const answer = ref<number | null>(null);
const isCaptcha = ref(false);
const isLargeScreen = window.innerWidth > 960;
const passwordMode = ref<'password' | 'text'>('password');
const passwordButton: { icon: string; type: string; onClick: () => void } = {
  icon: 'key',
  type: 'default',
  onClick: () => {
    passwordMode.value = passwordMode.value === 'text' ? 'password' : 'text';
  },
};
const forgotPasswordId = ref<string>('');
const forgotPasswordEmailOrCellPhone = ref<string>('');
const disableButton = ref(false);
const brandSet = ref<any[]>([]);
const cordovaAppVersion = ref('');
const logoVision = appInfo.defaultLanguage === "zhTW" ? appInfo.loginVisionTW : appInfo.loginVisionEN;
const timer = ref<any>(null);
const headerCaptionStyle = ref("color:" + (cssVariable.baseFvTextEditorColor || '#000'))
const btnLogin = ref<{ instance: dxButton } | null>(null)
const vg = ref<{ instance: ValidationGroup } | null>(null);
const factorySet = ref<FactoryItem[]>([]);
const forgotSelect = ref<{ instance: dxSelectBox } | null>(null);

// ============================================
// TypeScript 型別定義
// ============================================
interface ForgotPasswordFormData {
  forgotPasswordId: string;
  forgotPasswordEmailOrCellPhone: string;
}

type DxValidationResult = ReturnType<ValidationGroup['validate']>;
type FactoryItem = { display: string; value: string; factory?: string };

// ============================================
// 輔助函數
// ============================================
// helper: unified POST wrapper
const apiPost = <T = any>(apiUrl: string, params: any) => {
  return axios.post<T>(apiUrl, params, {
    baseURL: appInfo.serverUrl ?? undefined,
    responseType: 'json',
    headers: {
      'Access-Control-Allow-Origin': '*',
      Authorization: 'Bearer ' + (auth.getToken() || '')
    }
  })
}

// Helper: Check if value is null or empty
const isNullOrEmpty = (value: any): boolean => {
  return value === null || value === undefined || value === '';
}


// ============================================
// 事件處理函數
// ============================================
const onLoginClick = (e: Event) => {
  if (appInfo.isShowFactorySet) {
    if (isNullOrEmpty(appInfo.factory)) {
      showAlert(`Please select location - ${appInfo.title}`)
      return;
    }
  }

  const result: DxValidationResult | undefined = vg.value?.instance.validate();
  if (!result || !result.isValid) {
    return;
  }

  disableButton.value = true;
  if (randomAnswer.value != answer.value && isCaptcha.value) {
    showAlert(`The answer is ${randomAnswer.value}, please try again. - ${appInfo.title}`)
    randomA.value = Math.floor(Math.random() * Math.floor(50));
    randomB.value = Math.floor(Math.random() * Math.floor(50));
    randomAnswer.value = randomA.value + randomB.value;

    loginCount++;
    if (loginCount > 5) {
      showAlert(`Please try it later!! - ${appInfo.title}`);
      const b = btnLogin.value!.instance;
      b?.option("disabled", true);
    }
    disableButton.value = false;
    return;
  }

  loading.value = true;
  const para = {
    userId: login.value,
    password: password.value,
  };
  logining(para);
}

const onEnterKey = (e: Event) => {
  onLoginClick(e);
}

const onSelectedFactory = (e: ValueChangedEvent) => {
  const selected = e.value as { factory: string; value: string; display: string };
  appInfo.factory = selected.factory;
  appInfo.serverUrl = selected.value;
}

const setDefault = () => {
  if (appInfo.isCordova) {
    if (appInfo.isShowFactorySet) {
      if (!isNullOrEmpty(appInfo.factory)) {
        const o = factorySet.value.filter(
          (x) => x.display == appInfo.factory
        );
        if (o.length > 0) {
          appInfo.serverUrl = o[0].value;
        }
      }
    }
    isCaptcha.value = false;
    appInfo.rootGuid = appInfo.mobileRoot;
    appInfo.homeGuid = appInfo.mobileHome;
  } else {
    if (appInfo.isMobile && appInfo.isShowMobileSwitch) {
      //手機網頁
      isCaptcha.value = appInfo.isShowCaptcha;
      appInfo.rootGuid = appInfo.mobileRoot;
      appInfo.homeGuid = appInfo.mobileHome;
    } else {
      //PC網頁
      isCaptcha.value = appInfo.isShowCaptcha; //for all
      appInfo.rootGuid = appInfo.pcRoot;
      appInfo.homeGuid = appInfo.pcHome;
    }
  }
  //keep rootGuid; used in header-toolbar
  window.localStorage.setItem("rootGuid", appInfo.rootGuid || '');
  window.localStorage.setItem("homeGuid", appInfo.homeGuid || '');
}

const onForgetPassword = () => {
  popupVisible.value = true;
}

const onSendPasswordToEmail = () => {
  if (appInfo.isShowFactorySet) {
    const fs = forgotSelect.value!.instance;
    if (isNullOrEmpty(fs.option("value") as string)) {
      showAlert("please select location", appInfo.title);
      return;
    }
  }

  disableButton.value = true;
  //先判斷加減數字是否正確
  if (randomAnswer.value != answer.value && isCaptcha.value) {
    showAlert(
      "The answer is " +
        randomAnswer.value.toString() +
        ", please try again.",
      appInfo.title
    );
    randomA.value = Math.floor(Math.random() * Math.floor(50));
    randomB.value = Math.floor(Math.random() * Math.floor(50));
    randomAnswer.value = randomA.value + randomB.value;
    disableButton.value = false;
    return;
  }

  const formData: ForgotPasswordFormData = {
    forgotPasswordId: forgotPasswordId.value,
    forgotPasswordEmailOrCellPhone: forgotPasswordEmailOrCellPhone.value
  };
  //成功, else無mail
  apiPost('/api/auth/ForgetPasswordSendMail', formData)
    .then((res) => {
      if (res.status == 200) {
        showAlert('Password has been sent.', appInfo.title);
        popupVisible.value = false;
      } else {
        showAlert('No record, please contact IT.', appInfo.title);
      }
      disableButton.value = false;
    })
    .catch((err) => {
      disableButton.value = false;
      showAlert(err, appInfo.title);
    });
}

// 簡化版登入函數（移除不存在的 mixin 方法）
const logining = async (para: { userId: string; password: string }) => {
  //清除 $ms
  for (const key in $ms) {
    delete $ms[key];
  }

  try {
    const res = await apiPost("/api/auth/login", para);
    
    if (res.status == 200) {
      if (res.data[0].IsIPAllow == "1") {
        // 登入成功
        auth.logIn(res.data[0].Token);
        appInfo.userInfo.userId = res.data[0].UserId;
        appInfo.userInfo.userImageUrl = res.data[0].UserImageUrl;
        appInfo.userInfo.userGuid = res.data[0].UserGuid;
        appInfo.userInfo.userName = res.data[0].UserName;
        appInfo.userInfo.deptGuid = res.data[0].DeptGuid;
        appInfo.userInfo.deptName = res.data[0].DeptName;
        appInfo.userInfo.deptNameAll = res.data[0].DeptNameAll;
        appInfo.userInfo.userTitle = res.data[0].UserTitle;
        appInfo.userInfo.groupNames = res.data[0].GroupNames;
        appInfo.userInfo.groupGuids = res.data[0].GroupGuids;

        const lastLoginDate = res.data[0].LastActiveDate;
        window.localStorage.setItem('userId', para.userId);
        window.localStorage.setItem('password', para.password);

        // TODO: 需要實作以下功能
        // - getCMSConfig()
        // - getCMSLang()
        // - getExchange()
        // - funcAllPreload()
        // 目前暫時跳過這些預載步驟

        loginCount = 0;
        
        // 導向首頁
        if (appInfo.isShowMainMenu) {
          router.push({ path: "/CMSMainMenu" }).catch(() => {});
        } else {
          const redirect = route.query.redirect as string || `/CMSPage/${appInfo.homeGuid}`;
          router.push(redirect).catch(() => {});
        }
      } else {
        showAlert(
          "Login fail:Your IP " +
            res.data[0].UserIP +
            " is prohibited, Please contact IT department.",
          appInfo.title
        );
      }
    } else if (res.status == 204) {
      showAlert(
        "Login fail:Incorrect account or password.",
        appInfo.title
      );
    } else {
      showAlert(String(res.data), appInfo.title);
    }
    
    loginCount++;
    if (loginCount > 5) {
      showAlert("please try it later!!", appInfo.title);
      const b = btnLogin.value?.instance;
      b?.option("disabled", true);
    }
    loading.value = false;
    disableButton.value = false;
  } catch (err) {
    disableButton.value = false;
    loading.value = false;
    showAlert(JSON.stringify(err), "Login");
  }
}


</script>

<style>
.container { max-width: 420px; margin: 40px auto; font-family: system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif; }
.login-card { padding: 20px; }
.title { margin: 0 0 12px; font-weight: 600; }
.field { margin: 12px 0; display: flex; flex-direction: column; }
.error { color: #c00; margin-top: 12px; }
.success { background: #f5f5f5; padding: 12px; border-radius: 8px; margin-top: 12px; }
</style>


