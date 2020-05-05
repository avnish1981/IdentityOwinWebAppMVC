using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IdentityOwinWebApp.Demo.Models
{
    public class Login
    {

          [Display(Name = "Email")]
          [Required]
        public string UserName { get; set; }
        [Required ]
        [DataType(DataType.Password )]
        public string  Password { get; set; }
    }
}