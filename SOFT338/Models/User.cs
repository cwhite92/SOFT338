using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SOFT338.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, UniqueEmail, MaxLength(50), RegularExpression(@"[-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4}", ErrorMessage = "The Email field must be a valid email address.")]
        public string Email { get; set; }

        [Required]
        private string password;
        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                // Hash the password using bcrypt
                var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
                this.password = BCrypt.Net.BCrypt.HashPassword(value, salt);
            }
        }
    }

    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Query the Users table for any other user with this email
            using (ApiDbContext db = new ApiDbContext())
            {
                var results = db.Users.Where(u => u.Email == value.ToString());
                if (results.Count() > 0)
                {
                    return new ValidationResult("The Email field must be unique.");
                }
            }

            return ValidationResult.Success;
        }
    }
}