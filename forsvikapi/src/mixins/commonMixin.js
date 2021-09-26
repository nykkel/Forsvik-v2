export default {
  data() {
    return {
      hello: true,
    };
  },
  methods: {
    showBusy() {
      this.emitter.emit("show-busy", true);
    },
    hideBusy() {
      this.emitter.emit("show-busy", false);
    },
    truncateString(str, num) {
      if (!str) return "";
      if (str.length <= num) {
        return str
      }
      return str.slice(0, num) + '...'
    }
  },
};
