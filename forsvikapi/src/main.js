/*!

=========================================================
* Vue Argon Dashboard - v2.0.0
=========================================================

* Product Page: https://www.creative-tim.com/product/vue-argon-dashboard
* Copyright 2021 Creative Tim (https://www.creative-tim.com)
* Licensed under MIT (https://github.com/creativetimofficial/vue-argon-dashboard/blob/master/LICENSE.md)

* Coded by Creative Tim

=========================================================

* The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

*/
import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import mitt from "mitt";
import ArgonDashboard from "./plugins/argon-dashboard";
import "element-plus/lib/theme-chalk/index.css";

window.currentUser = () => {
  return {
    isLoggedIn: document.cookie.indexOf(".AspNetCore.Cookies") >= 0,
    isAdmin:
      localStorage.getItem("currentUser") &&
      JSON.parse(localStorage.getItem("currentUser")).isAdmin,
  };
};
window.document.title = "Forsviks-Guidens Arkiv";

const emitter = mitt();
const appInstance = createApp(App);
appInstance.config.globalProperties.emitter = emitter;
appInstance.use(router);
appInstance.use(ArgonDashboard);
appInstance.mount("#app");
