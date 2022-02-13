<template>
  <div>
    <div class="container-fluid mt--5">
      <div class="row">
        <div class="col-xl-12">
          <card header-classes="bg-transparent" shadow shadowSize="3">
            <template v-slot:header>
              <div class="row">
                <div class="col-md-6">
                  <h1>Arkiv</h1>
                </div>
                <div class="col-md-6 text-right">
                  <button
                    v-if="isAdmin"
                    type="button"
                    @click="addArchive"
                    class="btn-forsvik"
                  >
                    Lägg till arkiv
                  </button>
                </div>
              </div>
            </template>
            <div class="row" style="min-height: 600px">
              <div
                v-for="archive in archives"
                :key="archive.id"
                class="col-md-3 mb-4"
                style="cursor: pointer"
              >
                <archive-pane
                  :archive="archive"
                  :folderId="archive.id"
                  @click="openArchive(archive.id)"
                  @editClicked="onEditArchive"
                />
              </div>
            </div>
          </card>
        </div>
      </div>
      <edit-folder-modal
        :isOpen="showEditArchive"
        @closed="closed"
        @deleteFolder="deleteFolder"
        :folderId="selectedFolderId"
      />
    </div>
  </div>
</template>
<script>
import EditFolderModal from "../modules/EditFolderModal";
import ArchivePane from "../modules/ArchivePane.vue";
import axios from "axios";

export default {
  components: {
    EditFolderModal,
    ArchivePane,
  },
  data() {
    return {
      showEditArchive: false,
      archives: [],
      selectedFolderId: null,
    };
  },
  computed: {
    isLoggedIn() {
      return window.currentUser().isLoggedIn;
    },
    isAdmin() {
      return window.currentUser().isAdmin;
    },    
  },
  mounted() {
    this.loadArchives();
  },
  methods: {
    openArchive(id) {
      this.$router.push({ name: "folder", query: { id: id } });
    },
    imgFromId(id) {
      let x = "/api/file/resource/" + id;
      return x;
    },
    addArchive() {
      this.selectedFolderId = null;
      this.showEditArchive = true;
    },
    deleteFolder(folderId) {
      axios.post("/api/file/removefolder", { folderId: folderId }).then(() => {
        this.showEditArchive = false;
        this.loadArchives();
      });
    },
    closed() {
      this.showEditArchive = false;
      this.loadArchives();
    },
    loadArchives() {
      this.emitter.emit("folder-open", false);
      fetch("/api/archive/getarchives")
        .then((response) => response.json())
        .then((archives) => {
          this.archives = archives;
        });
    },
    onEditArchive(folderId) {
      this.selectedFolderId = folderId;
      this.showEditArchive = true;
    },
  },
};
</script>
<style></style>
