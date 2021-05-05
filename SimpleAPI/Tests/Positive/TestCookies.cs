using Newtonsoft.Json;
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
        [Test, Order(9)]
        [TestCase("fakesession=fake-cookie-session-value")]
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
