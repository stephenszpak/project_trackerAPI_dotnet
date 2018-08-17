using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace ProjectTracker.Api.Auth
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        private List<string> UserRolesRequired { get; set; }

        public AuthorizeRolesAttribute(params string[] roles) : base()
        {
            UserRolesRequired = new List<string>();
            foreach (var item in roles)
            {
                UserRolesRequired.Add(item);
            }
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (UserRolesRequired != null)
            {
                foreach (var role in UserRolesRequired)
                    if (HttpContext.Current.User.IsInRole(role))
                    {
                        return true;
                    }
            }
            return false;
        }
    }
}