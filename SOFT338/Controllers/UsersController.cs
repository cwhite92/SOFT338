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

            var test = System.Web.HttpContext.Current.User;

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