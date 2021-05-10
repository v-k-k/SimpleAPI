using Allure.Commons;
using NUnit.Allure.Attributes;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleAPI.Tests.Positive
{
    [TestFixture]
    class TestCookies : BaseTest
    {
        [Test(Author = "Me", Description = "Checks cookies setting"), Order(9)]
        [TestCase("fakesession=fake-cookie-session-value")]
        [AllureTag("TC-9")]
        [AllureSeverity(SeverityLevel.normal)]
        [AllureIssue("ISSUE-9")]
        [AllureTms("My-TMS")]
        [AllureOwner("Me")]
        [AllureSuite("Positive")]
        [AllureSubSuite("Test cookies")]
        public void TestCookie(string expectedCookies)
        {
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            handler.UseCookies = true;
            client = new HttpClient(handler);
            request = new HttpRequestMessage(HttpMethod.Post, Endpoints.createCookie);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.Result.StatusCode);
            foreach (var cookie in cookies.GetCookies(new Uri(Endpoints.createCookie)))
                Assert.AreEqual(cookie.ToString(), expectedCookies);
        }
    }
}
