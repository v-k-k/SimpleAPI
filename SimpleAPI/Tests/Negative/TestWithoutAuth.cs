using Newtonsoft.Json;
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
        [Test, Order(1)]
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
