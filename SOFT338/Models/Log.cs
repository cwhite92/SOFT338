using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOFT338.Models
{
    public class Log
    {
        public int Id { get; set; }

        public int JourneyId { get; set; }

        public string Long { get; set; }

        public string Lat { get; set; }

        // Relationships
        public virtual Journey Journey { get; set; }
    }
}