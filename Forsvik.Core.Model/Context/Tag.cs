using System;
using System.Collections.Generic;
using System.Text;

namespace Forsvik.Core.Model.Context
{
    public class Tag
    {
        public Guid Id { get; set; }

        public Guid EntityId { get; set; }

        public string Text { get; set; }

        public EntityType EntityType { get; set; }
    }
}
