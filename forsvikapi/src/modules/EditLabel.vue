<style>
.descDiv:hover {
  border-radius: 3px;
  background: #f1f1f1;
  border: 1px solid lightgray;
}
.wrapper .spanleft {
  position: relative;
  float: right;
  top: 2px;
  color: green;
  cursor: pointer;
}
.wrapper {
  position: relative;
}
</style>
<template>
  <div style="vertical-align: top">
    <div
      style="height: 30px"
      v-show="!editMode"
      class="descDiv text-muted"
      @click="setEditMode"
    >
    <div class="forsvik-text" style="font-size:14px;max-height:50px;overflow-y: hidden">{{ inputText }}&nbsp;</div>
    </div>
    <div v-show="editMode" class="wrapper">
      <textarea
        class="form-control"
        rows="2"
        :id="internalId"
        style="position: relative; font-size: 14px"
        v-model="boundText"
        autofocus
        :placeholder="ghostText"
      ></textarea>
      <div
        class="spanleft"
        style="margin-top: -25px; margin-right: 25px; color: black"
        @click="cancel"
      >
        <i class="fas fa-times" title="Avbryt"></i>
      </div>
      <div class="spanleft" style="margin-top: -25px" @click="saveChanges()">
        <i class="fas fa-check" title="Spara"></i>
      </div>
    </div>
  </div>
</template>
<script>
import CommonMixin from "../mixins/commonMixin";
export default {
  mixins: [CommonMixin],
  name: "edit-label",
  props: {
    itemId: String,
    inputText: String,
    ghostText: String,
    isReadOnly: Boolean
  },
  data() {
    return {
      editMode: false,
      boundText: null,
      internalId: null,
    };
  },
  mounted() {
    this.internalId = this.guidGenerator();
  },
  methods: {
    setEditMode() {      
      if (this.isReadOnly) {        
       return;
      }
      this.editMode = true;
      this.boundText = this.inputText;
      document.getElementById(this.internalId).focus();
    },
    saveChanges() {
      this.editMode = false;
      this.$emit("saveChanges", { id: this.itemId, text: this.boundText });
    },
    cancel() {
      this.editMode = false;
      this.boundText = this.inputText;
    },
    guidGenerator() {
      var S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
      };
      return (
        S4() +
        S4() +
        "-" +
        S4() +
        "-" +
        S4() +
        "-" +
        S4() +
        "-" +
        S4() +
        S4() +
        S4()
      );
    },
  },
};
</script>
<style></style>
