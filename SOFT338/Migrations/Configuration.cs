namespace SOFT338.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using SOFT338.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<SOFT338.Models.APIContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SOFT338.Models.APIContext context)
        {
            // TODO: Seed some sensible example data
        }
    }
}
