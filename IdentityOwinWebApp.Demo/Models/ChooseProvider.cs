using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IdentityOwinWebApp.Demo.Models
{
    public class ChooseProvider
    {
        public List<string > Providers { get; set; }
        public string ChosenProvider { get; set; }
    }
}