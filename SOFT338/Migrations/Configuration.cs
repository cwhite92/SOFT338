namespace SOFT338.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SOFT338.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<SOFT338.Models.ApiDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(SOFT338.Models.ApiDbContext context)
        {
            // TODO: Seed some sensible example data
        }
    }
}
