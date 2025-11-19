import{
    createRouter,
    createWebHashHistory
} from "vue-router";

import simpleLayout from "./layouts/single-card.vue"; 
import CMSLogin from "./views/CMSLogin.vue";
import CMSHomePage from "./views/CMSHomePage.vue";
import CMSMainMenu from "./views/CMSMainMenu.vue";


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
            meta: { requiresAuth: true },
            component: CMSMainMenu,
        },
        {
            path: "/CMSHomePage",
            name: "CMSHomePage",
            meta: { requiresAuth: true },
            component: CMSHomePage,
        }
        ]
})

export default router;
