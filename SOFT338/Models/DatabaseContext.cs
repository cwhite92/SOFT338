using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SOFT338.Models
{
    public class APIContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}