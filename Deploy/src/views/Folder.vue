<style scoped>
.btn-forsvik {
  background-color: #444;
  border-color: white;
  color: whitesmoke;
}
.folder-box {
  box-shadow: 0 3.2px 7.2px 0 var(--callout-shadow-color, rgba(0, 0, 0, 0.132)),
    0 0.6px 1.8px 0 var(--callout-shadow-secondary-color, rgba(0, 0, 0, 0.108));
}
</style>
<template>
  <div>
    <div class="folder-view" v-show="currentFolder.imageFileId">
      <img
        v-if="currentFolder.imageFileId"
        :src="imgFromId(currentFolder.imageFileId)"
        width="300"
        height="200"
      />
    </div>
    <div class="container-fluid mt--5">
      <div class="row">
        <div class="col-xl-12">
          <card header-classes="bg-transparent" shadow shadowSize="3">
            <template v-slot:header>
              <div class="row ml-2">
                <div class="col-md-4">
                  <div class="row">
                    <div
                      v-for="nav in navigation"
                      :key="nav.folderId"
                      class="text-success mr-3"
                    >
                      <router-link
                        :to="nav.route"
                        class="heading-small text-gray"
                        style="font-size: 13px; display: inline-block"
                        >{{ nav.name }}</router-link
                      >
                      <span style="font-size: 30px; margin-left: 10px">|</span>
                    </div>
                  </div>
                </div>
                <div class="col-md-8">
                  <div class="row">
                    <div class="col-md-3">
                      <h2 class="heading-small text-muted" style="padding: 3px">
                        Namn
                      </h2>
                      <edit-label
                        @saveChanges="saveFolderName"
                        :itemId="folderId"
                        :inputText="currentFolder.name"
                      />
                    </div>
                    <div class="col-md-3">
                      <h2 class="heading-small text-muted">Sök-taggar</h2>
                      <edit-label
                        @saveChanges="saveFolderTags"
                        :itemId="folderId"
                        ghostText="Ange kommaseparerat (tag1, tag2...)"
                        :inputText="currentFolder.tags"
                      />
                    </div>
                    <div class="col-md-3">
                      <h2 class="heading-small text-muted">Beskrivning</h2>
                      <edit-label
                        @saveChanges="saveFolderDescription"
                        :itemId="folderId"
                        :inputText="currentFolder.description"
                      />
                    </div>
                    <div class="col-md-3 text-right">
                      <button
                        v-if="isLoggedIn"
                        style="width: 180px"
                        @click="addFolder"
                        class="btn-forsvik"
                      >
                        Lägg till katalog
                      </button>
                    </div>
                  </div>
                </div>
              </div>
              <div
                v-if="hasFolders"
                style="height: 1px; background-color: #ccc"
              />
            </template>
            <div v-if="hasFolders" class="row" style="margin-top: -30px">
              <h2 class="heading-small text-muted ml-3">Kataloger</h2>
            </div>
            <div v-if="hasFolders">
              <div class="card-body">
                <div class="row">
                  <div
                    class="col-lg-2"
                    v-for="folder in folders"
                    :key="folder.id"
                  >
                    <folder-pane
                      class="folder-box"
                      :folder="folder"
                      @click="openFolder(folder.id)"
                      @editFolder="editFolder"
                    />
                  </div>
                </div>
              </div>
            </div>
            <div class="row" v-bind:class="{ move45: !hasFolders }">
              <div
                style="background-color: #000000; height: 1px; margin: 10px"
              />
            </div>
            <div style="height: 1px; background-color: #ccc" />
            <div class="row ml-3 mr--4 mt-3">
              <file-view :folderId="folderId" :reload="reloadFiles" />
            </div>
          </card>
        </div>
      </div>
      <edit-folder-modal
        :isOpen="showEditFolder"
        @closed="addFolderClosed"
        :newFolderParentId="currentFolder.id"
        @deleteFolder="deleteFolder"
        :folderId="subFolderId"
      />
    </div>
  </div>
</template>
<script>
import FileView from "./FileView";
import FolderPane from "../modules/FolderPane";
import EditLabel from "../modules/EditLabel";
import CommonMixin from "../mixins/commonMixin";
import EditFolderModal from "../modules/EditFolderModal";
import axios from "axios";

export default {
  name: "folder-view",
  mixins: [CommonMixin],
  components: {
    EditFolderModal,
    FileView,
    FolderPane,
    EditLabel,
  },
  data() {
    return {
      currentFolder: {
        folderId: null,
        parentFolderId: null,
        name: null,
      },
      navigation: null,
      reloadFiles: false,
      folders: [],
      files: [],
      showEditFolder: false,
      hasFolders: false,
      subFolderId: null,
    };
  },
  computed: {
    folderId() {
      return this.$route.query.id;
    },
    isLoggedIn() {
      return window.currentUser().isLoggedIn;
    },
  },
  watch: {
    folderId(value) {
      if (value) {
        this.initialize();
      }
    },
  },
  mounted() {
    this.initialize();
  },
  methods: {
    initialize() {
      this.showBusy();
      this.loadCurrentFolder();
      this.loadFolders();
      this.loadFiles();
      this.showBusy();
    },
    deleteFolder(folderId) {
      axios.post("/api/file/removefolder", { folderId: folderId }).then(() => {
        this.$router.push({ name: "dashboard" });
      });
    },
    editFolder(id) {
      this.showEditFolder = true;
      this.subFolderId = id;
    },
    handleFileUploads() {
      this.files = this.$refs.files.files;
      this.uploadData();
    },
    createLink(id) {
      return "/folder/" + id;
    },
    openFolder(id) {
      this.$router.push({ name: "folder", query: { id: id } }).then(() => {
        this.loadFolders();
      });
    },
    imgFromId(id) {
      return "/api/file/resource/" + id;
    },
    addFolder() {
      this.subFolderId = null;
      this.showEditFolder = true;
    },
    addFolderClosed() {
      this.showEditFolder = false;
      this.subFolderId = null;
      this.loadFolders();
    },
    saveFolderName(item) {
      this.currentFolder.name = item.text;
      this.saveCurrentFolder();
    },
    saveFolderTags(item) {
      this.currentFolder.tags = item.text;
      this.saveCurrentFolder();
    },
    saveFolderDescription(item) {
      this.currentFolder.description = item.text;
      this.saveCurrentFolder();
    },
    saveCurrentFolder() {
      axios.post("/api/archive/savefolder", this.currentFolder);
    },
    loadFiles() {
      this.reloadFiles = !this.reloadFiles;
    },
    loadCurrentFolder() {
      this.emitter.emit("folder-open", true);
      fetch("/api/archive/getfolder/" + this.folderId)
        .then((response) => response.json())
        .then((folder) => (this.currentFolder = folder));
    },
    loadFolders() {
      fetch("/api/archive/getfolders/" + this.folderId)
        .then((response) => response.json())
        .then((folders) => {
          this.folders = folders;
          this.hasFolders = this.folders && this.folders.length > 0;

          fetch("/api/archive/getfoldernavigation/" + this.folderId)
            .then((response) => response.json())
            .then((navs) => {
              this.navigation = navs;
              this.hideBusy();
            });
        });
    },
  },
};
</script>
