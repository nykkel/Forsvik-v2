<template>
  <div class="row justify-content-center mt-5">
    <div class="col-lg-5 col-md-7">
      <div class="card bg-secondary shadow border-0">
        <div class="card-header bg-transparent">
          <div class="text-muted text-center mt-2 mb-3">
            <i class="fas fa-unlock-alt" style="font-size: 60px"></i>
          </div>
          <div class="btn-wrapper text-muted text-center">
            <small
              >För att få rättighet att ändra i arkivet krävas att du loggar
              in</small
            >
          </div>
        </div>

        <div class="card-body px-lg-5 py-lg-5">
          <form role="form">
            <input
              placeholder="Email"
              type="text"
              class="form-control mr-4 mb-4"
              v-model="model.email"
            />

            <input
              placeholder="Lösenord"
              type="password"
              class="form-control mr-4 mb-4"
              v-on:keyup.enter="doLogin"
              v-model="model.password"
            />

            <label class="checkdiv">
            <span style="font-size:16px">Kom ihåg inloggning</span>
              <input type="checkbox" v-model="model.rememberMe" />
              <span class="checkmark"></span>
            </label>
            <div class="text-center mt-5">
              <button class="btn btn-default" type="button" @click="doLogin">
                Öppna arkivet
              </button>
            </div>
          </form>
        </div>
      </div>
      <div class="row mt-3">
        <div class="col-6">
          <a href="#" class="text-light"><small>Glömt lösenord?</small></a>
        </div>
        <div class="col-6 text-right">
          <a href="#" class="text-light" @click="gotoDashboard"
            ><small>Tillbaka</small></a
          >
        </div>
      </div>
    </div>
    <modal v-model:show="showFailed" @close="close">
      <template v-slot:header>
        <h2 class="heading-small text-muted">Misslyckades</h2>
      </template>
      <h4>Användarnamn eller lösenord var felaktigt</h4>
      <template v-slot:footer>
        <base-button type="secondary" @click="close">Stäng</base-button>
      </template>
    </modal>
  </div>
</template>
<script>
import axios from "axios";

export default {
  name: "login",
  data() {
    return {
      model: {
        email: "",
        password: "",
        rememberMe: false,
      },
      showFailed: false,
    };
  },
  methods: {
    gotoDashboard() {
      this.$router.push("dashboard");
    },
    doLogin() {
      axios.post("/api/archive/login", this.model).then((response) => {
        if (response.data) {
          localStorage.setItem("currentUser", JSON.stringify(response.data));
          if (response.data.isFirstTimeUser) {
            this.$router.push("useradmin");
          } else {
            this.$router.push("dashboard");
          }
        } else {
          this.showFailed = true;
        }
      });
    },
    close() {
      this.showFailed = false;
    },
  },
};
</script>
<style></style>
