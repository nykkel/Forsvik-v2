import axios from "axios";

export default {
  data() {
    return {
      showProgress: false,
    };
  },
  methods: {
      saveFile(blob, fileName) {
          var fileURL = window.URL.createObjectURL(new Blob([blob]));
          var fileLink = document.createElement('a');
          fileLink.href = fileURL;
          fileLink.setAttribute('download', fileName);
          document.body.appendChild(fileLink);
    
          fileLink.click();
      },
      downloadFile(fileId) {
          this.showProgress = true;
    
          const body = {
            fileIds: [fileId],
          };
    
          axios
            .post("api/file/fileinfo", body)
              .then(response => {
                  let fileLength = response.data.fileLength;
                  let fileName = response.data.fileName;
                  axios
                  ({
                      url: "/api/file/resources",
                      method:'POST',
                      responseType: 'blob',
                      data: body,
                      onDownloadProgress: function (progressEvent) {
                      let completed = Math.round(
                        (progressEvent.loaded * 100) / fileLength
                      );
                      completed = completed > 100 ? 100 : completed;
                      console.log(completed);
                      //callback(completed);
                    },
                  })              
                  .then((response) => {
                    this.showProgress = false;
                    this.saveFile(response.data, fileName);
                  })
                  .catch(err => {
                    console.log(err);
                  })
                })
        },
  },
};
  