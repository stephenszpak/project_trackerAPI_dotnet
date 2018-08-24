using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectTracker.Helpers;

namespace ProjectTracker.Tests
{
    [TestClass]
    public class AuthTests
    {
        [TestMethod]
        public async Task AuthorizeAndGetToken()
        {
            // Only useful for local machine testing
            var username = "bill";
            var password = "srsdruM21!";

            var jwt = await AuthHelpers.CreateAuthorize(username, password);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(jwt.Token));
        }
    }
}
