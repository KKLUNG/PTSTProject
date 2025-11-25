import{
    createRouter,
    createWebHashHistory
} from "vue-router";

import auth from "./utils/auth";
// 佈局組件
import simpleLayout from "./layouts/single-card.vue";
import sideNavOuterToolbar from "./layouts/side-nav-outer-toolbar.vue";
import sideNavInnerToolbar from "./layouts/side-nav-inner-toolbar.vue";
import mainMenuLayout from "./layouts/MainMenuLayout.vue";
import noMenuLayout from "./layouts/noMenuLayout.vue";
import mobileWFSLayout from "./layouts/mobileWFSLayout.vue";
// 視圖組件
import CMSLogin from "./views/CMSLogin.vue";
import CMSHomePage from "./views/CMSHomePage.vue";
import CMSMainMenu from "./views/CMSMainMenu.vue";
import CMSPage from "./views/CMSPage.vue";
import CMSUserManagement from "./views/CMSUserManagement.vue";


//ts不用new createRouter，直接createRouter
const router = createRouter({
    history: createWebHashHistory(),
    routes: [
        {
            path: "/",
            redirect: "/CMSLogin"
        },
        {
            path: "/CMSLogin",
            name: "CMSLogin",
            meta: { requiresAuth: false, layout: simpleLayout },
            component: CMSLogin,
        },
        {
            path: "/CMSMainMenu",
            name: "CMSMainMenu",
            meta: { requiresAuth: true, layout: sideNavOuterToolbar },
            component: CMSMainMenu,
        },
        {
            path: "/CMSHomePage",
            name: "CMSHomePage",
            meta: { requiresAuth: true, layout: sideNavOuterToolbar },
            component: CMSHomePage,
        },
        {
            path: "/CMSPage/:menuGuid",
            name: "CMSPage",
            meta: { requiresAuth: true, layout: sideNavOuterToolbar },
            component: CMSPage,
        },
        {
            path: "/CMSUserManagement",
            name: "CMSUserManagement",
            meta: { requiresAuth: true, layout: sideNavOuterToolbar },
            component: CMSUserManagement,
        }
        ]
})

// 路由守衛
router.beforeEach((to, from, next) => {
    // 如果已登入且訪問登入頁，跳轉到主頁
    if (to.name === 'CMSLogin' && auth.authenticated()) {
        next({ name: 'CMSMainMenu' })
        return
    }

    // 檢查是否需要驗證
    if (to.meta.requiresAuth) {
        if (!auth.authenticated()) {
            next({
                name: 'CMSLogin',
                query: { redirect: to.fullPath }
            })
            return
        }
    }

    next()
})

export default router;
