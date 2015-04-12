using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace SOFT338.Models
{
    public class Log
    {
        public int Id { get; set; }

        public int JourneyId { get; set; }

        public DbGeography Location { get; set; }

        [NotMapped]
        [Required, RegularExpression(@"^[+\-]?[0-9]{1,3}\.[0-9]{3,}$", ErrorMessage = "The Latitude field must be a valid latitude.")]
        public string Latitude { get; set; }

        [NotMapped]
        [Required, RegularExpression(@"^[+\-]?[0-9]{1,3}\.[0-9]{3,}$", ErrorMessage = "The Longitude field must be a valid longitude.")]
        public string Longitude { get; set; }

        // Relationships
        public virtual Journey Journey { get; set; }

        /// <summary>
        /// Returns an anonymous object including only the model fields which are appropriate for output.
        /// </summary>
        /// <returns>The anonymous object suitible for output to the client.</returns>
        public Object GetOutputObject(bool withJourney = true)
        {
            if (withJourney)
            {
                Journey journey = null;
                using (ApiDbContext db = new ApiDbContext())
                {
                    journey = db.Journeys.Find(this.JourneyId);
                }

                return new
                {
                    Id = this.Id,
                    Journey = journey.GetOutputObject(),
                    Latitude = this.Location.Latitude,
                    Longitude = this.Location.Longitude
                };
            }

            return new
            {
                Id = this.Id,
                Latitude = this.Location.Latitude,
                Longitude = this.Location.Longitude
            };
        }
    }
}