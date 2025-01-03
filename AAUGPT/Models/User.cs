using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AAUGPT.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }

        public bool IsEmailVerified { get; set; }
        public string VerificationToken { get; set; }
        public string RememberMeToken { get; set; }
    }
}