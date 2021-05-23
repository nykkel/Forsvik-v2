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
  },
};
