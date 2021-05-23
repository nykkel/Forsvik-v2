<template>
  <modal v-model:show="show" @close="close">
    <template v-slot:header>
      <h2 class="heading-small text-muted mb-4">Lägg till ny katalog</h2>
    </template>
    <div class="row" style="width: 100%; margin-top: -40px">
      <div class="col-md-12">
        <label class="form-control-label">Namn</label>
      </div>
      <div class="row" style="width: 100%">
        <div class="col-md-12">
          <input
            placeholder="Katalognamn"
            type="text"
            class="form-control ml-2"
            v-model="folderName"
          />
        </div>
      </div>
    </div>
    <template v-slot:footer>
      <base-button type="secondary" @click="close">Close</base-button>
      <button type="button" class="btn btn-default" v-on:click="saveFolder">
        Spara
      </button>
    </template>
  </modal>
</template>

<script>
import axios from "axios";

export default {
  name: "add-folder-modal",
  props: {
    isOpen: Boolean,
    parentFolderId: String,
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
      folderName: null,
    };
  },
  methods: {
    entered() {
      this.show = true;
      this.folderName = null;
    },
    saveFolder() {
      const body = {
        ParentFolderId: this.parentFolderId,
        Name: this.folderName,
      };
      axios.post("/api/archive/addFolder", body).then(() => {
        this.close();
      });
    },
    close() {
      this.show = false;
      this.$emit("closed");
    },
  },
};
</script>
