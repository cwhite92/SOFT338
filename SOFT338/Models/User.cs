﻿using System;
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
        public string Password { get; set; }

        /// <summary>
        /// Checks the stored password against the one supplied.
        /// </summary>
        /// <param name="password">The password to check.</param>
        /// <returns>True if the supplied password is correct, false otherwise.</returns>
        public bool CheckPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, this.Password);
        }

        /// <summary>
        /// Returns an anonymous object including only the model fields which are appropriate for output.
        /// </summary>
        /// <returns>The anonymous object suitible for output to the client.</returns>
        public Object GetOutputObject()
        {
            return new
            {
                Id = this.Id,
                Email = this.Email
            };
        }
    }

    /// <summary>
    /// Attribute class to provide validation for one unique email address per user.
    /// </summary>
    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) {
                // User didn't specify an email, just pass this validation rule for it to be picked up by Required
                return ValidationResult.Success;
            }

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