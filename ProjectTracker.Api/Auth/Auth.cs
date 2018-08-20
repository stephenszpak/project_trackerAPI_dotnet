using Microsoft.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace ProjectTracker.Api.Auth
{
    public class Auth
    {
        public class InternalOnly : AuthorizeAttribute
        {
            protected override bool IsAuthorized(HttpActionContext actionContext)
            {
                #region stuff

                //Code to validate jwt
                //var validationParameters = new TokenValidationParameters()
                //{
                //    ValidIssuer = issuer,
                //    ValidAudience = clientId,
                //    IssuerSigningKey = securityKey
                //};

                //SecurityToken token2 = new JwtSecurityToken();
                //var tokenHandler = new JwtSecurityTokenHandler();
                //var principal = tokenHandler.ValidateToken(jwt, validationParameters, out token2);
                //var isValude = principle != null;


                //var authApiKey = actionContext.Request.Headers.Authorization.ToString(); 

                #endregion

                try
                {
                    var headerValues = actionContext.Request.Headers.GetValues("api_key");
                    var headerApiKey = headerValues.FirstOrDefault();

                    var apiKey = CloudConfigurationManager.GetSetting("api.key");
                    return headerApiKey == apiKey;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }
    }
}