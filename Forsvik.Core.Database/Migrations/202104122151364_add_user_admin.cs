namespace Forsvik.Core.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_user_admin : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        IsAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
