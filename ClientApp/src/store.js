import { createStore } from 'vuex'
import { apiPost, apiGet } from '@/utils/api-util'
import appInfo from '@/utils/app-Info'

export default createStore({
  state: {
    menus: [],
    icons: [],
    dictionary: [],
    badge: 0,
  },
  mutations: {
    createMenus(state, data) {
      state.menus = data;
    },
    createIcons(state, data) {
      state.icons = data;
    },
    createDictionary(state, data) {
      state.dictionary = data;
    },
    createBadge(state, data) {
      state.badge = data[0].Badge;
    }

  },
  actions: {
    ZZZCREATE_MENUS: (context, para) => {  //para = $appInfo
      var language = appInfo.language;

       apiGet("/api/cms/GetCMSMenuByUserId",
          {
            KeyParameter: para.rootGuid,
            UserGuid: para.userInfo.userGuid,
            Language: language,
            IgnoreACL: "0",
            DisplayMode: para.isMobile ? "2" : "1",
          })
          .then((res) => {
            if (res.status == 200) {
              context.commit("createMenus", res.data)
            }
          })
          .catch((err) => {
            console.log('store_menus:' + err);
          });
      
    },

    CREATE_MENUS: async (context, para) => {  //para = $appInfo
      var language = appInfo.language;

       
      return await new Promise(function (resolve, reject) {
        apiGet("/api/cms/GetCMSMenuByUserId",
          {
            KeyParameter: para.rootGuid,
            UserGuid: para.userInfo.userGuid,
            Language: language,
            IgnoreACL: "0",
            DisplayMode: para.isMobile ? "2" : "1",
          })
          .then((res) => {
            if (res.status == 200) {
              context.commit("createMenus", res.data)
              resolve();
            }
          })
          .catch((err) => {
            console.log('store_menus:' + err);
          });
      })
    },

    CREATE_ICONS: (context, para) => {
      var bia_storeIcon = window.sessionStorage.getItem("bia_storeIcon")
      if(bia_storeIcon == null || bia_storeIcon == undefined) {

      apiPost("/api/cms/GetCMSCommand", { CommandName: 'GetRS_CMSCodes.xml', KeyParameter: 'TB', UserGuid: para.userInfo.userGuid })
        .then((res) => {
          if (res.status == 200) {
            context.commit("createIcons", res.data)
            window.sessionStorage.setItem("bia_storeIcon", JSON.stringify(res.data))
          }
        })
        .catch((err) => {
          console.log('store_icon:' + err);
        });
      } else {
        context.commit("createIcons", JSON.parse(bia_storeIcon))
      }

    },

    /* CREATE_DICTIONARY:(context, para) => {
      apiPost("/api/cms/GetCMSLang", { LangType:'',  UserGuid: '00000000-0000-0000-0000-000000000000'  })
        .then((res) => {
          if(res.status == 200) context.commit("createDictionary", res.data)
        })
        .catch((err) => {
          alert(JSON.stringify(err))
          alert(err);
        });
    }, */

    CREATE_BADGE: (context, para) => {
      apiGet("/api/Func/GetBadge", para)
        .then((res) => {
          if (res.status == 200) context.commit("createBadge", res.data)
        })
        .catch((err) => {
          console.log('store:_badge' + err);
        });
    },

  }
})