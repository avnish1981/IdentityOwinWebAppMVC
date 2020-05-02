using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace IdentityOwinWebApp.Demo.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Please enter your first name")]
        [Display(Name = "First name")]
        [StringLength(50)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter your first name")]
        [Display(Name = "Password")]
        [StringLength(50)]
        public string Password { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string LastName { get; set; }
        public string AddressLine { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
        public string Email { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public string Roles { get; set; }
    }
}