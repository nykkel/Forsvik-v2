using Forsvik.Core.Database;
using Forsvik.Core.Database.Repositories;
using Forsvik.Core.Model.External;
using Forsvik.Core.Model.Interfaces;
using forsvikapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace forsvikapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ErrorHandlingFilter]
    public class FileController : RootController
    {
        public FileController(
            ILogService logService,
            IWebHostEnvironment hostingEnvironment,
            ArchivingRepository archivingRepository,
            DocumentRepository documentRepository) : base(logService, hostingEnvironment, archivingRepository, documentRepository)
        { }

        [HttpGet]
        [Route("resource/{id}")]
        public FileStreamResult Resource(Guid id)
        {
            var file = Repository.GetFile(id).Result;

            if (file != null)
            {
                return CreateResponseStream(file.Data, file.Name, file.Extension);
            }
            
            return null;
        }

        [HttpGet]
        [Route("thumbnail/{id}")]
        public FileStreamResult Thumbnail(Guid id)
        {
            var file = Repository.GetThumbnail(id).Result;

            if (file != null)
            {
                return CreateResponseStream(file.Data, file.Name, file.Extension);
            }

            return null;
        }

        [HttpPost]
        [Authorize]
        [Route("uploadfile")]
        public async Task<ActionResult<ResponseModel<Guid>>> UploadFile(IFormFile file)
        {
            try
            {
                if (file.Length == 0) return new ResponseModel<Guid> { Result = Guid.Empty };
                var result = await Task.Factory.StartNew(() =>
                {
                    MemoryStream target = new MemoryStream();
                    file.OpenReadStream().CopyTo(target);
                    byte[] data = target.ToArray();

                    var r = Repository.AddOrUpdateFile(file.FileName, data, null);
                    LogService.Info("All uploaded");
                    return r;
                });
                return new ResponseModel<Guid> { Result = result };
            }
            catch(Exception ex)
            {
                LogService.Error(ex.Message);
                return new ResponseModel<Guid> { Result = Guid.Empty, Error = ex.Message };
            }
        }

        [HttpPut]
        [Authorize]
        [Route("uploadfiles")]
        public async Task<ActionResult<ResponseModel<bool>>> UploadFiles(IFormCollection fileCollection)
        {
            HttpContext.Request.Headers.TryGetValue("FolderId", out var folderId);
            
            try
            {
                var ok = await Task.Factory.StartNew(() =>
                {
                    foreach (var file in fileCollection.Files)
                    {
                        using (MemoryStream target = new MemoryStream())
                        {
                            file.OpenReadStream().CopyTo(target);
                            byte[] data = target.ToArray();

                            Repository.AddOrUpdateFile(file.FileName, data, new Guid(folderId.ToString()));
                        }
                    }
                    return true;
                });
                return new ResponseModel<bool>{Result = true};
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
                return new ResponseModel<bool> { Error = ex.Message};
            }
        }

        [HttpPost]
        [Authorize]
        [Route("delete")]
        public async Task<ActionResult<bool>> Delete(FilesRequest request)
        {
            if (request.FileIds.Count > 0)
            {
                await Task.Factory.StartNew(() => Repository.DeleteFiles(request.FileIds));
            }
            return true;
        }

        [HttpPost]
        [Authorize]
        [Route("removefolder")]
        public async Task<ActionResult<bool>> RemoveFolder(RemoveFolderModel model)
        {
            await Repository.RemoveFolder(model.FolderId);
            return true;
        }

        [HttpPost]
        [Route("filesize")]
        public async Task<ActionResult<int>> FileSize(FilesRequest request)
        {
            var total = await Task.Factory.StartNew(() => request
                .FileIds
                .Select(id => Repository.GetFileSize(id))
                .Sum());

            return total * 1024;            
        }

        [HttpPost]
        [Route("resources")]
        public async Task<ActionResult<DownloadModel>> Resources(FilesRequest request)
        {
            if (request.FileIds.Count == 0) return new EmptyResult();

            if (request.FileIds.Count == 1)
            {
                var file = await Repository.GetFile(request.FileIds.First());
                HttpContext.Response.Headers.Add("File-Length", file.Data.Length.ToString());

                return new DownloadModel
                {
                    Data = Convert.ToBase64String(file.Data),
                    FileName = file.Name + "." + file.Extension
                };
            }

            // Multiple files. Pack in zip..
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var fileId in request.FileIds)
                    {
                        var file = await Repository.GetFile(fileId);

                        var demoFile = archive.CreateEntry(file.FileName, CompressionLevel.Optimal);

                        using (var entryStream = demoFile.Open())
                        using (var fileToCompressStream = new MemoryStream(file.Data))
                        {
                            fileToCompressStream.CopyTo(entryStream);
                        }
                    }
                }

                var zipData = memoryStream.GetBuffer();

                return new DownloadModel
                {
                    Data = Convert.ToBase64String(zipData),
                    FileName = "Samlingsfil.zip"
                };
            }
        }
    }
}
