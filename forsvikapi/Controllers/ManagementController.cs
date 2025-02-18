using System.Threading.Tasks;
using Forsvik.Core.Database.Repositories;
using Forsvik.Core.Model.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace forsvikapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly DocumentFileRepository _documentRepository;

        public ManagementController(IFileRepository documentRepository)
        {
            _documentRepository = (DocumentFileRepository)documentRepository;
        }

        [HttpGet("migrate")]
        public async Task<IActionResult> Migrate()
        {
            var count = await _documentRepository.MigrateFilesToDb();
            return Ok($"Done migrating {count} files");
        }
    }
}
