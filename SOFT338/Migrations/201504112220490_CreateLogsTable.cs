namespace SOFT338.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateLogsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        JourneyId = c.Int(nullable: false),
                        Long = c.String(),
                        Lat = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Journeys", t => t.JourneyId, cascadeDelete: true)
                .Index(t => t.JourneyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Logs", "JourneyId", "dbo.Journeys");
            DropIndex("dbo.Logs", new[] { "JourneyId" });
            DropTable("dbo.Logs");
        }
    }
}
