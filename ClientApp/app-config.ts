const title = "Tuntex BI"   //Tuntex EIP //美登 //Juenesse     //this parameter in CMSABMTicket's programName 
const titleLogo = "測試專案" //"TUNtex Integration Platform", //TUNtex Integration Platform //美登教室 //Juenesse 
const loginVisionTW = "貴龍管理系統"  //login logo 中文
const loginVisionEN = "Develop human capital<br />and create wisdom value" //login logo 英文
const defaultBrand = "Tuntex" //default brand name 
const defaultFactory = "TW"  //default factory name
const defaultLanguage = "zhTW"  //default language => enUS vi th id zhCN kh
const isShowMainMenu = false //use for show Main tile
const isShowHelp = false  //show help icon on footer => conrdova doesn't work
const isShowLang = true
const isShowBrand = false
const isShowCaptcha = true
const isShowFooter = false  // mobile or cordova show footer.
const isMobileEqualPC = true  //手機 pc 同頁面
const isPasswordChangeAlert = false //first time login, ask to change password
const isWarningOnHeader = false //警語在上方
const appVersion = "22.2.15"
const cacheMode = '1'  //0  no cache, 1 save to memory,  2 save to sessionStorage
const isNativeScrollBar = false
const preLoadXML: any[] = []
const useTheme = "theme_ticpur"

const outputDir = "../wwwroot"  //for pc build
const publicPath = "" //for virtual directory

export const pluginOptions = {
  title,
  titleLogo,
  isMobileEqualPC,
  loginVisionTW,
  loginVisionEN,
  isShowMainMenu,
  isShowHelp,
  isShowLang,
  isShowBrand,
  isShowCaptcha,
  isShowFooter,
  isPasswordChangeAlert,
  isWarningOnHeader,
  appVersion,
  defaultBrand,
  defaultFactory,
  defaultLanguage,
  cacheMode,
  isNativeScrollBar,
  preLoadXML,
  useTheme,
}

const appConfig = {
  pluginOptions,
  publicPath,
  outputDir,
}

export default appConfig

