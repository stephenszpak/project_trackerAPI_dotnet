using ProjectTracker.Api.Models;
using ProjectTracker.Api.Resources;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProjectTracker.Api.Controllers
{
    //[Auth.Auth.InternalOnly]
    //[AuthorizeRoles(CustomUserRoles.Admin, CustomUserRoles.User, CustomUserRoles.ReadOnly)]
    public class TasksController : BaseController
    {
        //[AuthorizeRoles(CustomUserRoles.Admin)]
        public async Task<IHttpActionResult> GetAsync([FromUri]TaskResource query)
        {
            if (default(TaskResource) == query)
                query = new TaskResource();
            var results = await query.GetAsync(this.Request, this.Url);
            return results.ToActionResult();
        }

        //[AuthorizeRoles(CustomUserRoles.Admin)]
        public async Task<IHttpActionResult> PostAsync([FromBody]TaskResource task)
        {
            var response = await task.PostAsync(this.Request, this.Url);
            return response.ToActionResult();
        }

        //[AuthorizeRoles(CustomUserRoles.Admin)]
        public async Task<IHttpActionResult> PutAsync([FromBody]TaskResource task)
        {
            var response = await task.PutAsync(this.Request, this.Url);
            return response.ToActionResult();
        }

        //[AuthorizeRoles(CustomUserRoles.Admin)]
        public async Task<IHttpActionResult> DeleteAsync([FromUri]TaskResource task)
        {
            var response = await task.DeleteAsync(this.Request);
            return response.ToActionResult();
        }
    }
}