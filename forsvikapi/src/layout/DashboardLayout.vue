<style scoped>
.link {
  color: white;
  font-size: 16px;
  font-weight: bold;
  font-variant: small-caps;
  display: inline-block;
  cursor: pointer;
  margin-right: 20px;
}
.centered {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  transform: -webkit-translate(-50%, -50%);
  transform: -moz-translate(-50%, -50%);
  transform: -ms-translate(-50%, -50%);
}
.loading {
  width: 220px;
  padding: 20px;
  border-radius: 5px;
  background-color: #444;
  text-align: center;
  color: white;
  font-size:14px;  
  opacity: 0;
  pointer-events: none;
  transition: opacity 0.2s;
}
.loadingvisible {
  opacity: 0.6;
}
</style>
<template>
  <div class="main-content">
    <div
      class="header pb-5"
      style="
        background-image: url(img/theme/forsvik_bridge.jpg);
        background-size: cover;
        background-position: center top;
      "
    >
      <span class="mask bg-gradient-indigo opacity-8"></span>

      <div class="row" style="padding: 20px">
        <div class="col-md-4">
          <span class="h3 text-white text-uppercase">Forsviks-Guidens Arkiv</span>
        </div>
        <div class="col-md-8 text-right">
          <router-link to="/dashboard" class="link">Hem</router-link>
          <div class="link" @click="gotoLastSearch">Sökresultat</div>
          <div v-if="isAdminLoggedIn" class="link" @click="gotoAdmin">
            Admin
          </div>
          <div class="link" @click="loginOrOut">{{ loginLabel }}</div>
        </div>
      </div>
      <div class="row mt-3">
        <div class="col-md-6">
          <form class="navbar-search navbar-search-dark ml-6 mt-2">
            <div class="form-group mb-0">
              <search-bar
                placeholder="Search"
                class="input-group-alternative"
                @textChanged="searchChanged"
              >
              </search-bar>
            </div>
          </form>
        </div>
        <div class="col-md-6">
          <slot name="folderimage" style="position: fixed"></slot>
          <h2 style="color: white; width: 500px" v-if="showBigText">
            Välkommen till Forsviks filarkiv. Här kan du hitta alla tänkbara
            nutida och gamla bilder och dokument.
          </h2>
        </div>
      </div>
      <div class="row ml-6">
        <div class="col-md-12">
          <div title="Tillbaka" class="prevnext" @click="goPrevious">
            <i class="fas fa-arrow-left" style="margin-right: 20px"></i>
          </div>
          <div title="Framåt" class="prevnext" @click="goNext">
            <i class="fas fa-arrow-right" style="margin-right: 50px"></i>
          </div>
        </div>
      </div>
    </div>
    <router-view></router-view>
    <div :class="{'loadingvisible': isBusy}" class="centered loading">
      <span>Arbetar. Var god vänta...</span>
    </div>

  </div>
</template>
<script>
import axios from "axios";
import SearchBar from "../modules/SearchBar";
import LayoutMixin from "../mixins/commonMixin";

export default {
  mixins: [LayoutMixin],
  components: {
    SearchBar,
  },
  data() {
    return {
      category: null,
      encodedText: null,
      loginLabel: null,
      showBigText: true,
      isBusy: false,
    };
  },
  computed: {
    isAdminLoggedIn() {
      let curr = window.currentUser();
      return curr.isAdmin && curr.isLoggedIn;
    },
  },
  mounted() {
    this.setAuth();
    this.emitter.on("folder-open", (isOpen) => {
      this.showBigText = !isOpen;
    });
    this.emitter.on("show-busy", (show) => {
      this.isBusy = show;
    });
  },
  methods: {
    goPrevious() {
      history.back();
    },
    goNext() {
      history.forward();
    },
    setAuth() {
      this.loginLabel = window.currentUser().isLoggedIn
        ? "Logga ut"
        : "Logga in";
    },
    gotoLastSearch() {
      if (this.encodedText) {
        this.$router.push({
          name: "search",
          query: { query: this.encodedText, category: this.category },
        });
      }
    },
    gotoAdmin() {
      this.$router.push("useradmin");
    },
    searchChanged(text, category) {
      this.encodedText = encodeURIComponent(text);
      this.category = category;
      this.gotoLastSearch();
    },
    loginOrOut() {
      if (window.currentUser().isLoggedIn) {
        axios.post("/api/archive/logout").then(() => {
          document.cookie =
            "username=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
          localStorage.removeItem("currentUser");
          this.setAuth();
          this.$router.push("dashboard");
          this.$router.go();
        });
      } else {
        this.$router.push({ name: "login" });
      }
    },
  },
};
</script>
<style lang="scss"></style>
