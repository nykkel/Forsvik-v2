using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forsvik.Core.Model.Context
{
    public class File
    {
        public Guid Id { get; set; }
        
        public Guid? FolderId { get; set; }

        public DateTime Created { get; set; }

        public virtual Folder Folder { get; set; }

        public int Size { get; set; }

        public string Name { get; set; }
        
        public string Description{ get; set; }

        public string Tags { get; set; }

        public string Extension { get; set; }

        public override string ToString()
        {
            return $"{Name}.{Extension}";
        }
    }
}
