using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityOwinWebApp.Demo.Models
{
    public class TwoFactor
    {
        public string  Provider { get; set; }
        public string Code { get; set; }
    }
}