using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forsvik.Core.Model.External
{
    public class FilesRequest
    {
        public List<Guid> FileIds { get; set; } = new List<Guid>();
    }
}
