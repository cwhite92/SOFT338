namespace SOFT338.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatePostcodeField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Logs", "Postcode", c => c.String(nullable: false, maxLength: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Logs", "Postcode");
        }
    }
}
