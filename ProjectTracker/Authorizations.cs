using Microsoft.Azure;
using System.IdentityModel.Tokens;
using ProjectTracker.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices.AccountManagement;

namespace ProjectTracker
{
    public class Authorizations
    {
        public Authorizations(Context context, IDataContext dataContext)
        {
        }

        public string CreateToken(string userName, int tokenExpirationInMinutes,
             string[] roles, string firstName, string lastName, string configNameOfIssuer = "AuthServer.issuer", string configNameOfRSAKey = "AuthServer.key")
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("audience", userName));
            if (roles != null && roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim("roles", role));
                }
            }

            var issued = DateTime.UtcNow;
            var validForDuration = DateTimeOffset.UtcNow + TimeSpan.FromMinutes(tokenExpirationInMinutes);
            var jwtToken = CreateToken(userName,
                issued, validForDuration, claims, firstName, lastName,
                configNameOfIssuer, configNameOfRSAKey);
            return jwtToken;

        }

        private string CreateToken(string clientId,
           DateTimeOffset? issued, DateTimeOffset? expires,
           IEnumerable<Claim> claims, string firstName, string lastName,
           string configNameOfIssuer = "AuthServer.issuer", string configNameOfRsaKey = "AuthServer.key")
        {
            var rsaProvider = RsaFromConfig(configNameOfRsaKey);
            var securityKey = new RsaSecurityKey(rsaProvider);

            var issuer = CloudConfigurationManager.GetSetting(configNameOfIssuer);
            if (string.IsNullOrEmpty(issuer))
                throw new SystemException("Issuer was not found in the configuration file");

            var token = new JwtSecurityToken(issuer, clientId, claims,
                issued.Value.UtcDateTime,
                expires.Value.UtcDateTime,
                new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256Signature,
                    SecurityAlgorithms.Sha256Digest));

            var handler = new JwtSecurityTokenHandler();

            token.Payload["first_name"] = firstName;
            token.Payload["last_name"] = lastName;


            var jwt = handler.WriteToken(token);

            //var validationParameters = new TokenValidationParameters()
            //{
            //    ValidIssuer = issuer,
            //    ValidAudience = clientId,
            //    IssuerSigningKey = securityKey
            //};

            //SecurityToken token2 = new JwtSecurityToken();
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var principal = tokenHandler.ValidateToken(jwt, validationParameters, out token2);

            return jwt;
        }

        public static RSACryptoServiceProvider RsaFromConfig(string configSettingName)
        {
            var secretAsRSAXmlBase64 = CloudConfigurationManager.GetSetting(configSettingName);
            if (string.IsNullOrEmpty(secretAsRSAXmlBase64))
                throw new SystemException("RSA public key was not found in the configuration file. AppSetting = " + configSettingName);
            var xml = Cryptography.Security.UrlBase64Decode(secretAsRSAXmlBase64);
            var rsaProvider = new RSACryptoServiceProvider();
            rsaProvider.FromXmlString(xml);
            return rsaProvider;
        }

        public async Task<TResult> AuthorizeAsync<TResult>(string userName, string password,
            Func<string, TResult> success,
            Func<string, TResult> failed,
            Func<TResult> unAuthorized)
        {

            using (var pc = new PrincipalContext(ContextType.Domain, "icg"))
            {
                if (!pc.ValidateCredentials(userName, password)) return unAuthorized();

                string[] output = null;

                using (var ctx = new PrincipalContext(ContextType.Domain))
                using (var user = UserPrincipal.FindByIdentity(ctx, userName))
                {
                    var firstName = string.Empty;
                    var lastName = string.Empty;
                    var displayName = string.Empty;

                    var roles = new List<string>();
                    if (user != null)
                    {
                        firstName = user.GivenName;
                        lastName = string.IsNullOrWhiteSpace(user.Surname) ? "service_account" : user.Surname;
                        displayName = user.DisplayName;
                        output = user.GetGroups()
                            .Select(x => x.SamAccountName)
                            .ToArray();
                  
                        var isAdmin = output.Contains(CloudConfigurationManager.GetSetting("AuthServer.Roles.Admin"));
                        var isUser = output.Contains(CloudConfigurationManager.GetSetting("AuthServer.Roles.User"));
                        var isDeveloper = output.Contains(CloudConfigurationManager.GetSetting("AuthServer.Roles.Developers"));
                        var readOnly = output.Contains(CloudConfigurationManager.GetSetting("AuthServer.Roles.ReadOnly"));
                        var deployedEnvironment = CloudConfigurationManager.GetSetting("Environment");

                        if (isAdmin || (isDeveloper && deployedEnvironment.Equals("Test")))
                        {
                            roles.Add(CustomUserRoles.Admin);
                            roles.Add(CustomUserRoles.User);
                            roles.Add(CustomUserRoles.ReadOnly);
                        }
                        else
                        {
                            if (isUser)
                            {
                                roles.Add(CustomUserRoles.User);
                                roles.Add(CustomUserRoles.ReadOnly);

                            }
                            if (readOnly) roles.Add(CustomUserRoles.ReadOnly);
                        }
                    }

                    if (roles.Count > 0)
                    {
                        var timeInMinutes =
                            int.Parse(CloudConfigurationManager.GetSetting("AuthServer.tokenExpiration.InMinutes"));
                        var token = CreateToken(userName, timeInMinutes, roles.ToArray(), firstName, lastName);
                        if (string.IsNullOrWhiteSpace(token)) return failed("Unable to authenticate user");
                        return success(token);

                    }
                    return unAuthorized();
                }
            }
        }
    }
}
