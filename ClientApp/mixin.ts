import { apiPost } from "@/utils/api-util";

const isNullOrEmpty = (str: string) => {
    if (str == '' || str == null || str == 'null' || str == 'undefined' || str == undefined)
      return true;
    else
      return false;
  };

  declare module 'vue/types/vue' {
    interface Vue {
      apiPost: typeof apiPost;
    }
  }
  
  Vue.prototype.apiPost = apiPost;