<template>
  <div class="row" style="width: 100%">
    <div class="col-md-6">
      <h2 class="heading-small text-muted mb-2 ml--3">Filer</h2>
    </div>
    <div class="col-md-6 text-right">
      <button
        v-if="isLoggedIn"
        type="button"
        @click="selectFiles"
        class="btn-forsvik mr-2"
      >
        Ladda upp
      </button>
      <input
        type="file"
        id="files"
        ref="files"
        style="display: none"
        multiple="multiple"
        onclick="value = null"
        @change="addFiles"
      />
      <button
        type="button"
        @click="downloadSelectedFiles"
        class="btn-forsvik mr-2"
      >
        Ladda ner
      </button>
      <button
        v-if="isLoggedIn"
        type="button"
        class="btn-forsvik"
        @click="showDeleteModal"
      >
        Ta bort
      </button>
    </div>
  </div>
  <div v-if="showProgress" class="centerprogress">
    <circle-progress :percent="percentCompleted" />
  </div>
  <div class="row">
    <table class="hoverTable">
      <tr class="heading-small text-muted mb-4">
        <th style="width: 100px; cursor: pointer">Välj</th>
        <th style="width: 100px; cursor: pointer">Ikon</th>
        <th style="width: 300px; cursor: pointer">Namn</th>
        <th style="width: 100px; cursor: pointer">Typ</th>
        <th style="width: 100px; cursor: pointer">Storlek</th>
        <th style="width: 300px; cursor: pointer">Beskrivning</th>
        <th style="width: 300px; cursor: pointer">Sök-taggar</th>
        <th style="width: 200px"></th>
      </tr>
      <tr
        v-for="file in files"
        :key="file.id"
        v-on:dblclick="openFile(file)"
        style="padding: 10px; font-size: 14px"
        class="mss"
      >
        <td>
          <label
            class="checkdiv"
            style="vertical-align: top; margin-top: -12px"
          >
            <input type="checkbox" v-model="file.isSelected" />
            <span class="checkmark"></span>
          </label>
        </td>
        <td>
          <img :src="thumbnailFromId(file.id)" height="50" />
        </td>
        <td>
          <edit-label
            @saveChanges="saveNameChanges"
            :itemId="file.id"
            :inputText="file.name"
          />
        </td>
        <td style="text-transform: uppercase" class="forsvik-text">
          {{ file.extension }}
        </td>
        <td>
          <div class="forsvik-text">{{ file.sizeDisplay }}</div>
        </td>
        <td>
          <edit-label
            @saveChanges="saveDescriptionChanges"
            :itemId="file.id"
            :inputText="file.description"
          />
        </td>
        <td>
          <edit-label
            @saveChanges="saveTagsChanges"
            :itemId="file.id"
            :inputText="file.tags"
            ghostText="Ange kommaseparerat (tag1, tag2...)"
          />
        </td>
        <td>
          <div title="Kopiera url" @click="fileUrlToClipboard(file)">
            <i class="fas fa-external-link-alt"></i>
          </div>
        </td>
      </tr>
    </table>
    <input type="hidden" v-text="copyText" id="copyInput" />

    <modal v-model:show="showDelete">
      <template v-slot:header>
        <h2 class="heading-small text-muted">Ta bort</h2>
      </template>
      <h4>Vill du ta bort {{ selectedFileCount }} markerade filer?</h4>
      <template v-slot:footer>
        <base-button type="secondary" @click="hideDeleteModal"
          >Stäng</base-button
        >
        <button
          type="button"
          class="btn btn-default"
          @click="deleteSelectedFiles"
        >
          Ta bort
        </button>
      </template>
    </modal>
    <modal v-model:show="showQuickDownload">
      <template v-slot:header>
        <h2 class="heading-small text-muted">Ladda ner</h2>
      </template>
      <h4>Vill du ladda ner filen?</h4>
      <template v-slot:footer>
        <base-button type="secondary" @click="showQuickDownload = false"
          >Stäng</base-button
        >
        <button type="button" class="btn btn-default" @click="quickDownload">
          Ladda ner
        </button>
      </template>
    </modal>

    <div id="pictureModal" class="modal">
      <span class="close-modal" @click="closeModal">&times;</span>
      
      <img id="modalImg" class="modal-content1" />
      
    </div>
  </div>
</template>
<script>
import EditLabel from "../modules/EditLabel";
import CommonMixin from "../mixins/commonMixin";
import CircleProgress from "vue3-circle-progress";

import axios from "axios";

export default {
  mixins: [CommonMixin],
  name: "file-view",
  components: {
    EditLabel,
    CircleProgress
  },
  props: {
    folderId: String,
    reload: Boolean,
  },
  computed: {
    selectedFileCount() {
      return this.files.filter((f) => {
        return f.isSelected;
      }).length;
    },
    isLoggedIn() {
      return window.currentUser().isLoggedIn;
    },
  },
  data() {
    return {
      files: [],      
      showDelete: false,
      copyText: null,
      showQuickDownload: false,
      selectedFile: null,
      percentCompleted: 0,
      showProgress: false
    };
  },
  watch: {
    reload() {
      this.loadFiles();
    },
    percentCompleted(x){
      console.log(x);      
    }
  },
  methods: {
    fileUrlToClipboard(file) {
      var element = document.getElementById("copyInput");
      element.setAttribute("type", "text");
      element.value = `${window.location.origin}/api/file/resource/${file.id}`;
      element.select();
      document.execCommand("copy");
      element.setAttribute("type", "hidden");
      alert("Kopierad!");
    },
    openFile(file) {
      const ext = file.extension.toLowerCase();
      this.selectedFile = file;

      if (ext !== "jpg" && ext !== "jpeg" && ext !== "png") {
        this.showQuickDownload = true;
        return;
      }

      var modal = document.getElementById("pictureModal");
      var modalImg = document.getElementById("modalImg");

      modal.style.display = "block";
      modalImg.src = "/api/file/resource/" + file.id;
    },
    quickDownload() {
      this.showQuickDownload = false;
      this.selectedFile.isSelected = true;
      this.files = [this.selectedFile];
      this.downloadSelectedFiles();      
    },
    gotoFolder(folderId) {
      this.$router.push({ name: "folder", query: { id: folderId } });
    },
    closeModal() {
      var modal = document.getElementById("pictureModal");
      modal.style.display = "none";
    },
    showDeleteModal() {
      if (this.files.find(f => f.isSelected)) {
        this.showDelete = true;
      } else {
        alert("Du har inte markerat några filer!");
      }
    },
    hideDeleteModal() {
      this.showDelete = false;
    },
    saveNameChanges(item) {      
      let file = this.files.find((f) => f.id === item.id);
      file.name = item.text;
      this.saveFileChanges(file);
    },
    saveDescriptionChanges(item) {
      let file = this.files.find((f) => f.id === item.id);
      file.description = item.text;
      this.saveFileChanges(file);
    },
    saveTagsChanges(item) {
      let file = this.files.find((f) => f.id === item.id);
      file.tags = item.text;
      this.saveFileChanges(file);
    },
    loadFiles() {
      this.showBusy();

      return fetch("/api/archive/getfiles/" + this.folderId)
        .then((response) => response.json())
        .then((files) => {
          this.files = files;
          files.forEach((f) => {
            f.editName = false;
            f.editDescription = false;
            f.editTags = false;
            this.hideBusy();
          });
        });
    },
    saveFileChanges(file) {
      this.showBusy();
      axios.post("/api/archive/savefilechanges", file).then(() => {
        file.editDescription = false;
        file.editName = false;
        file.editTags = false;
        this.hideBusy();
      });
    },
    downloadSelectedFiles() {
      this.showProgress = true;   

      var files = this.files.filter((f) => {
        return f.isSelected;
      });
      if (files.length === 0) {
        this.showProgress = false;   
        alert("Du har inte markerat några filer!");        
        return;
      }
      const body = {
        fileIds: files.map((file) => file.id),
      };
      var callback = this.updateCompleted;
      axios.post("/api/file/resources", body, {
        onDownloadProgress: function(progressEvent) {          
            let completed = Math.round((progressEvent.loaded * 100) / progressEvent.total);
            console.log(completed);
            callback(completed);
          }
        }).then((response) => {
        this.showProgress = false;
        this.saveFile(response);        
      });
    },
    saveFile(response) {
      this.showBusy();
      var data = response.data ? response.data : response.data.data;
      var blob = this.b64toBlob(data, "", 1024);
      var link = document.createElement("a");
      link.href = window.URL.createObjectURL(blob);
      link.download = response.data.fileName;
      link.click();
      this.hideBusy();
    },
    b64toBlob(b64Data, contentType, sliceSize) {
      contentType = contentType || "";
      sliceSize = sliceSize || 512;

      var byteCharacters = atob(b64Data);
      var byteArrays = [];

      for (
        var offset = 0;
        offset < byteCharacters.length;
        offset += sliceSize
      ) {
        var slice = byteCharacters.slice(offset, offset + sliceSize);

        var byteNumbers = new Array(slice.length);
        for (var i = 0; i < slice.length; i++) {
          byteNumbers[i] = slice.charCodeAt(i);
        }

        var byteArray = new Uint8Array(byteNumbers);

        byteArrays.push(byteArray);
      }

      var blob = new Blob(byteArrays, { type: contentType });
      return blob;
    },
    deleteSelectedFiles() {
      this.showBusy();
      this.hideDeleteModal();
      var files = this.files.filter((f) => {
        return f.isSelected;
      });

      const body = {
        fileIds: files.map((file) => file.id),
      };

      axios.post("/api/file/delete", body).then(() => {
        this.loadFiles().then(() => this.hideBusy());
      });
      // Remove files
    },
    thumbnailFromId(id) {
      return "/api/file/thumbnail/" + id;
    },
    selectFiles() {
      document.getElementById("files").click();
    },
    addFiles(event) {
      this.uploadData(event.target.files);
    },
    updateCompleted(c) {
      this.percentCompleted = c;
    },
    uploadData(files) {
      this.showProgress = true;      
      var formData = new FormData();
      for (var i = 0; i < files.length; i++) {
        let file = files[i];
        formData.append("files[" + i + "]", file, file.name);
      }
      var callback = this.updateCompleted;
      axios
        .put("/api/file/uploadfiles", formData, {
          headers: {
            'folderId': `${this.folderId}`,
            'Content-Type': 'multipart/form-data'
          },
          onUploadProgress: function(progressEvent) {
            let completed = Math.round((progressEvent.loaded * 100) / progressEvent.total);
            callback(completed);
          }
        })
        .catch(err => {
          alert("ERR " + err.response + ", status: " + err.response.status + ", data: " + err.response.data);
          this.showProgress = false;
        })
        .then(() => {
          this.loadFiles();
          this.showProgress = false;
          this.percentCompleted = 0;
        });
    },
  },
};
</script>

<style scoped>
.btn-forsvik {
  background-color: gray;
  border-color: white;
  color: whitesmoke;
}
.centerprogress {
            width:200px;
            height:200px;
            position: fixed;            
            top: 50%;
            left: 50%;
            margin-top: -100px;
            margin-left: -100px;
        }
</style>
