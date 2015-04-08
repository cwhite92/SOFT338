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

        [Required, MaxLength(50), RegularExpression(@"[-0-9a-zA-Z.+_]+@[-0-9a-zA-Z.+_]+\.[a-zA-Z]{2,4}", ErrorMessage = "The Email field must be a valid email address.")]
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
}