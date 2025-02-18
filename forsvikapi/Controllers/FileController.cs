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
using System.Net.Mime;
using System.Threading.Tasks;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using FileInfo = Forsvik.Core.Model.External.FileInfo;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            ArchivingRepository archivingRepository) : base(logService, hostingEnvironment, archivingRepository)
        { }

        [HttpGet]
        [Route("resource/{id}")]
        public FileContentResult Resource(Guid id)
        {
            var file = Repository.GetFile(id).Result;

            if (file == null) return null;

            if (IsJpg(file.Extension))
                file.Data = CompressImage(file.Data, 50);

            return CreateResponseContent(file.Data, file.Name, file.Extension);
        }

        [HttpGet]
        [Route("thumbnail/{id}")]
        public FileContentResult Thumbnail(Guid id)
        {
            var type = Repository.GetFileInfo(id).GetAwaiter().GetResult();

            FileDataModel file;

            if (type != null && IsJpg(type.Extension))
            {
                file = Repository.GetFile(id).Result;

                if (file == null) return null;
                if (file.Data.Length == 0) return null; // Metadata exists but file missing!

                file.Data = CompressImage(file.Data, 10);
                return CreateResponseContent(file.Data, file.Name, file.Extension);
            }

            file = Repository.GetThumbnail(id).Result;

            if (file != null)
            {
                return CreateResponseContent(file.Data, file.Name, file.Extension);
            }

            return null;
        }

        [HttpPost]
        [Route("uploadfile")]
        public async Task<ActionResult<ResponseModel<Guid>>> UploadFile(IFormFile file)
        {
            try
            {
                LogService.Info($"SUPLOAD: 1");

                if (file.Length == 0) return new ResponseModel<Guid> { Result = Guid.Empty };
                LogService.Info($"SUPLOAD: 2");

                MemoryStream target = new MemoryStream();
                file.OpenReadStream().CopyTo(target);
                target.Position = 0;
                DateTime? date = null;
                try
                {
                    date = ExtractMetaData(target);
                }
                catch (Exception e)
                {
                    LogService.Error("Error extractid data " + e);
                }
                target.Position = 0;
                byte[] data = target.ToArray();
                LogService.Info($"SUPLOAD: 3");

                var result = await Repository.AddOrUpdateFile(file.FileName, data, null, date);
                LogService.Info("All uploaded");

                LogService.Info($"SUPLOAD: 4");

                return new ResponseModel<Guid> { Result = result };
            }
            catch(Exception ex)
            {
                LogService.Error(ex.Message);
                return new ResponseModel<Guid> { Result = Guid.Empty, Error = ex.Message };
            }
        }

        private static DateTime? ExtractMetaData(Stream stream)
        {
            var directories = ImageMetadataReader.ReadMetadata(stream);

            // access the date time
            var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            var dateTime = subIfdDirectory?.Tags.FirstOrDefault(x => x.Name.StartsWith("Date/Time"));

            if (dateTime != null)
            {
                var parts = dateTime.Description!.Split(' ');
                var date = parts[0].Replace(':', '-');
                var time = parts[1];
                return DateTime.Parse($"{date} {time}");
            }

            return null;
        }

        [HttpPost]
        [Route("uploadfiles")]
        public async Task<ActionResult<ResponseModel<bool>>> UploadFiles(IFormCollection fileCollection)
        {
            HttpContext.Request.Headers.TryGetValue("FolderId", out var folderId);
            LogService.Info($"UPLOAD: 1");
            try
            {
                foreach (var file in fileCollection.Files)
                {
                    LogService.Info($"UPLOAD: 2");

                    using (MemoryStream target = new MemoryStream())
                    {
                        file.OpenReadStream().CopyTo(target);
                        target.Position = 0;
                        DateTime? date = null;
                        try
                        {
                            date = ExtractMetaData(target);
                        }
                        catch (Exception e)
                        {
                            LogService.Error("Error extractid data " + e);
                        }
                        target.Position = 0;
                        byte[] data = target.ToArray();

                        await Repository.AddOrUpdateFile(file.FileName, data, new Guid(folderId.ToString()), date);
                        LogService.Info($"UPLOAD: 3");
                    }
                }

                return new ResponseModel<bool>{Result = true};
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
                return new ResponseModel<bool> { Error = ex.Message};
            }
        }

        [HttpPost]
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
        [Route("removefolder")]
        public async Task<ActionResult<bool>> RemoveFolder(RemoveFolderModel model)
        {
            await Repository.RemoveFolder(model.FolderId);
            return true;
        }

        [HttpPost]
        [Route("fileinfo")]
        public async Task<ActionResult<FileInfo>> FileInfo(FilesRequest request)
        {
            var fileName = request.FileIds.Count == 1
                ? Repository.GetFileName(request.FileIds.First())
                : "FileCollection.zip";

            var total = await Task.Factory.StartNew(() => request
                .FileIds
                .Select(id => Repository.GetFileSize(id))
                .Sum());

            return new FileInfo
            {
                FileLength = total * 1024,
                FileName = fileName
            };
        }

        [HttpPost]
        [Route("resources")]
        public async Task<FileContentResult> Resources(FilesRequest request)
        {
            if (request.FileIds.Count == 0) return null;

            if (request.FileIds.Count == 1)
            {
                var file = await Repository.GetFile(request.FileIds.First());

                return new FileContentResult(file.Data, GetFileType(file.Extension));
            }
            
            // Multiple files. Pack in zip..
            var zipData = await Repository.CreateZipFile(request.FileIds);
            return CreateResponseContent(zipData, "FileCollection", "Zip");
        }


    }
}
