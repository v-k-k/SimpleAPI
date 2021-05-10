using Allure.Commons;
using Newtonsoft.Json;
using NUnit.Allure.Attributes;
using NUnit.Framework;
using SimpleAPI.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAPI.Tests.Negative
{
    [TestFixture]
    class TestNegativeFlow : BaseTest
    {
        [Test(Author = "Me", Description = "Checks info"), Order(10)]
        [TestCase("empty")]
        [AllureTag("TC-10")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureIssue("ISSUE-10")]
        [AllureTms("My-TMS")]
        [AllureOwner("Me")]
        [AllureSuite("Negative")]
        [AllureSubSuite("Test GET info")]
        public void TestInfo(string storageState)
        {
            request = new HttpRequestMessage(HttpMethod.Get, Endpoints.info);
            request.Headers.Add("Authorization", "Bearer " + token);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<Info>(content.Result);
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.Result.StatusCode);
            Assert.AreEqual(deserialized.token, token);
            Assert.AreNotEqual(deserialized.storage, storageState);
        }

        [Test(Author = "Me", Description = "Checks post send"), Order(11)]
        [TestCase((HttpStatusCode)422, "{{\"foo\": \"bar\"}}")]
        [AllureTag("TC-11")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureIssue("ISSUE-11")]
        [AllureTms("My-TMS")]
        [AllureOwner("Me")]
        [AllureSuite("Negative")]
        [AllureSubSuite("Test POST send")]
        public void TestSending(HttpStatusCode expectedStatus, string fakeData)
        {
            request = new HttpRequestMessage(HttpMethod.Post, Endpoints.send)
            {
                Content = new StringContent(fakeData, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", "Bearer " + token);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            Assert.AreEqual(expectedStatus, responseMessage.Result.StatusCode);
        }

        [Test(Author = "Me", Description = "Checks fake item"), Order(12)]
        [TestCase(100500)]
        [AllureTag("TC-12")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureIssue("ISSUE-12")]
        [AllureTms("My-TMS")]
        [AllureOwner("Me")]
        [AllureSuite("Negative")]
        [AllureSubSuite("Test GET info fake")]
        public void TestShow(int fakeItemId)
        {
            request = new HttpRequestMessage(HttpMethod.Get, Endpoints.info + fakeItemId);
            request.Headers.Add("Authorization", "Bearer " + token);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<Fail>(content.Result);
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.Result.StatusCode);
            Assert.AreEqual(deserialized.status, "FAILED");
            Assert.AreEqual(deserialized.operation, "get");
            Assert.AreEqual(deserialized.message, $"no item with id {fakeItemId}");
        }

        [Test(Author = "Me", Description = "Checks deletion"), Order(13)]
        [TestCase(100500)]
        [AllureTag("TC-13")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureIssue("ISSUE-13")]
        [AllureTms("My-TMS")]
        [AllureOwner("Me")]
        [AllureSuite("Negative")]
        [AllureSubSuite("Test DELETE item")]
        public void TestDelete(int fakeItemId)
        {
            request = new HttpRequestMessage(HttpMethod.Delete, Endpoints.delete + fakeItemId);
            request.Headers.Add("Authorization", "Bearer " + token);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<Fail>(content.Result);
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.Result.StatusCode);
            Assert.AreEqual(deserialized.status, "FAILED");
            Assert.AreEqual(deserialized.operation, "delete");
            Assert.AreEqual(deserialized.message, $"no item with id {fakeItemId}");
        }
    }
}
