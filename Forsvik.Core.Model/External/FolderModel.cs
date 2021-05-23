using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forsvik.Core.Model.External
{
    public class FolderModel
    {
        public Guid Id { get; set; }

        public Guid? ParentFolderId { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }

        public string Tags { get; set; }

        public int Index { get; set; }

        public string Description { get; set; }

        public Guid? ImageFileId { get; set; }
    }
}
