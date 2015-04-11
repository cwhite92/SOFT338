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
    [BasicAuth]
    public class JourniesController : BaseController
    {
        // GET: api/Journies
        public IQueryable<Journey> GetJourneys()
        {
            // TODO: make this not return user data
            int userId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
            return db.Journeys.Where(j => j.UserId == userId);
        }

        // GET: api/Journies/5
        [ResponseType(typeof(Journey))]
        public IHttpActionResult GetJourney(int id)
        {
            Journey journey = db.Journeys.Find(id);
            if (journey == null)
            {
                return NotFound();
            }

            // Ensure user is only getting their own journey
            if (HttpContext.Current.User.Identity.Name != journey.UserId.ToString())
            {
                return NotFound();
            }

            return Ok(journey.GetOutputObject());
        }

        // PUT: api/Journies/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutJourney(int id, Journey journey)
        {
            journey.Id = id;
            journey.UserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != journey.Id)
            {
                return BadRequest();
            }

            db.Entry(journey).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JourneyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Journies
        [ResponseType(typeof(Journey))]
        public IHttpActionResult PostJourney(Journey journey)
        {
            // Add authed user ID to model
            journey.UserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Journeys.Add(journey);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = journey.Id }, journey.GetOutputObject());
        }

        // DELETE: api/Journies/5
        [ResponseType(typeof(Journey))]
        public IHttpActionResult DeleteJourney(int id)
        {
            Journey journey = db.Journeys.Find(id);
            if (journey == null)
            {
                return NotFound();
            }

            // Ensure the user is deleting one of their own journies
            if (HttpContext.Current.User.Identity.Name != journey.UserId.ToString())
            {
                return NotFound();
            }

            db.Journeys.Remove(journey);
            db.SaveChanges();

            return Ok(journey.GetOutputObject());
        }

        private bool JourneyExists(int id)
        {
            return db.Journeys.Count(e => e.Id == id) > 0;
        }
    }
}