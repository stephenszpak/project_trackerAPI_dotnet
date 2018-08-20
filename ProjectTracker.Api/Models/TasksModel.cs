using ProjectTracker.Api.Resources;
using ProjectTracker.Api.Resources.Operations;
using ProjectTracker.SolutionExtensions;
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
    public static class TasksModel
    {
        public static async Task<HttpResponseMessage> GetAsync(this TaskResource task, HttpRequestMessage request, UrlHelper url)
        {
            var context = default(Context);
            context = context ?? request.GetContext();

            var items = new List<TaskResource>();

            if (task.Id != default(long))
            {
                return await context.Tasks.FindTasksByIdAsync(task.Id, (name, description, isComplete) =>
                {
                    items.Add(new TaskResource()
                    {
                        Id = task.Id,
                        Name = name,
                        Description = description,
                        IsComplete = isComplete
                    });
                    return request.CreateResponse(HttpStatusCode.OK, items.ToArray());
                },
                () => request.CreateResponse(HttpStatusCode.OK, items.ToArray()),
                () => request.CreateResponse(HttpStatusCode.Unauthorized));
            }

            return await context.Tasks.GetAllTasksAsync((id, name, description, isComplete) =>
            {
                items.Add(new TaskResource
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    IsComplete = isComplete
                });
                return request.CreateResponse(HttpStatusCode.OK, items.ToArray());
            },
            () => request.CreateResponse(HttpStatusCode.OK, items.ToArray()),
            () => request.CreateResponse(HttpStatusCode.Unauthorized));
        }

        public static async Task<HttpResponseMessage> PostAsync(this TaskResource task, HttpRequestMessage request, UrlHelper url)
        {
            var context = default(Context);
            context = context ?? request.GetContext();

            var username = "Billy";
            //var username = request.GetUserName();

            var result = await context.Tasks.InsertAsync(task.Name, task.Description, task.IsComplete, username,
                id =>
                {
                    task.Id = id;
                    return request.CreateResponse(HttpStatusCode.Created, task);
                },
                failed => request.CreateResponse(HttpStatusCode.BadRequest, "This Failed"),
                () => request.CreateResponse(HttpStatusCode.Unauthorized));

            return result;
        }

        public static async Task<HttpResponseMessage> PutAsync(this TaskResource task, HttpRequestMessage request, UrlHelper url)
        {
            var context = default(Context);
            context = context ?? request.GetContext();

            var username = "Billy";
            //var username = request.GetUserName();

            var result = await context.Tasks.UpdateAsync(task.Id, task.Name, task.Description, task.IsComplete, username,
                () => { return request.CreateResponse(HttpStatusCode.Created, task); },
                failed => request.CreateResponse(HttpStatusCode.BadRequest, "This failed"),
                () => request.CreateResponse(HttpStatusCode.Unauthorized));

            return result;

        }

        public static async Task<HttpResponseMessage> DeleteAsync(this TaskResource task, HttpRequestMessage request)
        {
            var context = default(Context);
            context = context ?? request.GetContext();

            var username = "Billy";
            //var username = request.GetUserName();

            if (task.Id == default(long))
                return request.CreateResponse(HttpStatusCode.BadRequest, "Id is required");

            return await context.Tasks.DeleteAsync(task.Id, username,
                () => request.CreateResponse(HttpStatusCode.OK),
                failed => request.CreateResponse(HttpStatusCode.Conflict, failed));
        }
    }
}