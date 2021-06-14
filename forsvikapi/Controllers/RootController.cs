using Forsvik.Core.Database;
using Forsvik.Core.Database.Repositories;
using Forsvik.Core.Model.External;
using Forsvik.Core.Model.Interfaces;
using forsvikapi.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace forsvikapi.Controllers
{
    public class RootController : ControllerBase
    {
        protected ArchivingRepository Repository;
        protected DocumentRepository DocumentRepository;
        protected ILogService LogService;

        public RootController(
            ILogService logService,
            IWebHostEnvironment hostingEnvironment,
            ArchivingRepository archivingRepository,
            DocumentRepository documentRepository)
        {
            ServerHost.PublicPath = Path.Combine(hostingEnvironment.WebRootPath ?? hostingEnvironment.ContentRootPath, "public");

            LogService = logService;
            Repository = archivingRepository;
            DocumentRepository = documentRepository;
        }
                
        protected FileContentResult CreateResponseContent(byte[] data, string filename, string extension)
        {
            if (data == null) return null;
            {
                return new FileContentResult(data, GetFileType(extension));
            }
        }

        protected string GetFileType(string extension)
        {
            var type = "application/octet-stream";

            switch (extension.ToLower())
            {
                case "jpg":
                case "jpeg":
                    type = "image/jpeg"; break;
                case "png": type = "image/png"; break;
                case "gif": type = "image/gif"; break;
                case "svg": type = "image/svg+xml"; break;
                case "txt":
                case "csv": type = "text/plain"; break;
                case "doc":
                    type = "application/msword"; break;
                case "docx":
                    type = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; break;
                case "xls":
                    type = "application/vnd.ms-excel"; break;
                case "xlsx":
                    type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; break;
                case "ppt":
                    type = "application/vnd.ms-powerpoint"; break;
                case "pptx":
                    type = "application/vnd.openxmlformats-officedocument.presentationml.presentation"; break;
                case "pdf":
                    type = "application/pdf";
                    break;

            }
            return type;
        }
    }
}
