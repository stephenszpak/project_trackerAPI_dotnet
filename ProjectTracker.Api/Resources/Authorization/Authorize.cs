using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectTracker.Api.Resources.Authorization
{
    public class Authorize
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}