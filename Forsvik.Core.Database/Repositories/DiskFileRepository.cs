using Forsvik.Core.Model.External;
using Forsvik.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Forsvik.Core.Database.Repositories
{
    public class DiskFileRepository : IFileRepository
    {
        private ILogService LogService;

        public DiskFileRepository(ILogService logService)
        {
            LogService = logService;
        }

        private const string _ext = ".dat";
        private const string _tmb = ".tmb";

        public async Task Delete(Guid id)
        {
            await Task.Factory.StartNew(() => 
            { 
                try
                {
                    var path = GetStore();

                    var file = path + id.ToString() + _ext;

                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message);
                    throw;
                }
            });
        }

        public async Task<Guid> Save(byte[] data, Guid? preparedId = null)
        {
            return await Task.Factory.StartNew(() =>
            {
                var id = preparedId ?? Guid.NewGuid();

                try
                {
                    var path = GetStore();

                    var file = path + id.ToString() + _ext;

                    File.WriteAllBytes(file, data);
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message);
                    throw;
                }
                return id;
            });
        }

        public async Task<byte[]> GetThumbnail(Guid id)
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    var path = GetStore();
                    var file = path + id.ToString() + _tmb;
                    if (File.Exists(file))
                        return File.ReadAllBytes(file);
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message);
                    throw;
                }
                return null;
            });
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

        public async void SaveThumbnail(byte[] data, Guid id)
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    var path = GetStore();

                    var file = path + id.ToString() + _tmb;

                    File.WriteAllBytes(file, data);
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message);
                    throw;
                }
            });
        }

        public string GetReference(Guid id)
        {
            var path = GetStore();
            var file = path + id + _ext;

            if (File.Exists(file))
                return file;

            throw new Exception("File reference dont exist, " + file);
        }

        public IEnumerable<string> ListFiles()
        {
            var path = GetStore();
            return Directory.EnumerateFiles(path);
        }

        public async Task<byte[]> Get(Guid id)
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    var path = GetStore();
                    var file = path + id + _ext;

                    if (File.Exists(file))
                        return File.ReadAllBytes(file);
#if !DEBUG
                    LogService.Error("Expected file missing: " + file);
#endif
                    return new byte[0];
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message);
                    return new byte[0];
                }
            });
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


    }
}
