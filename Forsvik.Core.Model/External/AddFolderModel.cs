﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forsvik.Core.Model.External
{
    public class AddFolderModel
    {
        public string Name { get; set; }

        public Guid ParentFolderId { get; set; }
    }
}
