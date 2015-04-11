namespace SOFT338.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateJourniesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Journeys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 50),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Journeys", "UserId", "dbo.Users");
            DropIndex("dbo.Journeys", new[] { "UserId" });
            DropTable("dbo.Journeys");
        }
    }
}
