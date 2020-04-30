using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityOwinWebApp.Demo.Models
{
    public class ExtendingUser:IdentityUser
    {
        public ExtendingUser()
        {
            Addresses = new List<Address>();
        }
        
        public string LastName { get; set; }
         public string DOB { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Address> Addresses { get; private set; } //here you marked collection to be virtual to enable lazy loading .
    }
}