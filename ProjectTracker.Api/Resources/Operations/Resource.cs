using Newtonsoft.Json;
using ProjectTracker.DAL.Sql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http.Routing;

namespace ProjectTracker.Api.Resources.Operations
{
    public class Resource
    {
        private Context context;

        public void Configure(HttpRequestMessage request, UrlHelper url)
        {
            this.Request = request;
            this.Url = url;
        }

        [IgnoreDataMember]
        [JsonIgnore]
        public HttpRequestMessage Request { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        protected UrlHelper Url { get; private set; }

        [JsonIgnore]
        [IgnoreDataMember]
        protected Context Context
        {
            get
            {
                if (default(Context) == context)
                    context =
                        new Context(
                            () => new DataContext(ConfigurationManager.ConnectionStrings["sql.connectstring"].ConnectionString));
                return context;
            }
        }
    }

    public static class ResourceExtensions
    {
        public static Context GetContext(this HttpRequestMessage request)
        {
            return new Context(
                () => new DataContext(ConfigurationManager.ConnectionStrings["sql.connectstring"].ConnectionString));
        }
    }
}