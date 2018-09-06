using System.Configuration;
using System.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using ProjectTracker.Api;
using AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode;

[assembly: OwinStartup(typeof(Startup))]

namespace ProjectTracker.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var appSettings = WebConfigurationManager.AppSettings;
            var config = new HttpConfiguration();

            //// If CORS settings are present in Web.config
            //if (!string.IsNullOrWhiteSpace(appSettings["cors:Origins"]))
            //{
            //    // Load CORS settings from Web.config
            //    var corsPolicy = new EnableCorsAttribute(
            //        appSettings["cors:Origins"],
            //        appSettings["cors:Headers"],
            //        appSettings["cors:Methods"]);

            //    // Enable CORS for ASP.NET Identity
            //    app.UseCors(new CorsOptions
            //    {
            //        PolicyProvider = new CorsPolicyProvider
            //        {
            //            PolicyResolver = request =>
            //                request.Path.Value == "/Authenticate"
            //                    ? corsPolicy.GetCorsPolicyAsync(null, CancellationToken.None)
            //                    : Task.FromResult<CorsPolicy>(null)
            //        }
            //    });

            //    // Enable CORS for Web API
            //    config.EnableCors(corsPolicy);
            //}

            //app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            ConfigureAuth(app);
            ConfigureOAuth(app);

            app.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            var authServerIssuer = ConfigurationManager.AppSettings["AuthServer.issuer"];
            var authServerPublicKey = ConfigurationManager.AppSettings["AuthServer.publicKey"];

            // Api controllers with an [Authorize] attribute will be validated with JWT
            // Pass in all public keys for tokens we will be receiving
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidIssuers = new[] { authServerIssuer },
                        IssuerSigningTokens = new[] { Cryptography.Security.GetRsaSecurityToken(authServerPublicKey) },
                    },
                    AuthenticationMode = AuthenticationMode.Active,
                    Provider = new OAuthBearerAuthenticationProvider
                    {
                        OnValidateIdentity = context =>
                        {
                            //context.Ticket.Identity.AddClaim(new System.Security.Claims.Claim("newCustomClaim", "newValue"));
                            return Task.FromResult<object>(null);
                        }
                    }
                });
        }
    }
}