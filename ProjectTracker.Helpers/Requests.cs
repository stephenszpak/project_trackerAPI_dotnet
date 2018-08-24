using System;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Headers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
using ProjectTracker.DAL.Sql;
using System.Configuration;

namespace ProjectTracker.Helpers
{
    public static class Requests
    {
        public static HttpRequestMessage GetRequest()
        {
            var request = new HttpRequestMessage();
            var config = new HttpConfiguration();
            var context = new Context(() => new DataContext(ConfigurationManager.ConnectionStrings["sql.connectstring"].ConnectionString));

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            string[] emptyRoles = new string[] { };
            var token = context.Authorizations.CreateToken("TestUser", 200, emptyRoles, "test", "test");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer ", token);
            request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            request.Properties[HttpPropertyKeys.HttpRouteDataKey] = new HttpRouteData(new HttpRoute());
            request.RequestUri = new Uri("http://www.test.com");
            return request;

        }
    }
}
