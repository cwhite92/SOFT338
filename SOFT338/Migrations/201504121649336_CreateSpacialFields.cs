namespace SOFT338.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Spatial;
    
    public partial class CreateSpacialFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Logs", "Location", c => c.Geography());
            DropColumn("dbo.Logs", "Long");
            DropColumn("dbo.Logs", "Lat");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Logs", "Lat", c => c.String());
            AddColumn("dbo.Logs", "Long", c => c.String());
            DropColumn("dbo.Logs", "Location");
        }
    }
}
