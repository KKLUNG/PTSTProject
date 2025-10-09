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
import type dxButton from 'devextreme/ui/button';
import type { ValueChangedEvent } from 'devextreme/ui/select_box';

let loginCount = 0;

const { appContext } = getCurrentInstance()!
const appInfo = appContext.config.globalProperties.$appInfo
const cssVariable = appContext.config.globalProperties.$cssVariable
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
//差異:一般字串與 Composition API 的 ref 差異
// const passwordMode = "password";
const passwordMode = ref<'password' | 'text'>('password');
const passwordButton: { icon: string; type: string; onClick: () => void } = {
  icon: 'key',
  type: 'default',
  onClick: () => {
    passwordMode.value = passwordMode.value === 'text' ? 'password' : 'text';
  },
};
const forgotPasswordId = ref('');
const forgotPasswordEmailOrCellPhone = ref('');
const disableButton = ref(false);
const factorySet = ref([]);
const brandSet = ref([]);
const cordovaAppVersion = ref('');
const logoVision = appInfo.defaultLanguage ? appInfo.defaultLanguage === "zhTW" ? appInfo.loginVisionEN : appInfo.loginVisionTW;
const timer = ref(null);
const headerCaptionStyle = ref("color:" + cssVariable.baseFvTextEditorColor )
const btnLogin = ref<{ instance: dxButton } | null>(null)

const onLoginClick = (e:Event) => {
      if (appInfo.isShowFactorySet) {
        if (appContext.config.globalProperties.$isNullOrEmpty(appInfo.factory)) {
          alert(`Please select location - ${appInfo.title}`)
          return;
        }
      }

      if (!vg.validate().isValid) {
        return;
      }

      disableButton.value = true;
      if (randomAnswer.value != answer.value && isCaptcha.value) {
        alert(`The answer is ${randomAnswer.value}, please try again. - ${appInfo.title}`)
        randomA.value = Math.floor(Math.random() * Math.floor(50));
        randomB.value = Math.floor(Math.random() * Math.floor(50));
        randomAnswer.value = randomA.value + randomB.value;

        loginCount++;
        if (loginCount > 5) {
          alert(`Please try it later!! - ${appInfo.title}`);
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
},

const onEnterKey = (e:Event) => {
  const b = btnLogin.value!.instance;
  onLoginClick(e);
  //b.focus();
},

function onSelectedFactory(e: ValueChangedEvent) {
  const selected = e.value as { factory: string; value: string; display: string };
  appInfo.factory = selected.factory;
  appInfo.serverUrl = selected.value;
}

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

const onSendPasswordToEmail = () => {
  if (this.$appInfo.isShowFactorySet) {
        var fs = this.$refs["forgotSelect"].instance;
        if (this.isNullOrEmpty(fs.option("value"))) {
          this.alert("please select location", this.$appInfo.title);
          return;
        }
      }

      var that = this;
      this.disableButton = true;
      //先判斷加減數字是否正確
      if (this.randomAnswer != this.answer && this.isCaptcha) {
        this.alert(
          "The answer is " +
            this.randomAnswer.toString() +
            ", please try again.",
          this.$appInfo.title
        );
        this.randomA = Math.floor(Math.random() * Math.floor(50));
        this.randomB = Math.floor(Math.random() * Math.floor(50));
        this.randomAnswer = this.randomA + this.randomB;
        this.disableButton = false;
        return;
      }

      var formData = {};
      //成功, else無mail
      formData["forgotPasswordId"] = this.forgotPasswordId;
      formData["forgotPasswordEmailOrCellPhone"] =
        this.forgotPasswordEmailOrCellPhone;
      this.apiPost("/api/auth/ForgetPasswordSendMail", formData)
        .then((res) => {
          if (res.status == 200) {
            that.alert("Password has been sent.", that.$appInfo.title);
            that.popupVisible = false;
          } else {
            that.alert("No record, please contact IT.", that.$appInfo.title);
          }
          that.disableButton = false;
        })
        .catch((err) => {
          that.disableButton = false;
          that.alert(err, that.$appInfo.title);
        });
}

const logining = async (para) => {
      //2021.12.29 清除 $ms
      //this.$ms = {};
      for (var key in this.$ms) {
        delete this.$ms[key];
      }

      //2023.08.25 必須用上面的方法清空
      //Object.assign(this.$ms, {});
      //console.log(this.$ms)

      var that = this;
      await this.apiPost("/api/auth/login", para)
        .then(async (res) => {
          if (res.status == 200) {
            if (res.data[0].IsIPAllow == "1") {
              //#region loginSuccess
              auth.logIn(res.data[0].Token);
              that.$appInfo.userInfo.userId = res.data[0].UserId;
              that.$appInfo.userInfo.userImageUrl = res.data[0].UserImageUrl;
              that.$appInfo.userInfo.userGuid = res.data[0].UserGuid;
              that.$appInfo.userInfo.userName = res.data[0].UserName;
              that.$appInfo.userInfo.deptGuid = res.data[0].DeptGuid;
              that.$appInfo.userInfo.deptName = res.data[0].DeptName;
              that.$appInfo.userInfo.deptNameAll = res.data[0].DeptNameAll;
              that.$appInfo.userInfo.userTitle = res.data[0].UserTitle;
              that.$appInfo.userInfo.groupNames = res.data[0].GroupNames;
              that.$appInfo.userInfo.groupGuids = res.data[0].GroupGuids;

              let lastLoginDate = res.data[0].LastActiveDate;
              window.localStorage.userId = para.userId;
              window.localStorage.password = para.password;

              //preload
              /* if(that.$appInfo.preLoadXML.length > 0) {
                let xmlNames = that.$appInfo.preLoadXML.toString();                
                  let rtnObj = await that.funcAllPreload(xmlNames).then(res =>{
                    return res;
                  });
                  
                for(var i = 0; i < that.$appInfo.preLoadXML.length; i++) {
                  if (that.isNullOrEmpty(that.getItem(that.$appInfo.preLoadXML[i] + "_All")))
                    await that.setItem(that.$appInfo.preLoadXML[i] + "_All", rtnObj.data[that.$appInfo.preLoadXML[i]]);
                } 
                   
              } */

              await Promise.all([
                that.getCMSConfig(),
                that.getCMSLang(),
                that.getExchange(),
                that.funcAllPreload(that.$appInfo.preLoadXML.toString()),
              ]).then(async (res1) => {
                window.sessionStorage.setItem(
                  "systemConfig",
                  JSON.stringify(res1[0].data)
                );

                window.sessionStorage.setItem(
                  "dictionary",
                  JSON.stringify(res1[1].data)
                );
                window.sessionStorage.setItem(
                  "exchangeRate",
                  JSON.stringify(res1[2].data)
                );

                if (that.$appInfo.isCordova) {
                  //更新deviceToken to server
                  if (
                    !that.isNullOrEmpty(that.$appInfo.userInfo.userDeviceToken)
                  ) {
                    var ary = [];
                    ary.push(that.$appInfo.userInfo.userGuid);
                    ary.push(that.$appInfo.userInfo.userDeviceType);
                    ary.push(that.$appInfo.userInfo.userDeviceToken);
                    ary.push(that.$appInfo.userInfo.userUUID);
                    that.ExecuteCMSCommand(
                      "UpdateCMSUserMobile.xml",
                      ary.toString(),
                      true
                    );
                  }
                }
                //因應品牌功能 在這裡設定 homeguid
                if (this.$appInfo.isShowBrand) {
                  this.$appInfo.rootGuid = this.brandSet.filter(
                    (x) => x.value == this.$appInfo.brand
                  )[0].rootGuid;
                  this.$appInfo.homeGuid = this.brandSet.filter(
                    (x) => x.value == this.$appInfo.brand
                  )[0].homeGuid;
                }

                //設定工廠,,如果cmsconfig有值的話, 就用, 否則使用vue.config 預設值
                if (!this.isNullOrEmpty(this.getSystemConfig("Factory"))) {
                  this.$appInfo.factory = this.getSystemConfig("Factory");
                }

                if (that.$appInfo.cacheMode != "0") {
                  for (var key in res1[3].data) {
                    if (that.isNullOrEmpty(that.getItem(key + "_All")))
                      await that.setItem(key + "_All", res1[3].data[key]);
                  }
                }
                //2023.07.10 get BiaVersion
                //var currentVer = res1[0].data.filter(x => x.ConfigName == 'BiaVersion');
                var currentVer = this.getSystemConfig("BiaVersion");
                if (
                  !this.$appInfo.isCordova &&
                  !this.isNullOrEmpty(currentVer) &&
                  this.$appInfo.appVersion != currentVer
                ) {
                  that
                    .alertThen(
                      this.Message("sys_versionChange"),
                      that.$appInfo.title
                    )
                    .then(() => {
                      auth.logOut();
                      window.location.reload();
                    });
                } else {
                  //第一次進平台,,強制轉到變更密碼那一頁
                  if (
                    that.isNullOrEmpty(lastLoginDate) &&
                    that.$appInfo.isPasswordChangeAlert
                  ) {
                    that
                      .alertThen(
                        "Please change your password.",
                        that.$appInfo.title
                      )
                      .then(() => {
                        var url = "";
                        if (that.isLargeScreen)
                          url =
                            "/CMSFrame/AdminFormView/3/Form_CMSUser_ChangePWD.xml";
                        else
                          url =
                            "/CMSFrame/" +
                            that.Message("myInfo") +
                            "/AdminFormView/3/Form_CMSUser_ChangePWD.xml";

                        that.$router.push(url).then(() => {
                          if (that.$appInfo.isCordova) {
                            setTimeout(() => {
                              window.navigator.splashscreen.hide();
                            }, 2000);
                          } //這裡用cache 會有錯,
                        });
                      });
                  } else {
                    //2023.11.12 modify by Allen
                    if (that.$appInfo.isShowMainMenu) {
                      this.$router
                        .push({
                          path: "/CMSMainMenu",
                        })
                        .catch((err) => {});
                    } else {
                      var redirect = that.$route.query.redirect;
                      if (that.isNullOrEmpty(redirect))
                        redirect = "/CMSPage/" + that.$appInfo.homeGuid;

                      that.$router.push(redirect).then(() => {
                        if (!that.$appInfo.isMobile) {
                          if (
                            window.navigator.onLine &&
                            !this.isNullOrEmpty(
                              this.getSystemConfig("Greeting")
                            )
                          ) {
                            this.GetCMSCommand(
                              "GETDaySentence.xml",
                              "",
                              false
                            ).then((res) => {
                              var text =
                                res.data[0].MsgSubject +
                                " 出自 " +
                                res.data[0].MsgContent;
                              that.speech(
                                this.$appInfo.userInfo.userName.substr(1, 3) +
                                  "您好," +
                                  text
                              );
                              that.alertThen(text, "每日一語").then(() => {
                                that.$speechBot.cancel();
                              });
                            });

                            /* var request = new XMLHttpRequest();
                              request.open("GET", "https://v1.hitokoto.cn/?c=d&c=e&c=i&c=k", false);
                              request.send(null);
                              var o = JSON.parse(request.responseText);
                              var author = o.from_who
                              if (author == null)
                                author = '不詳'

                              var from = o.from
                              if (from == null)
                                from = '不詳'
                              var text = o.hitokoto + " 作者 " + author + ",出自 " + from
                              text = that.Traditionalized(text)
                              that.speech(this.$appInfo.userInfo.userName.substr(1, 3) + "您好," + text);
                              that.alertThen(text, '每日一語').then(()=>{
                                that.$speechBot.cancel()
                              }); */
                          }
                        }
                        if (that.$appInfo.isCordova) {
                          setTimeout(() => {
                            window.navigator.splashscreen.hide();
                          }, 2000);
                        } //這裡用cache 會有錯,
                      });
                    }
                  }
                }
              });

              loginCount = 0;
              //#endregion
            } else {
              that.alert(
                "Login fail:Your IP " +
                  res.data[0].UserIP +
                  " is prohibited, Please contact IT department.",
                that.$appInfo.title
              );
            }
          } else if (res.status == 204) {
            that.alert(
              "Login fail:Incorrect account or password.",
              that.$appInfo.title
            );
          } else {
            that.alert(res.data, that.$appInfo.title);
          }
          loginCount++;
          if (loginCount > 5) {
            that.alert("please try it later!!", that.$appInfo.title);
            let b = that.$refs["btnLogin"].instance;
            b.option("disabled", true);
          }
          that.loading = false;
          that.disableButton = false;
        })
        .catch((err) => {
          that.disableButton = false;
          that.loading = false;
          that.alert(JSON.stringify(err), "Login");
        });
    },


</script>

<style>
.container { max-width: 420px; margin: 40px auto; font-family: system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif; }
.login-card { padding: 20px; }
.title { margin: 0 0 12px; font-weight: 600; }
.field { margin: 12px 0; display: flex; flex-direction: column; }
.error { color: #c00; margin-top: 12px; }
.success { background: #f5f5f5; padding: 12px; border-radius: 8px; margin-top: 12px; }
</style>


