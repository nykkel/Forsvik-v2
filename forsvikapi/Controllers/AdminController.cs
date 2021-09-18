using Forsvik.Core.Database.Repositories;
using Forsvik.Core.Model.Context;
using Forsvik.Core.Model.External;
using Forsvik.Core.Model.Interfaces;
using forsvikapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace forsvikapi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : RootController
    {
        private readonly UserService UserService;

        public AdminController(
            ILogService logService,
            IWebHostEnvironment hostingEnvironment,
            ArchivingRepository archivingRepository,            
            UserService userService, 
            DocumentRepository documentRepository) : base(logService, hostingEnvironment, archivingRepository, documentRepository)
        {
            UserService = userService;
        }

        [Route("users")]
        public async Task<ActionResult<List<UserListItem>>> Users()
        {
            var users = await Task.Factory.StartNew(() => UserService.GetUsers());

            return users;
        }

        [HttpPost]
        [Route("adduser")]
        public async Task<ActionResult<bool>> AddUser(UserModel user)
        {
            await Task.Factory.StartNew(() => UserService.AddUser(user));
            return true;
        }

        [HttpPost]
        [Route("update")]
        public async Task<ActionResult<bool>> UpdateUser(UpdateUserModel model)
        {
            await Task.Factory.StartNew(() => UserService.UpdateUser(model));
            return true;
        }
        
        [HttpPost]
        [Route("delete")]
        public async Task<ActionResult<bool>> DeleteUser(UpdateUserModel model)
        {
            await Task.Factory.StartNew(() => UserService.DeleteUser(model.Id));
            return true;
        }

    }
}
