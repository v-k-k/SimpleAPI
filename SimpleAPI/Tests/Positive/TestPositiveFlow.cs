using Newtonsoft.Json;
using NUnit.Framework;
using SimpleAPI.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SimpleAPI.Utils;
using NUnit.Allure.Attributes;
using Allure.Commons;
using System;

namespace SimpleAPI.Tests.Positive
{
    [TestFixture]
    class TestPositiveFlow : BaseTest
    {
        [Test(Author = "Me", Description = "Checks Me endpoint"), Order(2)]
        [AllureTag("TC-2")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureIssue("ISSUE-2")]
        [AllureTms("My-TMS")]
        [AllureOwner("Me")]
        [AllureSuite("Positive")]
        [AllureSubSuite("Test Me")]
        public void TestMe()
        {
            request = new HttpRequestMessage(HttpMethod.Get, Endpoints.me);
            request.Headers.Add("Authorization", "Bearer " + token);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<Authenticated>(content.Result);
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.Result.StatusCode);
            Assert.AreEqual(deserialized.username, Env.User1);
            Assert.AreEqual(deserialized.hashed_password, $"fakehashed{Env.Password1}");
        }

        [Test(Author = "Me", Description = "Checks info"), Order(3)]
        [TestCase("empty")]
        [AllureTag("TC-3")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureIssue("ISSUE-3")]
        [AllureTms("My-TMS")]
        [AllureOwner("Me")]
        [AllureSuite("Positive")]
        [AllureSubSuite("Test info")]
        public void TestInfo(string storageState)
        {
            request = new HttpRequestMessage(HttpMethod.Get, Endpoints.info);
            request.Headers.Add("Authorization", "Bearer " + token);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<Info>(content.Result);
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.Result.StatusCode);
            Assert.AreEqual(deserialized.token, token);
            Assert.AreEqual(deserialized.storage, storageState);
        }

        [Test(Author = "Me", Description = "Checks sending"), Order(4)]
        [TestCaseSource("ItemTestData")]
        [AllureTag("TC-4")]
        [AllureSeverity(SeverityLevel.critical)]
        [AllureIssue("ISSUE-4")]
        [AllureTms("My-TMS")]
        [AllureOwner("Me")]
        [AllureSuite("Positive")]
        [AllureSubSuite("Test send")]
        public void TestSending(Item item)
        {
            request = new HttpRequestMessage(HttpMethod.Post, Endpoints.send)
            {
                Content = new StringContent(item.ToJson(), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", "Bearer " + token);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<Send>(content.Result);
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.Result.StatusCode);
            Assert.AreEqual(deserialized.status, "SUCCESSFUL");
            Assert.AreEqual(deserialized.operation, "post");
        }

        [Test(Author = "Me", Description = "Checks showing"), Order(5)]
        [TestCaseSource("ItemTestData")]
        [AllureTag("TC-5")]
        [AllureSeverity(SeverityLevel.blocker)]
        [AllureIssue("ISSUE-5")]
        [AllureTms("My-TMS")]
        [AllureOwner("Me")]
        [AllureSuite("Positive")]
        [AllureSubSuite("Test show")]
        public void TestShow(Item item)
        {
            request = new HttpRequestMessage(HttpMethod.Get, Endpoints.info + item.id);
            request.Headers.Add("Authorization", "Bearer " + token);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<ItemData>(content.Result);
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.Result.StatusCode);
            Assert.AreEqual(deserialized.id, item.id);
            Assert.AreEqual(deserialized.field1, item.field1);
            Assert.AreEqual(deserialized.field2, item.field2);
            Assert.AreEqual(deserialized.field3, item.field3);
        }

        [Test(Author = "Me", Description = "Checks updating"), Order(6)]
        [TestCaseSource("ItemTestData")]
        [AllureTag("TC-6")]
        [AllureSeverity(SeverityLevel.blocker)]
        [AllureIssue("ISSUE-6")]
        [AllureTms("My-TMS")]
        [AllureOwner("Me")]
        [AllureSuite("Positive")]
        [AllureSubSuite("Test update")]
        public void TestUpdate(Item item)
        {
            item.field1 = StringHelper.RandomString(5);
            item.field2 = StringHelper.RandomString(5);
            request = new HttpRequestMessage(HttpMethod.Put, Endpoints.update)
            {
                Content = new StringContent(item.ToJson(true), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", "Bearer " + token);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<ItemData>(content.Result);
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.Result.StatusCode);
            Assert.AreEqual(deserialized.id, item.id);
            Assert.AreEqual(deserialized.field1, item.field1);
            Assert.AreEqual(deserialized.field2, item.field2);
            Assert.AreEqual(deserialized.field3, item.field3);
        }

        [Test(Author = "Me", Description = "Checks adding with PUT"), Order(7)]
        [TestCase("val1", "val2", false, 1000)]
        [AllureTag("TC-7")]
        [AllureSeverity(SeverityLevel.trivial)]
        [AllureIssue("ISSUE-7")]
        [AllureTms("My-TMS")]
        [AllureOwner("Me")]
        [AllureSuite("Positive")]
        [AllureSubSuite("Test add with PUT")]
        public void TestAddWithPut(string field1, string field2, bool field3, int id)
        {
            var item = new Item { field1 = field1, field2 = field2, field3 = field3, id = id };
            request = new HttpRequestMessage(HttpMethod.Put, Endpoints.update)
            {
                Content = new StringContent(item.ToJson(true), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", "Bearer " + token);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<Send>(content.Result);
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.Result.StatusCode);
            Assert.AreEqual(deserialized.status, "SUCCESSFUL");
            Assert.AreEqual(deserialized.operation, "put");
            Assert.AreEqual(deserialized.item_id, item.id);
        }

        [Test(Author = "Me", Description = "Checks deletion"), Order(8)]
        [TestCase(0)]
        [AllureTag("TC-8")]
        [AllureSeverity(SeverityLevel.minor)]
        [AllureIssue("ISSUE-8")]
        [AllureTms("My-TMS")]
        [AllureOwner("Me")]
        [AllureSuite("Positive")]
        [AllureSubSuite("Test deletion")]
        public void TestDelete(int id)
        {
            var storageBefore = GetItemsCount();
            request = new HttpRequestMessage(HttpMethod.Delete, Endpoints.delete + id);
            request.Headers.Add("Authorization", "Bearer " + token);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<Send>(content.Result);

            var stepOneResult = new StepResult();
            stepOneResult.name = "Check response";
            AllureLifecycle.Instance.StartStep(Guid.NewGuid().ToString(), stepOneResult);
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.Result.StatusCode);
            Assert.AreEqual(deserialized.status, "SUCCESSFUL");
            Assert.AreEqual(deserialized.operation, "delete");
            Assert.AreEqual(deserialized.item_id, id);
            AllureLifecycle.Instance.StopStep(step => stepOneResult.status = Status.passed);

            var stepTwoResult = new StepResult();
            stepTwoResult.name = "Check items amount changed";
            AllureLifecycle.Instance.StartStep(Guid.NewGuid().ToString(), stepTwoResult);
            var storageAfter = GetItemsCount();
            Assert.AreEqual(storageBefore - storageAfter, 1);
            AllureLifecycle.Instance.StopStep(step => stepTwoResult.status = Status.passed);
        }
    }
}
