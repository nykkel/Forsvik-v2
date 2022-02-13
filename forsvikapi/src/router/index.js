import { createRouter, createWebHashHistory } from "vue-router";

import DashboardLayout from "@/layout/DashboardLayout";
import AuthLayout from "@/layout/AuthLayout";
import FreeLayout from "@/layout/FreeLayout";

import Dashboard from "../views/Dashboard.vue";
import Folder from "../views/Folder.vue";
import Login from "../views/Login.vue";
import UserAdmin from "../views/UserAdmin.vue";
import QueryOne from "../views/pedigree/QueryOne.vue";
import SearchPage from "../views/SearchPage.vue";

const routes = [
  {
    path: "/",
    redirect: "/dashboard",
    component: DashboardLayout,
    children: [
      {
        path: "/dashboard",
        name: "dashboard",
        components: { default: Dashboard },
      },
      {
        path: "/folder",
        name: "folder",
        components: { default: Folder },
      },
      {
        path: "/search",
        name: "search",
        components: { default: SearchPage },
      },
      {
        path: "/useradmin",
        name: "useradmin",
        components: { default: UserAdmin },
      }      
    ]
  },
  {
    path: "/",
    redirect: "login",
    component: AuthLayout,
    children: [
      {
        path: "/login",
        name: "login",
        components: { default: Login },
      },
      {
        path: "/pedigree",
        name: "queryOne",
        components: { default: QueryOne },
      }
    ],
  },
  {
    path: "/",
    redirect: "pedigree",
    component: FreeLayout,
    children: [      
      {
        path: "/pedigree",
        name: "queryOne",
        components: { default: QueryOne },
      }
    ],
  },  
];

const router = createRouter({
  history: createWebHashHistory(),
  linkActiveClass: "active",
  routes,
});

export default router;
