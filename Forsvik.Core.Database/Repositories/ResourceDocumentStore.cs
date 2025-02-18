using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forsvik.Core.DocumentStore.Contract;

namespace Forsvik.Core.Database.Repositories
{
    public class ResourceDocumentStore : DocumentStore.Repositories.DocumentStore
    {
        public ResourceDocumentStore(ArchivingContext database) : base(database)
        {
        }
    }
}
