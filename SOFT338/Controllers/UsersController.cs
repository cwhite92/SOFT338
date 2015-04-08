using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using SOFT338.Models;
using SOFT338.Filters;
using System.Web;

namespace SOFT338.Controllers
{
    public class UsersController : ApiController
    {
        private ApiDbContext db = new ApiDbContext();

        // GET: api/Users/5
        [ResponseType(typeof(User))]
        [BasicAuthenticator]
        public IHttpActionResult GetUser(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            // Check that the authenticated user is accessing themselves
            if (HttpContext.Current.User.Identity.Name != id.ToString())
            {
                return NotFound();
            }

            return Ok(user.GetOutputObject());
        }

        // POST: api/Users
        [ResponseType(typeof(User))]
        [AllowAnonymous]
        public IHttpActionResult PostUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Hash user's password using BCrypt
            var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, salt);

            db.Users.Add(user);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = user.Email }, user.GetOutputObject());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}