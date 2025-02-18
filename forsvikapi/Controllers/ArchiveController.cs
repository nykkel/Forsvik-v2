using Forsvik.Core.Model.Interfaces;
using Forsvik.Core.Database.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forsvik.Core.Model.External;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Web;
using Microsoft.Extensions.Configuration;
using Forsvik.Core.Database;
using forsvikapi.Services;
using Microsoft.AspNetCore.Authorization;

namespace forsvikapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ErrorHandlingFilter]
    public class ArchiveController : RootController
    {
        private readonly ArchivingContext Database;
        private UserService UserService;

        public ArchiveController(
            ILogService logService, 
            IWebHostEnvironment hostingEnvironment, 
            ArchivingRepository archivingRepository, 
            UserService userService,
            ArchivingContext database) : base(logService, hostingEnvironment, archivingRepository)
        {
            Database = database;
            UserService = userService;
        }

        [HttpPost]
        [Route("logout")]
        public async Task<ActionResult<bool>> Logout()
        {
            await LoginService.SignOut(HttpContext);
            return true;
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<CurrentUserModel>> Login(LoginModel model)
        {
            var authUser = UserService.IsValidUser(model);
            if (authUser != null)
            {
                await LoginService.SignIn(HttpContext, model.Email, model.RememberMe);
            }
            return authUser;
        }

        [HttpGet]
        [Route("search")]
        public async Task<ActionResult<List<SearchModel>>> Search(string query, SearchCategory category)
        {
            var searchRepository = new SearchRepository(Database);

            var text = HttpUtility.UrlDecode(query);

            var items = await searchRepository.Find(text, category);
            return items;
        }

        [HttpGet]
        [Route("getarchives")]
        public async Task<IActionResult> GetArchives()
        {
            try
            {
                var archives = await Repository.GetArchives();
                return Ok(archives);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Ok($"Error: {e.GetBaseException().Message}, {e.StackTrace}");
            }
        }

        [HttpGet]
        [Route("getfolder/{id}")]
        public async Task<ActionResult<FolderModel>> GetFolder(Guid id)
        {
            var folder = await Repository.GetFolder(id);
            return folder;
        }

        [HttpGet]
        [Route("getfolders/{parentFolderId}")]
        public async Task<ActionResult<List<FolderModel>>> GetFolders(Guid parentFolderId)
        {
            var folders = await Repository.GetFolders(parentFolderId);
            return folders;
        }

        [HttpGet]
        [Route("getfoldernavigation/{folderId}")]
        public async Task<ActionResult<List<NavigationModel>>> GetFolderNavigation(Guid folderId)
        {
            var list = new List<NavigationModel>();
            var folder = await Repository.GetFolder(folderId);
            list.Add(new NavigationModel
            {
                Name = folder.Name,
                Route = "/folder?id=" + folder.Id
            });

            while (folder.ParentFolderId != null)
            {
                folder = await Repository.GetFolder(folder.ParentFolderId.Value);
                list.Insert(0, new NavigationModel
                {
                    Name = folder.Name,
                    Route = "/folder?id=" + folder.Id
                });
            }

            list.Insert(0, new NavigationModel
            {
                Name = "Arkiv",
                Route = "/dashboard"
            });

            if (list.Count > 1)
                list.Last().IsLastFolder = true;

            return list;
        }

        [HttpGet]
        [Route("getfiles/{folderId}")]
        public async Task<ActionResult<List<FileModel>>> GetFiles(Guid folderId, bool? sortAsc, SearchField searchField)
        {
            var files = await Repository.GetFiles(folderId, sortAsc, searchField);
            files.ForEach(file =>
            {
                file.Url = $"file/resource/{file.Id}";
            });
            return files;
        }

        [HttpPost]
        [Route("savefilechanges")]
        public async Task<ActionResult> SaveFileChanges(FileModel model)
        {
            await Repository.SaveFileChanges(model);
            return new EmptyResult();
        }

        [HttpPost]
        [Route("savefolder")]
        public async Task<ActionResult<Guid>> SaveFolder(FolderModel model)
        {
            return await Repository.SaveFolder(model);
        }

        [HttpGet]
        [Route("getarchive/{id}")]
        public async Task<ActionResult<FolderModel>> GetArchive(Guid id)
        {
            var model = await Repository.GetArchive(id);
            return model;
        }

        [HttpPost]        
        [Route("addfolder")]
        public async Task<ActionResult<Guid>> AddFolder(AddFolderModel model)
        {
            var id = await Repository.AddFolder(model);
            return id;
        }
    }
}
