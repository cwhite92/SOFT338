using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Device.Location;
using System.Linq;
using System.Web;

namespace SOFT338.Models
{
    public class Journey
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required, MaxLength(50)]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [NotMapped]
        public double TotalDistance
        {
            get
            {
                double totalDistance = 0;

                using (ApiDbContext db = new ApiDbContext())
                {
                    List<Log> logs = db.Logs.Where(l => l.JourneyId == this.Id).ToList();

                    // If there's only one log entry the distance is 0
                    if (logs.Count() == 1)
                    {
                        return 0;
                    }
                    
                    // Loop through all logs, adding distances
                    for (int i = 1; i < logs.Count(); i++)
                    {
                        var sCoord = new GeoCoordinate(Convert.ToDouble(logs[i - 1].Location.Latitude), Convert.ToDouble(logs[i - 1].Location.Longitude));
                        var eCoord = new GeoCoordinate(Convert.ToDouble(logs[i].Location.Latitude), Convert.ToDouble(logs[i].Location.Longitude));

                        // Distance is in meters, convert to kilometers
                        totalDistance += (sCoord.GetDistanceTo(eCoord) / 1000);
                    }
                }

                return totalDistance;
            }
        }

        // Relationships
        public virtual User User { get; set; }
        public virtual ICollection<Log> Logs { get; set; }

        /// <summary>
        /// Returns an anonymous object including only the model fields which are appropriate for output.
        /// </summary>
        /// <returns>The anonymous object suitible for output to the client.</returns>
        public Object GetOutputObject(bool withLogs = true)
        {
            if (withLogs)
            {
                List<object> output = new List<object>();

                using (ApiDbContext db = new ApiDbContext())
                {
                    List<Log> logs = db.Logs.Where(l => l.JourneyId == this.Id).ToList();

                    foreach (Log log in db.Logs.Where(l => l.JourneyId == this.Id))
                    {
                        output.Add(log.GetOutputObject(false));
                    }
                }

                return new
                {
                    Title = this.Title,
                    Date = this.Date,
                    TotalDistance = this.TotalDistance,
                    Logs = output
                };
            }

            return new
            {
                Title = this.Title,
                Date = this.Date,
                TotalDistance = this.TotalDistance
            };
        }
    }
}