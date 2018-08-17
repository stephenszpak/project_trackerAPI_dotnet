using ProjectTracker.Api.Resources.Authorization;
using ProjectTracker.Api.Resources.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Routing;

namespace ProjectTracker.Api.Models
{
    public static class AuthorizeModel
    {

        public static async Task<HttpResponseMessage> PostAsync(this Authorize authorize,
                  HttpRequestMessage request, UrlHelper url)
        {
            var context = default(Context);
            context = context ?? request.GetContext();

            if (string.IsNullOrEmpty(authorize.UserName))
                return request.CreateResponse(HttpStatusCode.BadRequest, "Username is required");

            if (string.IsNullOrEmpty(authorize.Password))
                return request.CreateResponse(HttpStatusCode.BadRequest, "Password is required");

            var result = await context.Authorizations.AuthorizeAsync(authorize.UserName, authorize.Password,
            (token) => request.CreateResponse(HttpStatusCode.Created, token),
            (why) => request.CreateResponse(HttpStatusCode.BadRequest, "The post failed"),
            () => request.CreateResponse(HttpStatusCode.Unauthorized));
            return result;
        }
    }
}
