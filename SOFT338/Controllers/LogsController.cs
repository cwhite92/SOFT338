﻿using System;
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
using System.Data.Entity.Spatial;
using SOFT338.Apis;
using SOFT338.Filters;

namespace SOFT338.Controllers
{
    [BasicAuth]
    public class LogsController : BaseController
    {
        [Route("api/v1/journies/{journeyId}/logs", Name="GetLogs")]
        [HttpGet]
        public IHttpActionResult GetLogs(int journeyId)
        {
            if (!this.JourneyExists(journeyId))
            {
                return NotFound();
            }

            List<object> output = new List<object>();

            foreach (Log log in db.Logs.Where(l => l.JourneyId == journeyId))
            {
                output.Add(new {
                    Id = log.Id,
                    JourneyId = log.JourneyId,
                    Latitude = log.Location.Latitude,
                    Longitude = log.Location.Longitude,
                    Postcode = log.Postcode
                });
            }

            return Ok(output);
        }

        [ResponseType(typeof(Log))]
        [Route("api/v1/journies/{journeyId}/logs/{logId}", Name="GetLog")]
        [HttpGet]
        public IHttpActionResult GetLog(int journeyId, int logId)
        {
            if (!this.JourneyExists(journeyId))
            {
                return NotFound();
            }

            Log log = db.Logs.Find(logId);
            if (log == null)
            {
                return NotFound();
            }

            return Ok(log.GetOutputObject());
        }

        [ResponseType(typeof(void))]
        [Route("api/v1/journies/{journeyId}/logs/{logId}", Name="PutLog")]
        [HttpPut]
        public IHttpActionResult PutLog(int journeyId, int logId, Log log)
        {
            if (!this.JourneyExists(journeyId))
            {
                return NotFound();
            }

            log.Id = logId;
            log.JourneyId = journeyId;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (logId != log.Id)
            {
                return BadRequest();
            }

            db.Entry(log).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogExists(logId))
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

        [ResponseType(typeof(Log))]
        [Route("api/v1/journies/{journeyId}/logs", Name="PostLog")]
        [HttpPost]
        public IHttpActionResult PostLog(int journeyId, Log log)
        {
            if (!this.JourneyExists(journeyId))
            {
                return NotFound();
            }

            log.JourneyId = journeyId;

            // Convert lat and long into a DbGeography-compatible format
            string coordinates = String.Format("POINT({0} {1})", log.Latitude, log.Longitude);
            log.Location = DbGeography.FromText(coordinates);

            // Determine nearest postcode with 3rd party API
            log.Postcode = PostcodeApi.GetPostcodeFromLatLong(Convert.ToDouble(log.Latitude), Convert.ToDouble(log.Longitude));

            ModelState.Clear();
            TryValidateModel(log);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Logs.Add(log);
            db.SaveChanges();

            return CreatedAtRoute("GetLog", new { journeyId = journeyId, logId = log.Id }, log.GetOutputObject(false));
        }

        [ResponseType(typeof(Log))]
        [Route("api/v1/journies/{journeyId}/logs/{logId}", Name="DeleteLog")]
        [HttpDelete]
        public IHttpActionResult DeleteLog(int journeyId, int logId)
        {
            if (!this.JourneyExists(journeyId))
            {
                return NotFound();
            }

            Log log = db.Logs.Find(logId);
            if (log == null)
            {
                return NotFound();
            }

            db.Logs.Remove(log);
            db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        private bool JourneyExists(int id)
        {
            return db.Journeys.Count(e => e.Id == id) > 0;
        }

        private bool LogExists(int id)
        {
            return db.Logs.Count(e => e.Id == id) > 0;
        }
    }
}