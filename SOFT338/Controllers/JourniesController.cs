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

namespace SOFT338.Controllers
{
    public class JourniesController : ApiController
    {
        private ApiDbContext db = new ApiDbContext();

        // GET: api/Journies
        public IQueryable<Journey> GetJourneys()
        {
            return db.Journeys;
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

            return Ok(journey);
        }

        // PUT: api/Journies/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutJourney(int id, Journey journey)
        {
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Journeys.Add(journey);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = journey.Id }, journey);
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

            db.Journeys.Remove(journey);
            db.SaveChanges();

            return Ok(journey);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool JourneyExists(int id)
        {
            return db.Journeys.Count(e => e.Id == id) > 0;
        }
    }
}