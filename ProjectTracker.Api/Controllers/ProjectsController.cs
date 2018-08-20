using ProjectTracker.Api.Models;
using ProjectTracker.Api.Resources;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProjectTracker.Api.Controllers
{
    public class ProjectsController : BaseController
    {
        //[AuthorizeRoles(CustomUserRoles.Admin)]
        //public async Task<IHttpActionResult> GetAsync([FromUri]ProjectResource query)
        //{
        //    if (default(ProjectResource) == query)
        //        query = new ProjectResource();
        //    var results = await query.GetAsync(this.Request, this.Url);
        //    return results.ToActionResult();
        //}

        //[AuthorizeRoles(CustomUserRoles.Admin)]
        public async Task<IHttpActionResult> PostAsync([FromBody]ProjectResource project)
        {
            var response = await project.PostAsync(this.Request, this.Url);
            return response.ToActionResult();
        }

        //[AuthorizeRoles(CustomUserRoles.Admin)]
        //public async Task<IHttpActionResult> PutAsync([FromBody]ProjectResource task)
        //{
        //    var response = await task.PutAsync(this.Request, this.Url);
        //    return response.ToActionResult();
        //}

        //[AuthorizeRoles(CustomUserRoles.Admin)]
        //public async Task<IHttpActionResult> DeleteAsync([FromUri]ProjectResource task)
        //{
        //    var response = await task.DeleteAsync(this.Request);
        //    return response.ToActionResult();
        //}
    }
}