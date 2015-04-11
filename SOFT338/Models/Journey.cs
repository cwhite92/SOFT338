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

        /// <summary>
        /// Returns an anonymous object including only the model fields which are appropriate for output.
        /// </summary>
        /// <returns>The anonymous object suitible for output to the client.</returns>
        public Object GetOutputObject()
        {
            return new
            {
                Title = this.Title,
                Date = this.Date
            };
        }
    }
}