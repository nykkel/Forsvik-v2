using System;

namespace Forsvik.Core.Model.Context
{
    public class DocumentIndex_Old
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public Guid DocumentId { get; set; }

        public DateTime Created { get; set; }
    }
}
