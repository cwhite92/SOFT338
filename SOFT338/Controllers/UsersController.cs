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
    public class UsersController : BaseController
    {
        [ResponseType(typeof(User))]
        [BasicAuth]
        [Route("api/v1/users/{userId}", Name="GetUser")]
        [HttpGet]
        public IHttpActionResult GetUser(int userId)
        {
            User user = db.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Check that the authenticated user is accessing themselves
            if (HttpContext.Current.User.Identity.Name != userId.ToString())
            {
                return NotFound();
            }

            return Ok(user.GetOutputObject());
        }

        [ResponseType(typeof(User))]
        [AllowAnonymous]
        [Route("api/v1/users", Name="PostUser")]
        [HttpPost]
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

            return CreatedAtRoute("GetUser", new { userId = user.Id }, user.GetOutputObject());
        }
    }
}