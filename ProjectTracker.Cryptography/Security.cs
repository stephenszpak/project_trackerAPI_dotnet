using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.IdentityModel.Tokens;

namespace ProjectTracker.Cryptography
{
    public static class Security
    {
        public static string Base64Encode(string text)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string value)
        {
            var base64EncodedBytes = Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public static string UrlBase64Encode(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return System.Web.HttpServerUtility.UrlTokenEncode(bytes);
        }
        public static string UrlBase64Decode(string text)
        {
            return Microsoft.IdentityModel.Tokens.Base64UrlEncoder.Decode(text);
        }
        public static SecurityToken GetRsaSecurityToken(string base64EncodedValue)
        {
            var xml = UrlBase64Decode(base64EncodedValue);
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(xml);
            return new RsaSecurityToken(rsa);
        }
    }
}
