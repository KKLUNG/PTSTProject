import Vue from 'vue'
import Guid from 'devextreme/core/guid';
import auth from "@/utils/auth";


const isNullOrEmpty = (str: string) => {
    if (str == '' || str == null || str == 'null' | str == 'undefined' || str == undefined)
      return true
    else
      return false;
  },