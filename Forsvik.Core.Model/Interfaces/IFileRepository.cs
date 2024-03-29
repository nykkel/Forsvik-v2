﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forsvik.Core.Model.External;

namespace Forsvik.Core.Model.Interfaces
{
    public interface IFileRepository
    {
        Task<Guid> Save(byte[] data, Guid? preparedId = null);
        Task<byte[]> Get(Guid id);
        Task Delete(Guid id);
        void SaveThumbnail(byte[] thumbnail, Guid id);
        Task<byte[]> GetThumbnail(Guid id);
        Task<byte[]> CreateCompressedFile(IEnumerable<FileModel> files);
    }
}
