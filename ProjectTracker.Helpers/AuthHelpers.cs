using ProjectTracker.Api.Resources.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Helpers
{
    public static class AuthHelpers
    {
        public static async Task<Jwt> CreateAuthorize(string username, string password)
        {
            var request = Requests.GetRequest();

            var authItem = new Authorize()
            {
                UserName = username,
                Password = password
            };

            var createAuthorize = await Api.Models.AuthorizeModel.PostAsync(authItem, request, new System.Web.Http.Routing.UrlHelper(request));

            var otherContent = await createAuthorize.Content.ReadAsStringAsync();
            return new Jwt() { Token = string.Empty };
        }
    }
}
