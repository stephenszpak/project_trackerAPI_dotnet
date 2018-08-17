using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.SolutionExtensions
{
    public static class HeaderExtensions
    {
        public static string GetUserName(this HttpRequestMessage request)
        {
            try
            {
                IEnumerable<string> authHeaders;
                if (request.Headers.TryGetValues("Authorization", out authHeaders) || authHeaders.Count() > 1)
                {

                    var jwtStringPossibleBearer = authHeaders.ElementAt(0);
                    var securityClientJwtString = jwtStringPossibleBearer.ToLower().StartsWith("bearer ")
                        ? jwtStringPossibleBearer.Substring(7)
                        : jwtStringPossibleBearer;
                    //var securityClientJwt = new JwtSecurityToken(securityClientJwtString);
                    //var claimsDict = securityClientJwt.Claims.ToDictionary(claim => claim.Type, claim => claim.Value);
                    //return claimsDict.Select(claim => new Claim(claim.Key, claim.Value)).ToList();
                    var jwtToken = new JwtSecurityToken(securityClientJwtString);
                    return jwtToken.Audiences.FirstOrDefault();
                }
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
                //throw new ArgumentException("Problem getting user id from Authorization header");
            }
        }
    }
}
