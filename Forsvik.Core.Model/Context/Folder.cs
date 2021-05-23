using System;
using System.Collections.Generic;

namespace Forsvik.Core.Model.Context
{
    public class Folder
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Path { get; set; }

        public int Index { get; set; }

        public string Tags { get; set; }

        public Guid? ImageFileId { get; set; }

        public Guid? ParentFolderId { get; set; }

        public Folder ParentFolder { get; set; }

        public virtual ICollection<Folder> Folders { get; set; } = new List<Folder>();

        public virtual ICollection<File> Files { get; set; }
    }
}
