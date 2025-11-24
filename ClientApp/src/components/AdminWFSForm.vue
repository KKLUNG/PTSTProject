<template>
  <div v-if="isVisible">
    <DxLoadPanel
      :hide-on-outside-click="false"
      :visible="loading"
      :shading="true"
      :show-pane="true"
      shading-color="transparent"
      :message="loadingMessage"
    />

    <!-- caption -->
    <div
      :class="appInfo.isMobile ? 'webPartHeaderTransparent' : 'webPartHeader'"
    >
      <div
        class="webPartCaption"
        v-show="
          oHead.wfStatusFlag != 'B' &&
          oHead.wfStatusFlag != 'N' &&
          oHead.wfStatusFlag != 'R'
        "
      >
        <div><i :class="toolbarClass" /></div>
        <div>{{ Message('sys_AdminWFSForm_Caption1') }}</div>
      </div>
    </div>
    <hr />

    <!-- 表單修改 -->
    <div class="responsive-paddings1" v-if="form_hasEdit">
      <div style="flex-direction: column">
        <div class="lableStyle" :style="{ width: lblWidth }">
          <div class="labelAlign">
            {{ Message('sys_AdminWFSForm_Caption2') }}
          </div>
        </div>
        <div class="labelLine"></div>
      </div>

      <div>
        <AdminFormView
          v-if="form_hasEdit"
          :XMLName="form_XMLName"
          :isQueryPanel="false"
          :showHeader="false"
          :isInFlowEditing="true"
          FormMode="E"
          :XMLParameter="wfRefGuid"
          :wfSNMsgGuid="wfSNMsgGuid"
        />
      </div>
    </div>

    <div class="responsive-paddings1">
      <div
        style="flex-direction: column"
        v-show="
          oHead.wfStatusFlag == 'D' ||
          oHead.wfStatusFlag == 'O' ||
          oHead.wfRejectToApplier == '1'
            ? true
            : oHead.wfNoOpinion != '1' && oHead.wfHideSignOpinion != '1'
        "
      >
        <div class="lableStyle" :style="{ width: lblWidth }">
          <div class="labelAlign">
            {{ Message('sys_AdminWFSForm_Caption4') }}
          </div>
        </div>
        <div class="labelLine"></div>
      </div>

      <!-- 加簽、知會-->
      <div
        v-show="
          oHead.wfStatusFlag != 'B' &&
          oHead.wfStatusFlag != 'E' &&
          oHead.wfStatusFlag != 'N' &&
          oHead.wfStatusFlag != 'T' &&
          oHead.wfStatusFlag != 'R' &&
          oHead.wfStatusFlag != 'V' &&
          oHead.wfSNIsEnd != 'Y'
        "
      >
        <!-- TODO: 需要實作 POG 控件 -->
        <div
          class="responsive-paddings1"
          style="padding: 8px"
          v-if="oHead.wfAllowSign == '1' && oHead.wfStatusFlag != 'D'"
        >
          <div>
            <p>加簽功能 (POG 控件尚未實作)</p>
            <p>Endorse To: {{ endoreUserList.join(', ') }}</p>
          </div>
        </div>

        <div
          class="responsive-paddings1"
          style="padding: 8px"
          v-if="oHead.wfAllowNotify == '1'"
        >
          <div>
            <p>知會功能 (POG 控件尚未實作)</p>
            <p>Notify To: {{ notifyUserList.join(', ') }}</p>
          </div>
        </div>

        <hr />
      </div>

      <!-- 意見-->
      <div
        v-show="
          oHead.wfStatusFlag != 'B' &&
          oHead.wfStatusFlag != 'N' &&
          oHead.wfStatusFlag != 'R'
        "
      >
        <DxTextArea
          v-show="
            oHead.wfStatusFlag == 'D' ||
            oHead.wfStatusFlag == 'O' ||
            oHead.wfRejectToApplier == '1'
              ? true
              : oHead.wfNoOpinion != '1' && oHead.wfHideSignOpinion != '1'
          "
          :height="150"
          v-model:value="wfOpinion"
          :placeholder="Message('ph_inputApprovalOpinion')"
        />
      </div>

      <!-- 附件 -->
      <div
        style="flex-direction: column"
        v-if="oHead.wfAllowAttachment == '1'"
      >
        <br />
        <div class="lableStyle" :style="{ width: lblWidth }">
          <div class="labelAlign">
            {{ Message('sys_AdminWFSForm_Caption3') }}
          </div>
        </div>
        <div class="labelLine"></div>
      </div>
      <AdminGridForm
        v-if="oHead.wfAllowAttachment == '1'"
        XMLName="Code_WFSSNMsgFile.xml"
        :XMLParameter="wfSNMsgGuid"
        :showHeader="false"
      />
      <hr v-if="oHead.wfAllowAttachment == '1'" />

      <!-- 按鈕-->
      <div class="toolbarArea">
        <div></div>
        <div
          class="formButton"
          v-show="
            oHead.wfStatusFlag != 'B' &&
            oHead.wfStatusFlag != 'N' &&
            oHead.wfStatusFlag != 'R'
          "
        >
          <!-- 儲存按鈕 -->
          <div
            v-show="
              oHead.wfStatusFlag == 'O' ||
              (oHead.wfAllowEdit == '1' && oHead.wfStatusFlag != 'D')
            "
          >
            <DxButton
              :text="Message('sys_tempSave')"
              :hint="Message('sys_tempSave')"
              type="default"
              icon="save"
              :disabled="approveButtonDisabled"
              :styling-mode="appInfo.formButtonStylingMode || 'contained'"
              @click="onFlowEditSave"
            />
          </div>

          <!-- 核決按鈕 -->
          <div
            v-show="
              oHead.wfAllowReview == '1' &&
              oHead.wfStatusFlag != 'D' &&
              oHead.wfStatusFlag != 'O' &&
              oHead.EndoreMustWait == '0' &&
              oHead.ApproverTrans == '0'
            "
          >
            <DxButton
              :text="Message('sys_approve')"
              :hint="Message('sys_approve')"
              type="default"
              icon="todo"
              :disabled="approveButtonDisabled"
              :styling-mode="appInfo.formButtonStylingMode || 'contained'"
              @click="onFlowApprove"
            />
          </div>

          <!-- 核轉按鈕 -->
          <div
            v-show="
              oHead.wfAllowTransReview == '1' &&
              oHead.wfAllowReview == '1' &&
              oHead.EndoreMustWait == '0' &&
              oHead.wfStatusFlag != 'D' &&
              oHead.wfStatusFlag != 'O'
            "
          >
            <DxButton
              :text="Message('sys_approveTransfer')"
              :hint="Message('sys_approveTransfer')"
              type="default"
              icon="todo"
              :disabled="approveButtonDisabled"
              :styling-mode="appInfo.formButtonStylingMode || 'contained'"
              @click="onFlowChangeApprover"
            />
          </div>

          <!-- 加簽 退簽核者修改 送件  -->
          <div
            v-show="
              oHead.wfStatusFlag == 'D' || oHead.wfStatusFlag == 'O'
                ? true
                : oHead.EndoreMustWait == '0'
                ? oHead.wfAllowReview != '1'
                : false
            "
          >
            <DxButton
              :text="Message('sys_agree')"
              :hint="Message('sys_agree')"
              type="default"
              icon="todo"
              :disabled="approveButtonDisabled"
              :styling-mode="appInfo.formButtonStylingMode || 'contained'"
              @click="onFlowApprove"
            />
          </div>

          <!-- 退件 -->
          <div
            v-show="
              oHead.wfAllowReject == '1' &&
              oHead.EndoreMustWait == '0' &&
              oHead.wfStatusFlag != 'D' &&
              oHead.wfStatusFlag != 'O'
            "
          >
            <DxButton
              :text="Message('sys_disagree')"
              :hint="Message('sys_disagree')"
              type="default"
              icon="undo"
              :disabled="rejectButtonDisabled"
              :styling-mode="appInfo.formButtonStylingMode || 'contained'"
              @click="onFlowReject"
            />
          </div>

          <!-- 退上一關 -->
          <div
            v-show="
              oHead.wfRejectType == '1' &&
              oHead.EndoreMustWait == '0' &&
              oHead.wfStatusFlag != 'D' &&
              oHead.wfStatusFlag != 'O'
            "
          >
            <DxButton
              :text="Message('sys_disagreePrevious')"
              :hint="Message('sys_disagreePrevious')"
              type="default"
              icon="undo"
              :disabled="rejectButtonDisabled"
              :styling-mode="appInfo.formButtonStylingMode || 'contained'"
              @click="onFlowRejectPrevious"
            />
          </div>

          <!-- 退申請者修改 -->
          <div
            v-show="
              oHead.wfRejectToApplier == '1' &&
              oHead.EndoreMustWait == '0' &&
              oHead.wfStatusFlag != 'O' &&
              oHead.wfStatusFlag != 'D'
            "
          >
            <DxButton
              :text="Message('sys_wfRejectToApplier')"
              :hint="Message('sys_wfRejectToApplier')"
              type="default"
              icon="undo"
              :disabled="rejectButtonDisabled"
              :styling-mode="appInfo.formButtonStylingMode || 'contained'"
              @click="onFlowRejectToApplierModify"
            />
          </div>

          <!-- 加簽按鈕 -->
          <div
            v-show="
              oHead.wfAllowSign == '1' &&
              oHead.wfStatusFlag != 'D' &&
              oHead.wfStatusFlag != 'O'
            "
          >
            <DxButton
              :text="Message('sys_endorse')"
              :hint="Message('sys_endorse')"
              type="default"
              icon="group"
              :disabled="endoreButtonDisabled"
              :styling-mode="appInfo.formButtonStylingMode || 'contained'"
              @click="onFlowEndore"
            />
          </div>
        </div>

        <!-- 通知確認 -->
        <div
          class="formButton"
          v-show="
            oHead.wfStatusFlag == 'B' ||
            oHead.wfStatusFlag == 'N' ||
            oHead.wfStatusFlag == 'R'
          "
        >
          <div>
            <DxButton
              :text="Message('sys_confirm')"
              :hint="Message('sys_confirm')"
              type="default"
              icon="todo"
              :styling-mode="appInfo.formButtonStylingMode || 'contained'"
              @click="onFlowConfirmNotify"
            />
          </div>
        </div>
      </div>

      <div
        style="text-align: right; padding-right: 25px; padding-bottom: 10px"
        v-if="oHead.EndoreMustWait == '1'"
      >
        {{ Message('sys_AdminWFSForm_Caption5') }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import DxLoadPanel from 'devextreme-vue/load-panel'
import DxTextArea from 'devextreme-vue/text-area'
import DxButton from 'devextreme-vue/button'
import AdminFormView from './AdminFormView.vue'
import AdminGridForm from './AdminGridForm.vue'
import appInfo from '@/utils/app-Info'
import { apiPost } from '@/utils/api-util'
import { sizes } from '@/utils/media-query'

// ============================================
// Props
// ============================================
interface Props {
  XMLName?: string
  XMLParameter?: string
  wfSNMsgGuid?: string
  isApplierMoidfy?: boolean
  XMLCaption?: string
  pageKey?: string
}

const props = withDefaults(defineProps<Props>(), {
  XMLName: '',
  XMLParameter: '',
  wfSNMsgGuid: '',
  isApplierMoidfy: false,
  XMLCaption: '',
  pageKey: ''
})

// ============================================
// 狀態變數
// ============================================
const loading = ref(false)
const loadingMessage = ref('loading...please wait...')
const isVisible = ref(false)
const oHead = ref<any>({})
const wfRefGuid = ref('00000000-0000-0000-0000-000000000000')
const wfOpinion = ref('')
const form_hasEdit = ref(false)
const form_XMLName = ref('')
const approveButtonDisabled = ref(false)
const rejectButtonDisabled = ref(false)
const endoreButtonDisabled = ref(true)
const endoreUserList = ref<string[]>([])
const notifyUserList = ref<string[]>([])
const POGDataSource = ref<any[]>([])
const defaultNotifyUsers = ref('')

// ============================================
// Computed
// ============================================
const toolbarClass = computed(() => {
  return 'icon dx-icon-bookmark toolbarIcon'
})

const lblWidth = computed(() => {
  const isLargeScreen = sizes()['screen-large']
  return isLargeScreen ? `${window.screen.availWidth / 8}px` : '100%'
})

// ============================================
// 方法
// ============================================
const isNullOrEmpty = (value: any): boolean => {
  return value === null || value === undefined || value === ''
}

const Message = (key: string): string => {
  // TODO: 實作多語系訊息
  return key
}

const setButtonDisabled = () => {
  // TODO: 實作按鈕禁用邏輯
  if (endoreUserList.value.length > 0) {
    endoreButtonDisabled.value = false
  } else {
    endoreButtonDisabled.value = true
  }
}

const onFlowEditSave = async () => {
  // TODO: 實作流程編輯儲存邏輯
  console.log('Flow edit save')
}

const onFlowApprove = async () => {
  loading.value = true
  try {
    const res = await apiPost('/api/wfs/FlowApprove', {
      wfSNMsgGuid: props.wfSNMsgGuid,
      UserGuid: appInfo.userInfo.userGuid,
      wfOpinion: wfOpinion.value,
      NotifyUserGuids: notifyUserList.value.join(','),
    })

    if (res.status === 200) {
      // TODO: 實作成功處理
      // showNotify('sys_updSuccess', 'success', appInfo.notifyDelay)
      // 重新載入資料或關閉視窗
    }
  } catch (err) {
    console.error(err)
    alert('Flow approve error')
  } finally {
    loading.value = false
  }
}

const onFlowReject = async () => {
  loading.value = true
  try {
    const res = await apiPost('/api/wfs/FlowReject', {
      wfSNMsgGuid: props.wfSNMsgGuid,
      UserGuid: appInfo.userInfo.userGuid,
      wfOpinion: wfOpinion.value,
    })

    if (res.status === 200) {
      // TODO: 實作成功處理
    }
  } catch (err) {
    console.error(err)
    alert('Flow reject error')
  } finally {
    loading.value = false
  }
}

const onFlowRejectPrevious = async () => {
  // TODO: 實作退回上一關邏輯
  console.log('Flow reject previous')
}

const onFlowRejectToApplierModify = async () => {
  // TODO: 實作退回申請者修改邏輯
  console.log('Flow reject to applier modify')
}

const onFlowChangeApprover = async () => {
  // TODO: 實作變更簽核者邏輯
  console.log('Flow change approver')
}

const onFlowEndore = async () => {
  // TODO: 實作加簽邏輯
  console.log('Flow endorse')
}

const onFlowConfirmNotify = async () => {
  loading.value = true
  try {
    const res = await apiPost('/api/wfs/FlowConfirmNotify', {
      wfSNMsgGuid: props.wfSNMsgGuid,
      UserGuid: appInfo.userInfo.userGuid,
    })

    if (res.status === 200) {
      // TODO: 實作成功處理
      isVisible.value = false
    }
  } catch (err) {
    console.error(err)
    alert('Flow confirm notify error')
  } finally {
    loading.value = false
  }
}

const onFlowConfirmNotifyAutoClose = () => {
  // TODO: 實作自動關閉邏輯
  console.log('Flow confirm notify auto close')
}

// ============================================
// Watch
// ============================================
watch(
  endoreUserList,
  () => {
    setButtonDisabled()
  },
  { deep: true }
)

// ============================================
// 生命週期
// ============================================
onMounted(() => {
  wfRefGuid.value = props.XMLParameter || '00000000-0000-0000-0000-000000000000'
  loading.value = true

  const getFlowState = async () => {
    if (props.isApplierMoidfy) {
      return apiPost('/api/wfs/GetFlowStepStatusBywfSNMsgGuidByApplierEdit', {
        wfSNMsgGuid: props.wfSNMsgGuid,
        UserGuid: appInfo.userInfo.userGuid,
        wfRefGuid: wfRefGuid.value,
        XMLName: props.XMLName,
      })
    } else {
      return apiPost('/api/wfs/GetFlowStepStatusBywfSNMsgGuid', {
        wfSNMsgGuid: props.wfSNMsgGuid,
        UserGuid: appInfo.userInfo.userGuid,
        wfRefGuid: wfRefGuid.value,
      })
    }
  }

  const getPOGdataSource = async () => {
    // TODO: 實作 GetCMSCommand
    // return GetCMSCommand('GetCMSUserByWFSForm.xml', null)
    return Promise.resolve({ data: [] })
  }

  const getWFSNotify = async () => {
    // TODO: 實作 GetCMSCommand
    // return GetCMSCommand('GetWFSNotifyByUserGuid.xml', props.wfSNMsgGuid)
    return Promise.resolve({ data: [] })
  }

  Promise.all([getFlowState(), getPOGdataSource(), getWFSNotify()])
    .then((res) => {
      oHead.value = res[0].data?.[0] || {}
      POGDataSource.value = res[1].data || []
      defaultNotifyUsers.value = isNullOrEmpty(res[2].data)
        ? ''
        : (res[2].data[0] as any)?.NotifyUserGuids || ''

      if (!isNullOrEmpty(res[2].data)) {
        notifyUserList.value = defaultNotifyUsers.value.split(',').filter(Boolean)
      }

      if (!oHead.value || Object.keys(oHead.value).length === 0) {
        isVisible.value = false
        alert('Data not found, Please contact TIC IT department.')
        return
      }

      wfOpinion.value = oHead.value.wfOpinion || ''

      form_hasEdit.value =
        oHead.value.wfAllowEdit == '1' &&
        !'BD'.includes(oHead.value.wfStatusFlag)
          ? true
          : false
      form_XMLName.value = oHead.value.wfFlowStepXML || ''

      if (
        oHead.value.wfStatusFlag == 'B' ||
        oHead.value.wfStatusFlag == 'N' ||
        oHead.value.wfStatusFlag == 'R'
      ) {
        onFlowConfirmNotifyAutoClose()
        isVisible.value = false
      } else {
        isVisible.value = true
      }

      if (oHead.value.isHandle == '1') {
        isVisible.value = false
        alert(Message('sys_flowHasApproved'))
        return
      }

      if (oHead.value.IsReceiver == '0') {
        isVisible.value = false
        alert(Message('sys_flowNotReceiver'))
        return
      }

      setButtonDisabled()
      loading.value = false
    })
    .catch((err) => {
      console.error(err)
      loading.value = false
      isVisible.value = false
    })
})
</script>

<style lang="scss" scoped>
@use "sass:math";

.lableStyle {
  line-height: 1.8em;
  padding: 5px 5px 0px 5px;
  background-color: #337ab7;
  color: white;
  border-radius: 7px 7px 0px 0px;
}

.labelAlign {
  justify-content: center;
  align-items: center;
}

.labelLine {
  background-color: #337ab7;
  width: 100%;
  height: 3px;
  flex-direction: column;
}
</style>

