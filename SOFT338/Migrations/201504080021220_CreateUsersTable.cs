namespace SOFT338.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateUsersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true, name: "EmailIndex");
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
