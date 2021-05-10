using Allure.Commons;
using Newtonsoft.Json;
using NUnit.Allure.Attributes;
using NUnit.Framework;
using SimpleAPI.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;


namespace SimpleAPI.Tests.Negative
{
    [TestFixture]
    class TestsWithoutAuth : BaseTest
    {
        [Test(Author = "Me", Description = "Checks endpoint without auth"), Order(1)]
        [AllureTag("TC-1")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureIssue("ISSUE-1")]
        [AllureTms("My-TMS")]
        [AllureOwner("Me")]
        [AllureSuite("Negative")]
        [AllureSubSuite("Test without authentication")]
        public void TestWithoutAuth()
        {
            request = new HttpRequestMessage(HttpMethod.Get, Endpoints.me);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<NotAuthenticated>(content.Result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, responseMessage.Result.StatusCode);
            Assert.AreEqual(deserialized.detail, "Not authenticated");
        }
    }
}
