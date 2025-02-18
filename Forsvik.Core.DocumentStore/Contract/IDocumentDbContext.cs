using System.Threading;
using System.Threading.Tasks;
using Forsvik.Core.DocumentStore.Models;
using Microsoft.EntityFrameworkCore;

namespace Forsvik.Core.DocumentStore.Contract
{
    public interface IDocumentDbContext
    {
        DbSet<Document> Documents { get; set; }

        DbSet<DocumentIndex> DocumentIndexes { get; set; }

        Task<int> SaveChangesAsync(CancellationToken token = default);
    }
}
