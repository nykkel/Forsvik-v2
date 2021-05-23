module.exports = {
  outputDir: "./app",
  productionSourceMap: false,
  chainWebpack: (config) => {
    config.plugins.delete("friendly-errors");
  },
};
