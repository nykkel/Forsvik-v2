using System;

namespace Forsvik.Core.DocumentStore.Models
{
    public class DocumentIndex
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public Guid DocumentId { get; set; }

        public DateTime Created { get; set; }
    }
}
