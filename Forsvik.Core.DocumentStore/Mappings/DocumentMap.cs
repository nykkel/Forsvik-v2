using Forsvik.Core.DocumentStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forsvik.Core.DocumentStore.Mappings
{
    public class DocumentMap : IEntityTypeConfiguration<Document>
    {
        protected string SchemaName = "dbo";

        public void Configure(EntityTypeBuilder<Document> builder)
        {
            //builder.ToTable(schema: SchemaName, name: nameof(Document));
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Created).IsRequired();
            builder.Property(t => t.Data).IsRequired();
            builder.Property(t => t.DataType).IsRequired();
        }
    }
}
