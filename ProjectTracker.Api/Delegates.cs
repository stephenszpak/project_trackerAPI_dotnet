using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ProjectTracker.Api
{
    public delegate Task<HttpResponseMessage> HttpActionDelegate();

    public class HttpActionResult : IHttpActionResult
    {
        private HttpActionDelegate callback;

        public HttpActionResult(HttpActionDelegate callback)
        {
            this.callback = () => callback();
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return callback();
        }
    }
}