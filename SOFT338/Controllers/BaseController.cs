using SOFT338.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SOFT338.Controllers
{
    public class BaseController : ApiController
    {
        protected ApiDbContext db { get; set; }

        public BaseController()
        {
            this.db = new ApiDbContext();
        }

        /// <summary>
        /// Disposes of the database context at the end of a request cycle.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this.db.Dispose();
            base.Dispose(disposing);
        }
    }
}
