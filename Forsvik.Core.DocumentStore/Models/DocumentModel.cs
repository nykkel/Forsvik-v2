using System;

namespace Forsvik.Core.DocumentStore.Models
{
    public class DocumentModel<T>
    {
        public Guid DocumentId { get; set; }

        public T Model { get; set; }
    }
}
