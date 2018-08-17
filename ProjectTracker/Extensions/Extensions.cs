using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace ProjectTracker.Extensions
{
    public static class Extensions
    {
       public static string GetTokenFromAuthorizationHeader(this AuthenticationHeaderValue header)
        {
            try
            {
                var tokenRaw = header.ToString();
                return tokenRaw.ToLower().StartsWith("bearer ") ? tokenRaw.Substring(7) : tokenRaw;
            }
            catch
            {
                throw new ArgumentException("Problem getting user id from Auth Header");
            }
        }
    }
}
