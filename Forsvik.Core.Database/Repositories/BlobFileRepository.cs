//using Azure.Storage.Blobs;
//using Azure.Storage.Blobs.Models;
//using Forsvik.Core.Model.Interfaces;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using System.Threading.Tasks;
//using Forsvik.Core.Model.External;

//namespace Forsvik.Core.Database.Repositories
//{
//    public class BlobFileRepository : IFileRepository
//    {
//        private const string _ext = ".dat";
//        private const string _tmb = ".tmb";

//        private readonly IConfiguration Configuration;
//        private ILogService LogService;

//        public BlobFileRepository(IConfiguration configuration, ILogService logService)
//        {
//            Configuration = configuration;
//            LogService = logService;
//        }

//        public BlobContainerClient GetContainer()
//        {
//            var blobServiceClient = new BlobServiceClient(Configuration.GetConnectionString("ForsvikBlob"));

//            return blobServiceClient.GetBlobContainerClient("forsvikstorage");
//        }

//        public async Task Delete(Guid id)
//        {
//            try
//            {
//                var file = id.ToString() + _ext;

//                var container = GetContainer();
//                var blobClient = container.GetBlobClient(file);

//                if (blobClient.Exists())
//                    await blobClient.DeleteAsync();
//            }
//            catch (Exception ex)
//            {
//                LogService.Error(ex.Message);
//                throw;
//            }
//        }

//        public async Task<byte[]> Get(Guid id)
//        {
//            try
//            {
//                var file = id.ToString() + _ext;

//                var container = GetContainer();
//                var blobClient = container.GetBlobClient(file);

//                if (blobClient.Exists())
//                {
//                    BlobDownloadInfo download = await blobClient.DownloadAsync();

//                    using (MemoryStream stream = new MemoryStream())
//                    {
//                        await download.Content.CopyToAsync(stream);
//                        return stream.GetBuffer();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                LogService.Error(ex.Message);
//                throw;
//            }
//            return null;
//        }

//        public async Task<byte[]> GetThumbnail(Guid id)
//        {
//            try
//            {
//                var file = id.ToString() + _tmb;

//                var container = GetContainer();
//                var blobClient = container.GetBlobClient(file);
                
//                if (blobClient.Exists())
//                {
//                    BlobDownloadInfo download = await blobClient.DownloadAsync();

//                    using (MemoryStream stream = new MemoryStream())
//                    {
//                        await download.Content.CopyToAsync(stream);
//                        return stream.GetBuffer();
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                LogService.Error(ex.Message);
//                throw;
//            }
//            return null;

//        }

//        public Task<byte[]> CreateCompressedFile(IEnumerable<FileModel> files)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task<Guid> Save(byte[] data, Guid? preparedId = null)
//        {
//            var id = preparedId ?? Guid.NewGuid();

//            try
//            {
//                var file = id.ToString() + _ext;

//                var container = GetContainer();
//                var blobClient = container.GetBlobClient(file);
//                using (var stream = new MemoryStream(data))
//                {
//                    await blobClient.UploadAsync(stream, true);                    
//                }
//            }
//            catch (Exception ex)
//            {
//                LogService.Error(ex.Message);
//                throw;
//            }
//            return id;
//        }

//        public async void SaveThumbnail(byte[] thumbnail, Guid id)
//        {
//            try
//            {
//                var file = id.ToString() + _tmb;

//                var container = GetContainer();
//                var blobClient = container.GetBlobClient(file);
//                using (var stream = new MemoryStream(thumbnail))
//                {
//                    await blobClient.UploadAsync(stream, true);
//                }
//            }
//            catch (Exception ex)
//            {
//                LogService.Error(ex.Message);
//                throw;
//            }
//        }
//    }
//}
