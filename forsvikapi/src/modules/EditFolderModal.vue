<style scoped>
.maskimg {
  opacity: 0.4;
  /* Note that percentages work as well as px */
}
</style>
<template>
  <div>
    <modal v-model:show="show" @close="close" body-classes="p-3">
      <template v-slot:header>
        <h2 class="heading-small text-muted mb-4">{{ title }}</h2>
      </template>
      <div>
        <div
          class="row"
          style="margin-top: -40px; margin-left: 10px; margin-right: 10px"
        >
          <div class="col-md-12">
            <label class="form-control-label">Namn</label>
            <div class="row">
              <input
                type="text"
                class="form-control forsvik-text ml-3 mr-3"
                v-model="folder.name"
              />
            </div>
          </div>
        </div>

        <div class="row mt-4">
          <div class="col-md-12 text-center">
            <label class="form-control-label"
              >Klicka i bilden för att ändra...(jpeg/jpg)</label
            >
            <div>
              <img
                :src="imageUrl"
                :class="{ maskimg: folder.imageFileId == null }"
                id="logoimg"
                width="300"
                height="200"
                v-on:click="selectFile"
              />
              <div style="background-color: white" />
            </div>
            <input
              type="file"
              id="fileUpload"
              name="fileUpload"
              style="display: none"
              onclick="value = null"
              @change="filesChange"
              accept="image/*"
            />
            <!-- </form> -->
          </div>
        </div>
        <div class="row mt-4 ml-2 mr-2">
          <div class="col-md-12">
            <label class="form-control-label">Beskrivning</label>
            <textarea
              label="Beskrivning"
              class="form-control forsvik-text"
              rows="3"
              v-model="folder.description"
              placeholder="Skriv en beskrivning om arkivets innehåll ..."
            ></textarea>
          </div>
        </div>
      </div>
      <template v-slot:footer>
        <table>
          <tr>
            <td>
              <base-button
                v-if="folder.id && folder.id !== null"
                type="secondary"
                style="margin-right: 20px"
                @click="removeFolder"
                >Ta bort</base-button
              >
            </td>
            <td>
              <input
                type="text"
                class="form-control mr-5"
                placeholder="Pos"
                v-model="folder.index"
                style="width: 50px"
              />
            </td>
            <td>
              <base-button
                type="secondary"
                style="margin-right: 10px"
                @click="close"
                >Stäng</base-button
              >
            </td>
            <td>
              <button
                type="button"
                class="btn btn-default"
                v-on:click="saveFolder"
              >
                Spara
              </button>
            </td>
          </tr>
        </table>
      </template>
    </modal>
    <modal v-model:show="showDelete">
      <template v-slot:header>
        <h2 class="heading-small text-muted">Ta bort</h2>
      </template>
      <h4>
        Vill du ta bort katalogen och alla underliggade filer och kataloger?
      </h4>
      <template v-slot:footer>
        <base-button type="secondary" @click="showDelete = false"
          >Stäng</base-button
        >
        <button type="button" class="btn btn-default" @click="doRemoveFolder">
          Ta bort
        </button>
      </template>
    </modal>
  </div>
</template>

<script>
import axios from "axios";

export default {
  name: "edit-folder-modal",
  props: {
    isOpen: Boolean,
    folderId: String,
    newFolderParentId: String,
  },
  computed: {
    imageUrl() {
      return "img/theme/placeholder.jpg";
    },
    title() {
      return this.folder.parentFolderId ? "Spara katalog" : "Spara arkiv";
    },
  },
  watch: {
    isOpen(value) {
      if (value === true) {
        this.entered();
      }
    },
  },
  data() {
    return {
      show: false,
      folder: {
        name: null,
        description: null,
        imageFileId: null,
        parentFolderId: null,
        index: 1,
      },
      showDelete: false,
      logo: "img/brand/archives-icon.jpg",
    };
  },
  methods: {
    entered() {
      this.show = true;
      if (this.newFolderParentId) {
        this.folder.parentFolderId = this.newFolderParentId;
      }
      if (this.folderId) {
        fetch("/api/archive/getfolder/" + this.folderId)
          .then((response) => response.json())
          .then((data) => {
            this.folder = data;

            if (data.imageFileId) {
              let url = "/api/file/resource/" + data.imageFileId;

              const img = document.getElementById("logoimg");
              img.addEventListener("load", () =>
                URL.revokeObjectURL(this.imageUrl)
              );
              img.src = url;
              img.style.opacity = 1;
            } else {
              this.clear();
            }
          });
      } else {
        this.clear();
      }
    },
    doRemoveFolder() {
      this.$emit("deleteFolder", this.folder.id);
      this.showDelete = false;
      this.close();
    },
    clear() {
      this.folder.name = null;
      this.folder.id = undefined;
      this.folder.description = null;
      this.folder.imageFileId = null;
      this.folder.index = 1;
      document.getElementById("logoimg").src = this.imageUrl;
    },
    saveFolder() {
      axios.post("/api/archive/savefolder", this.folder).then(() => {
        this.close();
      });
    },
    close() {
      this.show = false;
      this.$emit("closed");
    },
    removeFolder() {
      this.showDelete = true;
    },
    selectFile() {
      document.getElementById("fileUpload").click();
    },
    filesChange(event) {
      if (event.target.files.length == 1) {
        this.uploadData(event.target.files[0]);
      }
    },
    uploadData(file) {
      var formdata = new FormData();

      formdata.append("file", file, file.name);

      axios
        .post("/api/file/uploadfile", formdata, {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        })
        .catch((err) => {
          alert(
            "ERR " +
              err.response +
              ", status: " +
              err.response.status +
              ", data: " +
              err.response.data
          );
        })
        .then((r) => {
          console.log("uploaded image id", r.data.result);
          this.folder.imageFileId = r.data.result;
          let url = "/api/file/resource/" + r.data.result;
          document.getElementById("logoimg").src = url;
        });
    },
  },
};
</script>
