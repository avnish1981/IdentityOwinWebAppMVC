using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityOwinWebApp.Demo.Models
{
    public class PasswordReset
    {
        public string  UserId { get; set; }
        public string  Token { get; set; }
        public string Password { get; set; }
    }
}