using Newtonsoft.Json;
using NUnit.Framework;
using SimpleAPI.Models;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SimpleAPI.Utils;


namespace SimpleAPI.Tests.Positive
{
    [TestFixture]
    class TestPositiveFlow : BaseTest
    {
        private int GetItemsCount()
        {
            request = new HttpRequestMessage(HttpMethod.Get, Endpoints.info);
            request.Headers.Add("Authorization", "Bearer " + token);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<Info>(content.Result);
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.Result.StatusCode);
            return int.Parse(deserialized.storage.Split(' ')[0]);
        }

        [Test, Order(2)]
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

        [Test, Order(3)]
        [TestCase("empty")]
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

        [Test, Order(4)]
        [TestCaseSource("ItemTestData")]
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

        [Test, Order(5)]
        [TestCaseSource("ItemTestData")]
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

        [Test, Order(6)]
        [TestCaseSource("ItemTestData")]
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

        [Test, Order(7)]
        [TestCase("val1", "val2", false, 1000)]
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

        [Test, Order(8)]
        [TestCase(0)]
        public void TestDelete(int id)
        {
            var storageBefore = GetItemsCount();

            request = new HttpRequestMessage(HttpMethod.Delete, Endpoints.delete + id);
            request.Headers.Add("Authorization", "Bearer " + token);
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<Send>(content.Result);
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.Result.StatusCode);
            Assert.AreEqual(deserialized.status, "SUCCESSFUL");
            Assert.AreEqual(deserialized.operation, "delete");
            Assert.AreEqual(deserialized.item_id, id);

            var storageAfter = GetItemsCount();
            Assert.AreEqual(storageBefore - storageAfter, 1);
        }
    }
}
