<style scoped>
.ghost-town {
  width: 200px;
  height: 133px;
  border-radius: 30px;
  overflow: hidden;
}
</style>
<template>
  <div class="shadow1" style="cursor: pointer">
    <div class="forsvik-header" id="cardHeader" style="padding: 15px">
      <table style="width: 100%">
        <tr>
          <td>
            <h2 class="mb-0 forsvik-text forsvik-header-text">
              {{ archive.name }}
            </h2>
          </td>
          <td>
            <div v-if="isLoggedIn" class="text-right" @click="editArchive">
              <i class="far fa-edit"></i>
            </div>
          </td>
        </tr>
      </table>
    </div>
    <div class="card-body">
      <div class="row justify-center">
        <p class="forsvik-text">{{ archive.description }}</p>
      </div>
      <div class="row justify-center">
        <span
          v-if="archive.imageFileId"
          class="ghost-town"
          style="background-color: transparent"
        >
          <img :src="imgFromId(archive.imageFileId)" class="ghost-town" />
        </span>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  name: "archive-pane",
  props: {
    archive: Object,
    folderId: String,
  },
  computed: {
    isLoggedIn() {
      return window.currentUser().isLoggedIn;
    },
  },
  methods: {
    imgFromId(id) {
      return "/api/file/thumbnail/" + id;
    },
    editArchive(evt) {
      this.$emit("editClicked", this.folderId);
      evt.stopPropagation();
    },
  },
};
</script>
<style></style>
