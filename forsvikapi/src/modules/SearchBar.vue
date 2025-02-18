<template>
  <div>
    <div class="row">
      <div class="col-md-12">
        <div class="search-frame">
          <table width="100%">
            <tr>
              <td>
                <input
                  type="text"
                  v-model="searchText"
                  placeholder="Sök"
                  class="search-text"
                  style="box-shadow: none"
                  v-on:keyup.enter="doSearch"
                  v-on:keydown.enter.capture.prevent.stop
                />
              </td>
              <td>
                <div
                  class="text-right"
                  v-on:click="doSearch"
                  style="cursor: pointer; color: white"
                >
                  <i class="fas fa-search"></i>
                </div>
              </td>
            </tr>
          </table>
        </div>
        <!-- <div class="form-group input-group focused" style="width: 440px">
          <slot>
            <input
              type="text"
              v-model="searchText"
              placeholder="Sök"
              class="form-control input-group-alternative"
              v-on:keyup.enter="doSearch"
              v-on:keydown.enter.capture.prevent.stop
            />
          </slot>
          <div
            class="input-group-append"
            v-on:click="doSearch"
            style="cursor: pointer"
          >
            <span class="input-group-text">
              <slot name="addonRight">
                <i class="fas fa-search"></i>
              </slot>
            </span>
          </div>
        </div> -->
      </div>
    </div>
    <div class="row">
      <div class="col-md-12">
        <table class="ml-2 mb-2">
          <tr>
            <td>
              <label class="radio-span heading-small"
                >Allt
                <input
                  id="altTest"
                  type="radio"
                  value="0"
                  v-model="category"
                  checked="checked"
                  name="radio"
                />
                <span class="radio-mark"></span>
              </label>
            </td>
            <td>
              <label class="radio-span heading-small"
                >Media
                <input type="radio" value="1" v-model="category" name="radio" />
                <span class="radio-mark"></span>
              </label>
            </td>
            <td>
              <label class="radio-span heading-small"
                >Dokument
                <input type="radio" value="2" v-model="category" name="radio" />
                <span class="radio-mark"></span>
              </label>
            </td>
            <td>
              <label class="radio-span heading-small"
                >Kataloger
                <input type="radio" value="3" v-model="category" name="radio" />
                <span class="radio-mark"></span>
              </label>
            </td>
          </tr>
        </table>
      </div>
    </div>
  </div>
</template>
<script>
//import Tab from '../components/Tabs/Tab.vue';
export default {
  //components: { Tab },
  inheritAttrs: false,
  name: "search-bar",
  data() {
    return {
      searchText: null,
      category: 0,
    };
  },
  methods: {
    doSearch(evt) {
      evt.preventDefault();
      this.$emit("textChanged", this.searchText, this.category);
    },
  },
};
</script>
<style></style>
