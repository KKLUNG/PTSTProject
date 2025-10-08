<template>
  <div class="container">
    <DxLoadPanel
      :hide-on-outside-click="false"
      :visible="loading"
      :shading="true"
      :show-pane="true"
      shading-color="transparent"
      :message="loadingMessage"
    />
    <DxPopup
      ref="rootPopup"
      width="150px"
      height="150px"
      :title="$appInfo.title"
      :drag-enabled="false"
      :hide-on-outside-click="false"
      :visible="hasNewApp"
    >
      <div style="text-align: center">
        <br /><br />
        Download progress <br />{{ downloadProgress }}
      </div>
    </DxPopup>
    <div class="row" v-if="!isLargeScreen && !$appInfo.isMobile">
      <div class="col-12">
        <div class="content-block">
          <span style="font-size: 1.5rem">{{ menuTitle }}</span>
        </div>
      </div>
    </div>
    <div class="row" v-if="oTop.length > 0">
      <div class="col-12">
        <div class="content-block">
          <div class="dx-card" v-for="item in oTop" :key="item.MenusGuid">
            <component
              :is="item.MenusControl"
              :XMLName="item.MenusXMLName"
              :XMLParameter="item.MenusParameter"
              :XMLCaption="
                getFieldCaption(item.MenusTitle, item.MenusTitleLang)
              "
              :pageKey="pageKey"
            ></component>
          </div>
        </div>
      </div>
    </div>
    <div class="content-block">
      <div class="row">
        <div :class="oLeftCol" v-if="oLeft.length > 0">
          <div class="dx-card" v-for="item in oLeft" :key="item.MenusGuid">
            <component
              :is="item.MenusControl"
              :XMLName="item.MenusXMLName"
              :XMLParameter="item.MenusParameter"
              :XMLCaption="
                getFieldCaption(item.MenusTitle, item.MenusTitleLang)
              "
              :pageKey="pageKey"
            ></component>
          </div>
        </div>
        <div :class="oCenterCol" v-if="oCenter.length > 0">
          <div class="dx-card" v-for="item in oCenter" :key="item.MenusGuid">
            <component
              :is="item.MenusControl"
              :XMLName="item.MenusXMLName"
              :XMLParameter="item.MenusParameter"
              :XMLCaption="
                getFieldCaption(item.MenusTitle, item.MenusTitleLang)
              "
              :pageKey="pageKey"
            ></component>
          </div>
        </div>
        <div :class="oRightCol" v-if="oRight.length > 0">
          <div class="dx-card" v-for="item in oRight" :key="item.MenusGuid">
            <component
              :is="item.MenusControl"
              :XMLName="item.MenusXMLName"
              :XMLParameter="item.MenusParameter"
              :XMLCaption="
                getFieldCaption(item.MenusTitle, item.MenusTitleLang)
              "
              :pageKey="pageKey"
            ></component>
          </div>
        </div>
      </div>
    </div>
    <div class="row" v-if="oBottom.length > 0">
      <div class="col-12">
        <div class="content-block">
          <div class="dx-card" v-for="item in oBottom" :key="item.MenusGuid">
            <component
              :is="item.MenusControl"
              :XMLName="item.MenusXMLName"
              :XMLParameter="item.MenusParameter"
              :XMLCaption="
                getFieldCaption(item.MenusTitle, item.MenusTitleLang)
              "
              :pageKey="pageKey"
            ></component>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
</script>

<style>
.home { max-width: 720px; margin: 40px auto; font-family: system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif; }
</style>


