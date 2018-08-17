using ProjectTracker.Api.Models;
using ProjectTracker.Api.Resources.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ProjectTracker.Api.Controllers
{
    [Auth.Auth.InternalOnly]
    public class AuthorizeController : BaseController
    {
        public async Task<IHttpActionResult> PostAsync([FromBody] Authorize authorize)
        {
            var response = await authorize.PostAsync(this.Request, this.Url);
            return response.ToActionResult();
        }
    }
}