using Forsvik.Core.Model.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forsvik.Core.Model.External
{
    public class SearchModel
    {
        public Guid EntityId { get; set; }

        public Guid FolderId { get; set; }

        public EntityType EntityType { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }        

        public string Extension { get; set; }

        public string Description { get; set; }

        public string Tags { get; set; }
    }
}
