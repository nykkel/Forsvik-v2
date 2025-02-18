using Forsvik.Core.DocumentStore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Forsvik.Core.DocumentStore.Mappings
{
    public class DocumentIndexMap : IEntityTypeConfiguration<DocumentIndex>
    {
        protected string SchemaName = "dbo";

        public void Configure(EntityTypeBuilder<DocumentIndex> builder)
        {
            //builder.ToTable(schema: SchemaName, name: nameof(DocumentIndex));
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Created).IsRequired();
            builder.Property(t => t.DocumentId);
            builder.HasIndex(t => t.DocumentId);
            builder.HasIndex(t => new {t.Key, t.Value});
            //.IncludeProperties(p => p.Value);
        }
    }
}
