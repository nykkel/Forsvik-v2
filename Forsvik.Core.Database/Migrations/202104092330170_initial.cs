namespace Forsvik.Core.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Data = c.Binary(),
                        DataType = c.String(),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DocumentIndexes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Key = c.String(maxLength: 100),
                        Value = c.String(maxLength: 100),
                        DocumentId = c.Guid(nullable: false),
                        Created = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.Key, t.Value });
            
            CreateTable(
                "dbo.Files",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FolderId = c.Guid(),
                        Created = c.DateTime(nullable: false),
                        Size = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Tags = c.String(),
                        Extension = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Folders", t => t.FolderId)
                .Index(t => t.FolderId);
            
            CreateTable(
                "dbo.Folders",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Path = c.String(),
                        Tags = c.String(),
                        ImageFileId = c.Guid(),
                        ParentFolderId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Folders", t => t.ParentFolderId)
                .Index(t => t.ParentFolderId);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Level = c.String(),
                        Created = c.DateTime(nullable: false),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        EntityId = c.Guid(nullable: false),
                        Text = c.String(maxLength: 200),
                        EntityType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Text);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Folders", "ParentFolderId", "dbo.Folders");
            DropForeignKey("dbo.Files", "FolderId", "dbo.Folders");
            DropIndex("dbo.Tags", new[] { "Text" });
            DropIndex("dbo.Folders", new[] { "ParentFolderId" });
            DropIndex("dbo.Files", new[] { "FolderId" });
            DropIndex("dbo.DocumentIndexes", new[] { "Key", "Value" });
            DropTable("dbo.Tags");
            DropTable("dbo.Logs");
            DropTable("dbo.Folders");
            DropTable("dbo.Files");
            DropTable("dbo.DocumentIndexes");
            DropTable("dbo.Documents");
        }
    }
}
