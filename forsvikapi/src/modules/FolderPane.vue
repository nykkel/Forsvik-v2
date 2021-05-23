<template>
  <div class="shadow2" style="cursor: pointer">
    <div class="forsvik-header" style="padding: 10px">
      <table style="width: 100%">
        <tr>
          <td>
            <h4 class="mb-0 forsvik-text forsvik-header-text">
              {{ folder.name }}
            </h4>
          </td>
          <td>
            <div v-if="isLoggedIn" class="text-right" @click="editFolder">
              <i class="far fa-edit"></i>
            </div>
          </td>
        </tr>
      </table>
    </div>
    <div class="card-body">
      <div class="row text-muted">
        <div class="col">
          <p class="forsvik-text" style="font-size: 14px">
            {{ folder.description }}
          </p>
        </div>
        <div class="col">
          <img :src="thumbnailFromId(folder.imageFileId)" class="ghost-town" height="70" width="100" v-if="folder.imageFileId" />
        </div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: "folder-pane",
  props: {
    folder: Object,
  },
  computed: {
    isLoggedIn() {
      return window.currentUser().isLoggedIn;
    },
  },
  methods: {
    thumbnailFromId(id) {
      return "/api/file/thumbnail/" + id;
    },
    editFolder(evt) {
      this.$emit("editFolder", this.folder.id);
      evt.stopPropagation();
      evt.preventDefault();
    },
  },
};
</script>
<style></style>
