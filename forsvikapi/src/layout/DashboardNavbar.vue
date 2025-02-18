<template>
  <base-nav
    class="navbar-top navbar-dark"
    id="navbar-main"
    :show-toggle-button="false"
    expand
  >
    <div class="row">
      <span class="h4 mb-9 text-white text-uppercase d-none d-lg-inline-block"
        >Forsviks-Guidens Arkiv</span
      >
    </div>
    <div class="row">
      <form
        class="navbar-search navbar-search-dark form-inline mr-3 d-none d-md-flex ml-lg-auto"
      >
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
    <div class="row mt-4 ml-5" style="width: 500px">
      <h2 style="color: white">
        Välkommen till Forsviks-Guidens filarkiv. Här kan du hitta
        nutida och gamla bilder och dokument.
      </h2>
    </div>
  </base-nav>
</template>
<script>
import SearchBar from "../modules/SearchBar";

export default {
  components: {
    SearchBar,
  },
  data() {
    return {
      activeNotifications: false,
      showMenu: false,
      searchQuery: "",
    };
  },
  methods: {
    searchChanged(text, category) {
      let encodedText = encodeURIComponent(text);
      this.$router.push({
        name: "search",
        query: { query: encodedText, category: category },
      });
    },
    toggleSidebar() {
      this.$sidebar.displaySidebar(!this.$sidebar.showSidebar);
    },
    hideSidebar() {
      this.$sidebar.displaySidebar(false);
    },
    toggleMenu() {
      this.showMenu = !this.showMenu;
    },
  },
  mounted() {
    //EventBus.$on('searchChanged', (value) => {
    //console.log("Search: ", value);
    //})
  },
};
</script>
