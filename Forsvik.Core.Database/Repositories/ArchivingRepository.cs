﻿using Forsvik.Core.Model.Context;
using Forsvik.Core.Model.External;
using Forsvik.Core.Model.Interfaces;
using Forsvik.Core.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.EntityFrameworkCore;

namespace Forsvik.Core.Database.Repositories
{
    public class ArchivingRepository
    {
        private ArchivingContext Database;
        private IFileRepository FileRepository;
        private ILogService LogService;

        public ArchivingRepository(ILogService logService, ArchivingContext database, IFileRepository fileRepository)
        {
            Database = database;
            FileRepository = fileRepository;
            LogService = logService;
        }

        public async Task<FolderModel> GetArchive(Guid id)
        {
            var model = await Database.Folders.FindAsync(id);
            return new FolderModel().SemanticCopy(model);
        }

        public async Task<FolderModel> GetFolder(Guid id)
        {
            var model = await Database.Folders.FindAsync(id);
            return new FolderModel().SemanticCopy(model);
        }

        public async Task<List<FolderModel>> GetArchives()
        {
            var models =  await Database
                .Folders
                .Where(x => x.ParentFolderId == null)
                .OrderBy(x => x.Index)
                .ToListAsync();

            return models.Select(a => new FolderModel { Id = a.Id}.SemanticCopy(a))
                .ToList();
        }
                
        public async Task<List<FolderModel>> GetFolders(Guid parentFolderId)
        {
            var models = await Database
                .Folders
                .Where(x => x.ParentFolderId == parentFolderId)
                .OrderBy(x => x.Index)
                .ToListAsync();

            return models
                .Select(x => new FolderModel().SemanticCopy(x))
                .ToList();
        }

        public async Task<List<FileModel>> GetFiles(Guid folderId, bool? sortAsc, SearchField searchField)
        {
            var asc = sortAsc ?? true;

            var files = Database
                .Files
                .Where(x => x.FolderId == folderId);

            var models = await files
                .OrderBy(searchField.ToString(), asc)
                .ToListAsync();
            
            return models.Select(x => new FileModel().SemanticCopy(x))
                .ToList();
        }

        public void DeleteFiles(List<Guid> fileIds)
        {
            foreach (var id in fileIds)
            {
                try
                {
                    var file = Database.Files.Find(id);
                    FileRepository.Delete(id);
                    Database.Files.Remove(file);
                    Database.SaveChanges();
                    LogService.Info($"File '{file?.Name}' removed");
                }
                catch (Exception e)
                {
                    LogService.Error($"File remove failed. {e.Message}");
                }
            }
        }

        public async Task<Guid> SaveFolder(FolderModel model)
        {
            var isNew = model.Id == Guid.Empty;

            var folder = isNew ? new Folder
            {
                Id = Guid.NewGuid(),
                Name = model.Name,                
                Path = RecursePath(model.Name, model.ParentFolderId),
                ParentFolderId = model.ParentFolderId
            } 
            : await Database.Folders.FindAsync(model.Id);

            var tagsChanged = folder.Tags != model.Tags;

            var nameChanged = folder.Name != model.Name;
            folder.Name = model.Name;
            folder.Tags = model.Tags;
            folder.Index = model.Index;
            folder.Description = model.Description;
            folder.ImageFileId = model.ImageFileId;
            
            if (isNew)
                Database.Folders.Add(folder);

            ReorderFolders(folder);

            await Database.SaveChangesAsync();

            if (tagsChanged)
            {
                UpdateTags(model.Tags, folder.Id, EntityType.Folder);
            }

            if (nameChanged)
            {
                await UpdatePaths(folder);
            }
            return folder.Id;
        }

        private async Task UpdatePaths(Folder folder)
        {
            // Find root folder

            while (folder != null) 
            { 
                var parent = folder.ParentFolder;
                if (parent != null)
                    folder = parent;
                else
                    break;
            }

            RecreateFilePaths(folder, "");
            await Database.SaveChangesAsync();
        }

        private void RecreateFilePaths(Folder folder, string path)
        {            
            folder.Path = $"{path}/{folder.Name}";

            foreach (Folder f in folder.Folders)
            {
                RecreateFilePaths(f, folder.Path);
            }
        }

        private void ReorderFolders(Folder folder)
        {
            var folders = Database
                .Folders
                .Where(x => x.ParentFolderId == folder.ParentFolderId && x.Id != folder.Id)
                .OrderBy(x => x.Index)
                .ToList();

            var inx = 1;
            for (int i=0; i < folders.Count; i++)
            {
                if (inx == folder.Index)
                {
                    inx++;
                }

                folders[i].Index = inx;
                inx++;
            }
        }

        public async Task SaveFileChanges(FileModel model)
        {
            var file = await Database.Files.FindAsync(model.Id);
            var tagsChanged = file.Tags != model.Tags;

            file.Description = model.Description;
            file.Name = model.Name;
            file.Tags = model.Tags;
            
            await Database.SaveChangesAsync();

            if (tagsChanged)
            {
                UpdateTags(model.Tags, file.Id, EntityType.File);
            }
        }

        public async Task RemoveFolder(Guid folderId)
        {
            var folder = Database.Folders.Find(folderId);
            await RemoveFolderRecursive(folder);
            
            Database.SaveChanges();
        }

        private async Task RemoveFolderRecursive(Folder folder)
        {
            // Remove image
            if (folder.ImageFileId.HasValue)
            {
                await FileRepository.Delete(folder.ImageFileId.Value);
            }

            // Remove files
            var files = Database.Files.Where(x => x.FolderId == folder.Id).ToList();
            foreach (var file in files)
            {
                await FileRepository.Delete(file.Id);
                Database.Files.Remove(file);
            }

            // Remove sub folders
            var folders = Database
                .Folders
                .Where(x => x.ParentFolderId == folder.Id)
                .ToList();

            foreach (var subFolder in folders)
            {
                await RemoveFolderRecursive(subFolder);
            }

            Database.Folders.Remove(folder);
        }

        public async Task<Guid> AddFolder(AddFolderModel model)
        {
            var folder = new Folder
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Path = RecursePath(model.Name, model.ParentFolderId),
                ParentFolderId = model.ParentFolderId
            };

            Database.Folders.Add(folder);
            await Database.SaveChangesAsync();

            return folder.Id;
        }

        private string RecursePath(string name, Guid? parentFolderId)
        {
            var path = name;
            while(true)
            {
                var parent = Database.Folders.Find(parentFolderId);
                if (parent != null)
                {
                    path = parent.Name + "/" + path;
                    parentFolderId = parent.ParentFolderId;
                }
                else
                {
                    break;
                }
            }
            path = "//" + path;

            return path;
        }

        public async Task<Guid> AddOrUpdateFile(string fileName, byte[] data, Guid? folderId, DateTime? created = null)
        {
            var name = Path.GetFileNameWithoutExtension(fileName);
            var extension = Path.GetExtension(fileName).TrimStart('.');
                        
            if (!name.HasValue() || !extension.HasValue())
                throw new Exception("Incorrect filename: " + fileName);

            Core.Model.Context.File file;

            var fileId = Exists(fileName, folderId);
            if (fileId != null)
            {
                file = await Database.Files.FindAsync(fileId);
                file!.Size = (int)(Convert.ToDouble(data.Length) / 1024);                
            }
            else
            {                
                file = new Core.Model.Context.File
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Size = (int)(Convert.ToDouble(data.Length) / 1024),
                    Extension = extension,
                    FolderId = folderId,
                    Created = created ?? DateTime.Now,
                    RetrievedCreatedFromImage = created != null
                };
            }
            
            await FileRepository.Save(data, file.Id);

            if (file.Extension.Equals("jpg", StringComparison.OrdinalIgnoreCase) ||
                file.Extension.Equals("jpeg", StringComparison.OrdinalIgnoreCase))
            {
                var thumbnail = MakeThumbnail(data, 100, 100);
                FileRepository.SaveThumbnail(thumbnail, file.Id);
            }

            if (fileId == null)
                Database.Files.Add(file);

            await Database.SaveChangesAsync();

            return file.Id;
        }

        private Guid? Exists(string fileName, Guid? folderId)
        {
            if (!fileName.HasValue()) return null;
            
            var parts = fileName.Split('.');
            var name = parts[0];
            var ext = parts[1];
            
            var file = Database
                .Files
                .FirstOrDefault(x => x.FolderId == folderId && x.Name == name && x.Extension == ext);

            return file?.Id;
        }

        public async Task<Guid> SaveFile(byte[] data)
        {
            return await FileRepository.Save(data);
        }

        public async Task<Model.Context.File> GetFileInfo(Guid fileId)
        {
            return await Database.Files.FindAsync(fileId);
        }

        public async Task<FileDataModel> GetThumbnail(Guid fileId)
        {
            var file = await Database.Files.FindAsync(fileId);
            if (file == null)
            {
                return FolderThumnail();
            }

            var data = await FileRepository.GetThumbnail(fileId);

            if (data != null)
            {
                return new FileDataModel
                {
                    Data = data
                }.SemanticCopy(file);
            }

            // Return default image
            return DefaultThumnail(file.Name, file.Extension.ToLower());
        }

        public int GetFileSize(Guid fileId)
        {
            return Database.Files.FirstOrDefault(x => x.Id == fileId)!.Size;
        }

        public string GetFileName(Guid fileId)
        {
            return Database.Files.First(x => x.Id == fileId).ToString();
        }


        public async Task<byte[]> CreateZipFile(IEnumerable<Guid> fileIds)
        {
            var files = Database
                .Files
                .Where(f => fileIds.Contains(f.Id))
                .OrderBy(x => x.Name)
                .ToList()
                .Select(x => new FileModel().SemanticCopy(x));
            
            return await FileRepository.CreateCompressedFile(files);
        }

        public async Task<FileDataModel> GetFile(Guid fileId)
        {
            var file = await Database.Files.FindAsync(fileId);
            var data = await FileRepository.Get(fileId);            

            return new FileDataModel
            {
                Data = data                
            }.SemanticCopy(file);
        }

        private byte[] MakeThumbnail(byte[] myImage, int thumbWidth, int thumbHeight)
        {
            using (MemoryStream ms = new MemoryStream())
            using (Image thumbnail = Image.FromStream(new MemoryStream(myImage)).GetThumbnailImage(thumbWidth, thumbHeight, null, new IntPtr()))
            {
                thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        public string GetDefaultStore()
        {   
            var path = ServerHost.PublicPath;
            path = Path.Combine(path, "default");

            if (path.Last() != '\\')
                path += '\\';

            //throw new Exception("DefaultPath: " + path);
            return path;            
        }

        private FileDataModel DefaultThumnail(string name, string extension)
        {
            var store = GetDefaultStore();
            try
            {
                if (extension.EndsWith("x") && extension.Length == 4)
                    extension = extension.Substring(0, 3);

                var file = store + extension + ".png";
                if (!System.IO.File.Exists(file))
                {
                    file = store + "unknown.png";
                }
                var data = System.IO.File.ReadAllBytes(file);

                return new FileDataModel
                {
                    Data = data,
                    Name = name,
                    Extension = "png"
                };
            }
            catch
            {
                return new FileDataModel
                {
                    Data = new byte[0],
                    Name = "Exception: " + store,
                    Extension = "png"
                };
            }
        }

        private FileDataModel FolderThumnail()
        {
            var store = GetDefaultStore();

            var file = store + "folder.png";
            if (!System.IO.File.Exists(file))
            {
                file = store + "unknown.png";
            }
            var data = System.IO.File.ReadAllBytes(file);

            return new FileDataModel
            {
                Data = data,
                Name = "folder",
                Extension = "png"
            };
        }

        public async Task UpdateTags(string text, Guid entityId, EntityType type)
        {
            var tags = await Database
                .Tags
                .Where(x => x.EntityId == entityId)
                .ToListAsync();

            if (tags.Any())
            {
                tags.Do(t => Database.Tags.Remove(t));
            }
            await Database.SaveChangesAsync();

            if (!text.HasValue()) return;

            var parts = text
                .Split(',')
                .SelectMany(s => s.Split(' '))                
                .Select(t => t.Trim())
                .ToList();

            parts.Do(p =>
            {
                Database.Tags.Add(new Tag
                {
                    Id = Guid.NewGuid(),
                    EntityId = entityId,
                    EntityType = type,
                    Text = p
                });
            });

            await Database.SaveChangesAsync();
        }
    }
}
