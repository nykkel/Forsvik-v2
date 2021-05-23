using Forsvik.Core.Model.External;
using Forsvik.Core.Model.Interfaces;
using System;
using System.IO;
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
        
        public async Task<byte[]> Get(Guid id)
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    var path = GetStore();
                    var file = path + id.ToString() + _ext;

                    return File.ReadAllBytes(file);
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message);
                    throw;
                }
            });
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
