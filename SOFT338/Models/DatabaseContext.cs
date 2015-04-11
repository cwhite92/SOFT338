using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SOFT338.Models
{
    public class ApiDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public System.Data.Entity.DbSet<SOFT338.Models.Journey> Journeys { get; set; }

        public System.Data.Entity.DbSet<SOFT338.Models.Log> Logs { get; set; }
    }
}