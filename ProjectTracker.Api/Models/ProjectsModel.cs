using ProjectTracker.Api.Resources;
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
    public static class ProjectsModel
    {
        public static async Task<HttpResponseMessage> PostAsync(this ProjectResource project, HttpRequestMessage request, UrlHelper url)
        {
            var context = default(Context);
            context = context ?? request.GetContext();

            var username = "Bonkers";
            //var username = request.GetUserName();

            var result = await context.Projects.InsertAsync(project.Name, project.Description, project.ProjectSponsor, project.ExecutiveSponsor, project.ProductSponsor,
                project.ProjectTypeId, project.NewPricingRules, project.Volume, project.RevenueAtList, project.DealFormEligible, project.NewTitles, project.NewAccounts,
                project.ProjectDetails, project.BusinessCase, project.Comments, project.CreatedDate, username,
                id =>
                {
                    project.Id = id;
                    return request.CreateResponse(HttpStatusCode.Created, project);
                },
                failed => request.CreateResponse(HttpStatusCode.BadRequest, "This Failed"),
                () => request.CreateResponse(HttpStatusCode.Unauthorized));

            return result;
        }

        //public static async Task<HttpResponseMessage> PutAsync(this ProjectResource project, HttpRequestMessage request, UrlHelper url)
        //{
        //    var context = default(Context);
        //    context = context ?? request.GetContext();

        //    var username = "Bonkers";
        //    //var username = request.GetUserName();

        //    var result = await context.Projects.UpdateAsync(project.Id, project.Name, project.Description, project.IsComplete, username,
        //        () => { return request.CreateResponse(HttpStatusCode.Created, project); },
        //        failed => request.CreateResponse(HttpStatusCode.BadRequest, "This failed"),
        //        () => request.CreateResponse(HttpStatusCode.Unauthorized));

        //    return result;

        //}

        public static async Task<HttpResponseMessage> DeleteAsync(this ProjectResource project, HttpRequestMessage request)
        {
            var context = default(Context);
            context = context ?? request.GetContext();

            var username = "Bonkers";
            //var username = request.GetUserName();

            if (project.Id == default(long))
                return request.CreateResponse(HttpStatusCode.BadRequest, "Id is required");

            return await context.Projects.DeleteAsync(project.Id, username,
                () => request.CreateResponse(HttpStatusCode.OK),
                failed => request.CreateResponse(HttpStatusCode.Conflict, failed));
        }
    }
}