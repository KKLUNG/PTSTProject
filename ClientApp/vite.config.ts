//customize
const title = "Tuntex BI"   //Tuntex EIP //美登 //Juenesse     //this parameter in CMSABMTicket's programName 
const titleLogo = "TUNtex Business Intelligence" //"TUNtex Integration Platform", //TUNtex Integration Platform //美登教室 //Juenesse 
const loginVisionTW = "展人才資本<br />創智慧價值"  //login logo 中文
const loginVisionEN = "Develop human capital<br />and create wisdom value" //login logo 英文
const defaultBrand = "Tuntex" //default brand name 
const defaultFactory = "TW"  //default factory name
const defaultLanguage = "zhTW"  //default language => enUS vi th id zhCN kh
const isShowMainMenu = false //use for show Main tile
const isShowHelp = false  //show help icon on footer => conrdova doesn't work
const isShowLang = true
const isShowBrand = false
const isShowCaptcha = false
const isShowFooter = false  // mobile or cordova show footer.
const isMobileEqualPC = true  //手機 pc 同頁面
const isPasswordChangeAlert = false //first time login, ask to change password
const isWarningOnHeader = false //警語在上方
const appVersion = "22.2.15"
const cacheMode = '1'  //0  no cache, 1 save to memory,  2 save to sessionStorage
const isNativeScrollBar = false;
const preLoadXML = [];
const useTheme = "theme_ticpur"

//這個是為了博聯開發一直在更新, 要避色Jill一直執行到舊的
//const appVersion = "22.2.12"

const outputDir = '../wwwroot'  //for pc build
//const outputDir= '../../../cordova/tuntexeip/www'  //for mac build
const publicPath = ''
//const publicPath = /eip' //for virtual directory


export default {

    pluginOptions: {
        //SYSTEM CONFIG
        title: title,
        titleLogo: titleLogo,
        isMobileEqualPC: isMobileEqualPC,
        loginVisionTW: loginVisionTW,
        loginVisionEN: loginVisionEN,
        isShowMainMenu: isShowMainMenu,
        isShowHelp: isShowHelp,
        isShowLang: isShowLang,
        isShowBrand: isShowBrand,
        isShowCaptcha: isShowCaptcha,
        isShowFooter: isShowFooter,
        isPasswordChangeAlert: isPasswordChangeAlert,
        isWarningOnHeader: isWarningOnHeader,
        appVersion: appVersion,
        //showVideo: showVideo, 
        defaultBrand: defaultBrand,
        defaultFactory: defaultFactory,
        defaultLanguage: defaultLanguage,
        cacheMode: cacheMode,
        isNativeScrollBar: isNativeScrollBar,
        preLoadXML: preLoadXML,
        useTheme: useTheme
    },

    outputDir: outputDir,
    publicPath: publicPath,
    productionSourceMap: false,
    chainWebpack: config => {
        config
            .plugin('html')
            .tap(
                args => {
                    args[0].title = titleLogo;
                    args[0].appVersion = appVersion;
                    return args;
                })

    },
    //https://awdr74100.github.io/2020-04-06-webpack-splitchunksplugin/
    configureWebpack: config => {
        //if (isProduction) {
        config.optimization = {
            runtimeChunk: 'multiple',
            splitChunks: {
                chunks: 'all',
                maxInitialRequests: Infinity,
                minSize: 20000,
                cacheGroups: {
                    vendor: {
                        test: /[\\/]node_modules[\\/]/,
                        name(module) {
                            const packageName = module.context.match(/[\\/]node_modules[\\/](.*?)([\\/]|$)/)[1]
                            return `npm.${packageName.replace('@', '')}`
                        },
                        enforce: true,
                        priority: 10, // 預設為 0，必須大於預設 cacheGroups
                    }
                }
            }
        };
        //}
    },
};
