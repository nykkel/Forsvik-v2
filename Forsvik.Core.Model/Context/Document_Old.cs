using System;

namespace Forsvik.Core.Model.Context
{
    public class Document_Old
    {
        public Guid Id { get; set; }

        public byte[] Data { get; set; }

        public string DataType { get; set; }

        public DateTime Created { get; set; }
    }
}
