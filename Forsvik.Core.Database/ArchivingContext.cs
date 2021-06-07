using Forsvik.Core.Model.Context;
using Microsoft.Extensions.Configuration;
using System.Data.Entity;

namespace Forsvik.Core.Database
{
    public class ArchivingContext : DbContext
    {
        public ArchivingContext() : base("Server=81.95.105.77;Initial Catalog=e002497;User ID=e002497a;Password=Rumpnisse234;Connection Timeout=30;")
        { }

        //public ArchivingContext() : base("Data Source=.;Initial Catalog=ForsvikDb;Integrated Security=true;")
        //{ }

        public ArchivingContext(string connectionString) : base(connectionString)
        {
        }

        public ArchivingContext(IConfiguration config) : base(config.GetConnectionString("ForsvikDb"))
        {
        }

        public DbSet<Folder> Folders { get; set; }
        
        public DbSet<File> Files { get; set; }

        public DbSet<Log> Logs { get; set; }
        
        public DbSet<Tag> Tags { get; set; }

        public DbSet<User> Users { get; set; }

        public virtual DbSet<Document> Documents { get; set; }

        public virtual DbSet<DocumentIndex> DocumentsIndexes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
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
