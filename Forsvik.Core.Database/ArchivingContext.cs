using Forsvik.Core.DocumentStore.Contract;
using Forsvik.Core.DocumentStore.Models;
using Forsvik.Core.Model.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Forsvik.Core.Database
{
    public class ArchivingContext : DbContext, IDocumentDbContext
    {
        public ArchivingContext(DbContextOptions<ArchivingContext> options) : base(options)
        {
        }

        public DbSet<Folder> Folders { get; set; }
        
        public DbSet<File> Files { get; set; }

        public DbSet<Log> Logs { get; set; }
        
        public DbSet<Tag> Tags { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<DocumentIndex> DocumentIndexes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DocumentIndex>()
                .Property(x => x.Key)
                .HasMaxLength(100);

            modelBuilder.Entity<DocumentIndex>()
                .Property(x => x.Value)
                .HasMaxLength(100);

            modelBuilder.Entity<DocumentIndex>()
                .HasIndex(p => new { p.Key, p.Value });

            modelBuilder.Entity<Tag>()
                .Property(x => x.Text)
                .HasMaxLength(200);

            modelBuilder.Entity<Tag>()
                .HasIndex(p => p.Text);

            base.OnModelCreating(modelBuilder);
        }
    }
}
