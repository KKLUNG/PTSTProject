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
    <RootPopup
      ref="rootPopup"
      :visible="popupVisible"
      :drag-enabled="false"
      :hide-on-outside-click="true"
      :show-title="true"
      :width="400"
      :height="450"
      titleTemplate="title"
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
    </RootPopup>
  </div>
</template>

<script setup lang="ts">
import axios from 'axios'
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import DxButton from 'devextreme-vue/button'
import auth from '../utils/auth'
import DxTextBox from "devextreme-vue/text-box";
import DxNumberBox from "devextreme-vue/number-box";
import { DxButton as DxTextBoxButton } from "devextreme-vue/text-box";
import DxValidationGroup from "devextreme-vue/validation-group";
import DxValidator, { DxRequiredRule } from "devextreme-vue/validator";
import DxSelectBox from "devextreme-vue/select-box";
import { getCurrentInstance } from 'vue'

let loginCount = 0;

const { appContext } = getCurrentInstance()!
const appInfo = appContext.config.globalProperties.appInfo
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
const answer = ref(null);
const isCaptcha = ref(false);
const isLargeScreen = window.innerWidth > 960 ? true : false;
const passwordMode = "password";
const passwordButton = ref({
  icon: "key",
  type: "default",
  onClick: () => {
    passwordMode = passwordMode.value === "text" ? "password" : "text";
  }
});
const forgotPasswordId = ref('');
const forgotPasswordEmailOrCellPhone = ref('');
const disableButton = ref(false);
const factorySet = ref([]);
const brandSet = ref([]);
const cordovaAppVersion = ref('');
const logoVision = appInfo.defaultLanguage ? appInfo.defaultLanguage === "zhTW" ? appInfo.loginVisionEN : appInfo.loginVisionTW;
const timer = ref(null);
const headerCaptionStyle = ref("color:" + this $cssVariableValue.baseFvTextEditorColor )


const onLoginClick = (e:Event) => {
      if (appInfo.isShowFactorySet) {
        if (isNullOrEmpty(appInfo.factory)) {
          alert("Please select location", appInfo.title);
          return;
        }
      }

      if (!vg.validate().isValid) {
        return;
      }

      disableButton.value = true;
      if (randomAnswer.value != answer.value && isCaptcha.value) {
        alert(
          "The answer is " +
            randomAnswer.value.toString() +
            ", please try again.",
          appInfo.title
        );
        randomA.value = Math.floor(Math.random() * Math.floor(50));
        randomB.value = Math.floor(Math.random() * Math.floor(50));
        randomAnswer.value = randomA.value + randomB.value;

        loginCount.value++;
        if (loginCount.value > 5) {
          alert("Please try it later!!", appInfo.title);
          var b = $refs["btnLogin"].instance;
          b.option("disabled", true);
        }
        disableButton.value = false;
        return;
      }

      loading.value = true;
      var para = {
        userId: login.value,
        password: password.value,
      };
      logining(para);
},

const onEnterKey = (e:Event) => {
  var b = $refs["btnLogin"].instance;
  onLoginClick(e);
  //b.focus();
},

const onSelectedFactory = (e:Event) => {
  appInfo.factory = e.value.factory;
  appInfo.serveUrl = e.value.serveUrl;
  setDefault();
},

const setDefault = () => {
  if (appInfo.isCordova) {
        if (appInfo.isShowFactorySet) {
          if (!isNullOrEmpty(appInfo.factory)) {
            //this.appInfo.serverUrl = this.appInfo.factory;
            var o = factorySet.value.filter(
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
      window.localStorage.setItem("rootGuid", appInfo.rootGuid);
      window.localStorage.setItem("homeGuid", appInfo.homeGuid);
}

const onForgetPassword = () => {
      popupVisible.value = true;
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


