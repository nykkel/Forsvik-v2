<template>
  <div>
    <div class="container-fluid mt--5">
      <div class="row">
        <div class="col-xl-12">
          <card header-classes="bg-transparent" shadow shadowSize="3">
            <template v-slot:header>
              <div class="row">
                <div class="col-md-6">
                  <h1>Sökresultat</h1>
                </div>
                <div class="col-md-6 text-right">
                </div>
              </div>
            </template>

            <div class="row">
              <table class="hoverTable">
                <tr class="heading-small text-muted mb-4">
                  <th style="width: 100px; cursor: pointer">Ikon</th>
                  <th style="width: 300px; cursor: pointer">Namn</th>
                  <th style="width: 100px; cursor: pointer">Typ</th>
                  <th style="width: 300px; cursor: pointer">Beskrivning</th>
                  <th style="width: 200px; cursor: pointer">Sök-taggar</th>
                  <th style="width: 200px">Katalog</th>
                  <th style="width: 30px"></th>
                </tr>
                <tr
                  v-for="hit in searchHits"
                  :key="hit.id"
                  v-on:dblclick="openLocation(hit)"
                >
                  <td>
                    <img :src="thumbnailFromId(hit.entityId)" height="50" />
                  </td>
                  <td>
                    <label class="forsvik-text" v-text="getFileName(hit)" />
                  </td>
                  <td style="text-transform: uppercase" class="forsvik-text">
                    {{ getType(hit.entityType) }}
                  </td>
                  <td>
                    <label class="forsvik-text" v-text="hit.description" />
                  </td>
                  <td>
                    <label class="forsvik-text" v-text="hit.tags" />
                  </td>
                  <td>
                    <label class="forsvik-text" v-text="hit.path" />
                  </td>
                  <td>
                    <div
                      title="Gå till katalog..."
                      @click="gotoFolder(hit.folderId)"
                    >
                      <i
                        class="far fa-folder-open"
                        style="font-size: 20px; cursor: pointer; color: #999"
                      ></i>
                    </div>
                  </td>
                </tr>
              </table>
            </div>
          </card>
        </div>
      </div>
    </div>
    <div id="pictureModal" class="modal">
      <span class="close-modal" @click="closeModal">&times;</span>
      <img id="modalImg" class="modal-content1" :src="getImageUrl" />
    </div>
  </div>
</template>
<script>
export default {
  data() {
    return {
      searchHits: [],
      lastSearch: null,
    };
  },
  computed: {
    query() {
      return this.$route.query.query;
    },
    category() {
      return this.$route.query.category;
    },
  },
  watch: {
    query() {
      this.search();
    },
    category() {
      this.search();
    },
  },

  methods: {
    thumbnailFromId(id) {
      return "/api/file/thumbnail/" + id;
    },
    getType(entityType) {
      if (entityType === 0) {
        return "Katalog";
      } else {
        return "Fil";
      }
    },
    getFileName(hit) {
      if (hit.entityType === 1) {
        return hit.name + "." + hit.extension;
      }
      return hit.name;
    },
    search() {
      let hash = this.query + this.category;
      if (hash === this.lastSearch) {
        return;
      }
      this.lastSearch = this.query + this.category;

      fetch(
        "/api/archive/search?query=" + this.query + "&category=" + this.category
      )
        .then((response) => response.json())
        .then((hits) => {
          this.searchHits = hits;
        });
    },
    gotoFolder(folderId) {
      this.$router.push({ name: "folder", query: { id: folderId } });
    },
    openLocation(hit) {
      const ext = hit.extension ? hit.extension.toLowerCase() : "";

      if (ext !== "jpg" && ext !== "jpeg" && ext !== "png") {
        this.gotoFolder(hit.folderId);
        return;
      }

      var modal = document.getElementById("pictureModal");
      var modalImg = document.getElementById("modalImg");

      modal.style.display = "block";
      modalImg.src = "/api/file/resource/" + hit.entityId;
    },
    closeModal() {
      var modal = document.getElementById("pictureModal");
      modal.style.display = "none";
    },
  },
  mounted() {
    this.search();
  },
};
</script>
