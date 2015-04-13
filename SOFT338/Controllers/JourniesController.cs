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
        [Route("api/v1/journies", Name="GetJournies")]
        [HttpGet]
        public IHttpActionResult GetJourneys()
        {
            int userId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);
            var journies = db.Journeys.Where(j => j.UserId == userId);

            List<object> output = new List<object>();

            foreach (Journey journey in db.Journeys.Where(j => j.UserId == userId))
            {
                output.Add(new {
                    Id = journey.Id,
                    Title = journey.Title,
                    Date = journey.Date,
                    TotalDistance = journey.TotalDistance
                });
            }

            return Ok(output);
        }

        [ResponseType(typeof(Journey))]
        [Route("api/v1/journies/{journeyId}", Name="GetJourney")]
        [HttpGet]
        public IHttpActionResult GetJourney(int journeyId)
        {
            Journey journey = db.Journeys.Find(journeyId);
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

        [ResponseType(typeof(void))]
        [Route("api/v1/journies/{journeyId}", Name="PutJourney")]
        [HttpPut]
        public IHttpActionResult PutJourney(int journeyId, Journey journey)
        {
            journey.Id = journeyId;
            journey.UserId = Convert.ToInt32(HttpContext.Current.User.Identity.Name);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (journeyId != journey.Id)
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
                if (!JourneyExists(journeyId))
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

        [ResponseType(typeof(Journey))]
        [Route("api/v1/journies", Name="PostJourney")]
        [HttpPost]
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

            return CreatedAtRoute("GetJourney", new { journeyId = journey.Id }, journey.GetOutputObject(false));
        }

        [ResponseType(typeof(Journey))]
        [Route("api/v1/journies/{journeyId}", Name="DeleteJourney")]
        [HttpDelete]
        public IHttpActionResult DeleteJourney(int journeyId)
        {
            Journey journey = db.Journeys.Find(journeyId);
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

            return StatusCode(HttpStatusCode.NoContent);
        }

        private bool JourneyExists(int id)
        {
            return db.Journeys.Count(e => e.Id == id) > 0;
        }
    }
}