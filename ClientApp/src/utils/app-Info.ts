import Device from "devextreme/core/devices";
import appConfig from "../../app-config"
import { langSet } from "./constData"
export default {
  title: appConfig.pluginOptions.title,   //Tuntex EIP //美登 //Juenesse     //this parameter in CMSABMTicket's programName 
  titleLogo: appConfig.pluginOptions.titleLogo, //"TUNtex Integration Platform", //TUNtex Integration Platform //美登教室 //Juenesse 
  loginVisionTW: appConfig.pluginOptions.loginVisionTW,
  loginVisionEN: appConfig.pluginOptions.loginVisionEN,
  isShowHelp: appConfig.pluginOptions.isShowHelp,  //show help icon on footer
  isShowLang: appConfig.pluginOptions.isShowLang,  // show language selection on right top
  isShowBrand: appConfig.pluginOptions.isShowBrand,
  _isShowCaptcha: appConfig.pluginOptions.isShowCaptcha,
  appVersion: appConfig.pluginOptions.appVersion,  // appversion
  //showVideo: appConfig.pluginOptions.showVideo,
  isMobileEqualPC: appConfig.pluginOptions.isMobileEqualPC,
  defaultBrand: appConfig.pluginOptions.defaultBrand,
  defaultFactory: appConfig.pluginOptions.defaultFactory,
  isShowFooter: appConfig.pluginOptions.isShowFooter,
  isWarningOnHeader: appConfig.pluginOptions.isWarningOnHeader,
  defaultLanguage: appConfig.pluginOptions.defaultLanguage,
  isPasswordChangeAlert: appConfig.pluginOptions.isPasswordChangeAlert, // control with first time login and display forgot password button.
  virtualDirectory: appConfig.publicPath,
  isShowMainMenu: appConfig.pluginOptions.isShowMainMenu,
  cacheMode: appConfig.pluginOptions.cacheMode,
  isNativeScrollBar: appConfig.pluginOptions.isNativeScrollBar,
  preLoadXML: appConfig.pluginOptions.preLoadXML,
  useTheme:appConfig.pluginOptions.useTheme,
  

  notifyDelay: 1500,
  notifyDelayByFlow: 3000,
  NotificationSenderID: '144333570078', //android 專案代號 推播用
  dictionary: [],



  get originalMobileRoot() {
    return this.isMobileEqualPC ? this.emptyGuid : '1313966A-C773-4CA1-C946-4EB9B7D043DE'
  },

  get isDebug() {
    //return false
    return this.userInfo.userId.toLowerCase() == 'Administrator'.toLowerCase() ? true : ((process.env.NODE_ENV == 'production') ? false : true);
  },

  get showVideo() {
    if (this.isCordova)
      return false;
    else {
      var http = new XMLHttpRequest();
      http.open('HEAD', "bg.mp4", false);
      http.send();
      //console.log(http.status)
      return http.status != 404;
    }
  },

  get isShowFactorySet() { return this.isCordova ? true : false; },  //with cordova only

  //2024.4.22 since cordova-android10+ and cordova11.0.0  do not use protocol == files to check isCordova, because it changes to https://localhost
  //ios still use files
  get isCordova() { return ((window.location.protocol + "//" + window.location.host) == 'https://localhost' || window.location.protocol == 'file:') ? true : false; },

  get isMobile() {
    if (this.isCordova)
      return true
    else
      return Device.current().deviceType == 'desktop' ? false : true;
  },

  get emptyGuid() { return '00000000-0000-0000-0000-000000000000'; },

  get homeGuid() {
    if (window.sessionStorage.getItem('_homeGuid') == null)
      window.sessionStorage.setItem('_homeGuid', (this.isCordova) ? this.mobileHome : this.pcHome)
    return window.sessionStorage.getItem('_homeGuid');
  }, //首頁
  set homeGuid(value) {
    //TypeScript 在 strictNullChecks 下，sessionStorage.getItem() 回傳型別是 string | null，
    window.sessionStorage.setItem("_homeGuid", value ?? '');
  },
  get rootGuid() {

    if (window.sessionStorage.getItem('_rootGuid') == null)
      window.sessionStorage.setItem('_rootGuid', (this.isCordova) ? this.mobileRoot : this.pcRoot)
    return window.sessionStorage.getItem('_rootGuid');

  },  //homeGuid's parent , 撈選單用
  set rootGuid(value) {
    window.sessionStorage.setItem("_rootGuid", String(value));
  },

  get factory() {
    if (window.localStorage.getItem('_factory') == null)
      window.localStorage.setItem('_factory', (this.isCordova) ? String(null) : this.defaultFactory)
    return window.localStorage.getItem('_factory');
  },
  set factory(value) {
    window.localStorage.setItem("_factory", String(value));
  },

  get brand() {
    if (window.localStorage.getItem('_brand') == null)
      window.localStorage.setItem('_brand', this.defaultBrand)
    return window.localStorage.getItem('_brand');
  },
  set brand(value) {
    window.localStorage.setItem("_brand", String(value));
  },

  get language() {
    if (window.localStorage.getItem('_language') == null) {
      var browserLanguage = navigator.language.replace("-", "");
      var oo = langSet.filter((x) => x.value == browserLanguage);
      if (oo.length != 0) window.localStorage.setItem('_language', browserLanguage);
      else window.localStorage.setItem('_language', this.defaultLanguage)
    }
    return window.localStorage.getItem('_language');
  },
  set language(value) {
    window.localStorage.setItem("_language", String(value));
  },

  get BPMListGuid() {
    if (this.originalMobileRoot == this.emptyGuid)
      return '7CA3D384-5B88-4620-9C9E-02A1076B9783';
    else
      return 'CCA0F833-85E9-4F0F-B278-1496021D6858';
  }, //app 簽核返回的清單

  //2022.01.12 若mobileRoot = 00000 表示pc, mobile 同一個rootGuid, 


  get mobileHome() {
    if (this.originalMobileRoot == this.emptyGuid)
      return 'BBC03CF9-B5A1-4962-AD21-650FFD271E32';
    else
      return '9A47DF98-2D06-4565-A9D1-933F91721CCD';
  },
  get mobileRoot() {
    if (this.originalMobileRoot == this.emptyGuid)
      if (this.isCordova)
        return this.pcRoot;
      else
        return this.emptyGuid;
    else
      return this.originalMobileRoot;
  },
  get pcHome() { return 'BBC03CF9-B5A1-4962-AD21-650FFD271E32'; },
  get pcRoot() { return '591CCB3D-62CE-47D0-87B3-B73C6BE66DD7'; },

  //isShowMobileSwitch: true,  //show mobile/pc switch button
  //這個要改成由cmsconfig 決定
  get isShowMobileSwitch() {
    if (this.isCordova)
      return false;
    else {
      return (this.isMobileEqualPC) ? false : true;
    }
  },
  //isShowCaptcha: true,  //show captcha
  get isShowCaptcha() {
    if (this.isCordova || !this._isShowCaptcha)
      return false;
    else
      return true;
  },

  //for cordova , set the server current url path
  get serverUrl() {
    if (window.sessionStorage.getItem('_serverUrl') == null)
      window.sessionStorage.setItem('_serverUrl', 'https://eip.tun.com.tw')  //default site here //https://eip.tun.com.tw  // http://60.251.69.11/

    //return 'https://nas.tun.com.tw/ticpur'
    //return 'https://eip.tun.com.tw'  // mac debug use
    return (this.isCordova) ? window.sessionStorage.getItem('_serverUrl') : (process.env.NODE_ENV == 'production') ? window.location.protocol + "//" + window.location.host + window.location.pathname.substring(0, window.location.pathname.length - 1) : 'https://localhost:44358' 
  },
  set serverUrl(value) {
    window.sessionStorage.setItem('_serverUrl', String(value))
  },

  userInfo: {
    get userImageUrl() {
      return window.sessionStorage.getItem('_userImageUrl');
    },
    set userImageUrl(value) {
      window.sessionStorage.setItem('_userImageUrl', String(value))
    },
    get userId() {
      return window.sessionStorage.getItem('_userId') ?? '';
    },
    set userId(value) {
      window.sessionStorage.setItem('_userId', String(value))
    },
    get userTitle() {
      return window.sessionStorage.getItem('_userTitle');
    },
    set userTitle(value) {
      window.sessionStorage.setItem('_userTitle', String(value))
    },
    get userName() {
      return window.sessionStorage.getItem('_userName');
    },
    set userName(value) {
      window.sessionStorage.setItem('_userName', String(value))
    },
    get userGuid() {
      return window.sessionStorage.getItem('_userGuid');
    },
    set userGuid(value) {
      window.sessionStorage.setItem('_userGuid', String(value))
    },

    get userDeviceToken() {
      return window.sessionStorage.getItem('_userDeviceToken');
    },
    set userDeviceToken(value) {
      window.sessionStorage.setItem('_userDeviceToken', String(value));
    },

    get userDeviceType() {
      return window.sessionStorage.getItem('_userDeviceType');
    },
    set userDeviceType(value) {
      window.sessionStorage.setItem('_userDeviceType', String(value));
    },

    get userUUID() {
      return window.sessionStorage.getItem('_userUUID');
    },
    set userUUID(value) {
      window.sessionStorage.setItem('_userUUID', String(value));
    },

    get deptGuid() {
      return window.sessionStorage.getItem('_deptGuid');
    },
    set deptGuid(value) {
      window.sessionStorage.setItem('_deptGuid', String(value))     
    },

    get deptName() {
      return window.sessionStorage.getItem('_deptName');
    },
    set deptName(value) {
      window.sessionStorage.setItem('_deptName', String(value))
    },

    get deptNameAll() {
      return window.sessionStorage.getItem('_deptNameAll');
    },
    set deptNameAll(value) {
      window.sessionStorage.setItem('_deptNameAll', String(value))
    },

    get groupNames() {
      return window.sessionStorage.getItem('_groupNames');
    },
    set groupNames(value) {
      window.sessionStorage.setItem('_groupNames', String(value))
    },

    get groupGuids() {
      return window.sessionStorage.getItem('_groupGuids');
    },
    set groupGuids(value) {
      window.sessionStorage.setItem('_groupGuids', String(value))
    },
 
  },
  formButtonStylingMode: 'text'

};