using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DOB { get; set; }
        public string Country { get; set; }
        public string AddressesLine { get; set; }




        public virtual ICollection<Address> Addresses { get; private set; } //here you marked collection to be virtual to enable lazy loading .
    }

   

    
}