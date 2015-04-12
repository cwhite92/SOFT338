using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
                    Logs = output
                };
            }

            return new
            {
                Title = this.Title,
                Date = this.Date
            };
        }
    }
}