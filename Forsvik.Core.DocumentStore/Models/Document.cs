using System;

namespace Forsvik.Core.DocumentStore.Models
{
    public class Document
    {
        public Guid Id { get; set; }

        public byte[] Data { get; set; }

        public string DataType { get; set; }

        public DateTime Created { get; set; }
    }
}
