using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Forsvik.Core.Model.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Forsvik.Core.Model.External;
using System.IO.Compression;
using System.Linq;
using Forsvik.Core.Model.Context;
using File = System.IO.File;

namespace Forsvik.Core.Database.Repositories
{
    public class DocumentFileRepository : IFileRepository
    {
        private const string _ext = ".dat";
        private const string _tmb = ".tmb";
        private const string Tag = "Tag";

        private ILogService LogService;
        private readonly ResourceDocumentStore _store;

        public DocumentFileRepository(ILogService logService, ResourceDocumentStore store)
        {
            LogService = logService;
            _store = store;
        }

        private KeyValue KeyTag(string value)
        {
            return new KeyValue{Key = Tag, Value = value};
        }

        public async Task Delete(Guid id)
        {
            try
            {
                var file = id.ToString() + _ext;

                var documentId = await _store.Exists<byte[]>(KeyTag(file));
                if (documentId != null)
                {
                    await _store.Remove(documentId.Value);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message);
                throw;
            }
        }

        public async Task<byte[]> Get(Guid id)
        {
            var file = id.ToString() + _ext;

            var data = await _store.Get<byte[]>(KeyTag(file));

            return data;
        }

        public async Task<byte[]> GetThumbnail(Guid id)
        {
            var file = id.ToString() + _tmb;
            var data = await _store.Get<byte[]>(KeyTag(file));

            return data;
        }

        public async Task<byte[]> CreateCompressedFile(IEnumerable<FileModel> files)
        {
            await TryClearTempFolder();

            var zipFile = $"{GetTempStore()}{Guid.NewGuid()}.zip";

            using (var outputStream = new FileStream(zipFile, FileMode.Create))
            {
                using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var zipEntry = archive.CreateEntry(file.FileName, CompressionLevel.Optimal);

                        using (var entryStream = zipEntry.Open())
                        {
                            using (var fileToCompressStream = new FileStream(GetReference(file.Id), FileMode.Open))
                            {
                                fileToCompressStream.CopyTo(entryStream);
                            }
                        }
                    }
                }
            }

            return await File.ReadAllBytesAsync(zipFile);
        }

        public async Task<Guid> Save(byte[] data, Guid? preparedId = null)
        {
            var id = preparedId ?? Guid.NewGuid();

            var file = id.ToString() + _ext;
            await _store.Save(data, file);

            return id;
        }

        public async void SaveThumbnail(byte[] thumbnail, Guid id)
        {
           var file = id.ToString() + _tmb;
            
           await _store.Save(thumbnail, file);
        }

        public string GetReference(Guid id)
        {
            var path = GetStore();
            var file = path + id + _ext;

            if (File.Exists(file))
                return file;

            throw new Exception("File reference dont exist, " + file);
        }

        public async Task<int> MigrateFilesToDb()
        {
            try
            {
                var fileRepo = new DiskFileRepository(LogService);
                var files = fileRepo.ListFiles().ToList();
                var cnt = 0;

                foreach (var filePath in files)
                {
                    var file = Path.GetFileName(filePath);
                    cnt++;

                    if (await _store.ExistsIndex(KeyTag(file)))
                        continue;

                    var split = file.Split('.');
                    var id = new Guid(split.First());
                    var data = split[1] == "dat"
                        ? await fileRepo.Get(id)
                        : await fileRepo.GetThumbnail(id);

                    await _store.Save(data, file);
                }

                return files.Count();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private string GetTempStore()
        {
            var path = ServerHost.PublicPath;
            path = Path.Combine(path, "temp");
            if (path.Last() != '\\')
                path += '\\';

            return path;
        }

        private string GetStore()
        {
            var path = ServerHost.PublicPath;
            path = Path.Combine(path, "archive");
            if (path.Last() != '\\')
                path += '\\';

            return path;
        }

        private async Task TryClearTempFolder()
        {
            var folder = GetTempStore();
            try
            {
                await Task.Factory.StartNew(() =>
                {
                    foreach (var file in Directory.EnumerateFiles(folder))
                    {
                        File.Delete(file);
                    }
                });
            }
            catch
            {
                // Accept failure to remove file in use
            }
        }

    }
}
