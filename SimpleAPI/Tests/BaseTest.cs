using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Net.Http;
using SimpleAPI.Models;
using Newtonsoft.Json;
using System.Collections;

namespace SimpleAPI.Tests
{
    class BaseTest 
    {
        protected readonly string authString = "grant_type=password&username={0}&password={1}";
        protected HttpClient client;
        protected HttpRequestMessage request;
        protected string token;

        protected void GetToken()
        {
            request = new HttpRequestMessage(HttpMethod.Post, Endpoints.auth)
            {
                Content = new StringContent(string.Format(authString, Env.User1, Env.Password1), Encoding.UTF8, "application/x-www-form-urlencoded")
            };
            Task<HttpResponseMessage> responseMessage = client.SendAsync(request);
            var content = responseMessage.Result.Content.ReadAsStringAsync();
            var deserialized = JsonConvert.DeserializeObject<Token>(content.Result);
            token = deserialized.access_token;
        }

        protected static IEnumerable ItemTestData
        {
            get
            {
                yield return new TestCaseData(
                    new Item { field1 = "c1_val1", field2 = "c1_val2", field3 = true, id = 0 }
                    );
                yield return new TestCaseData(
                    new Item { field1 = "c2_val1", field2 = "c2_val2", field3 = false, id = 1 }
                    );
                yield return new TestCaseData(
                    new Item { field1 = "c3_val1", field2 = "c3_val2", field3 = true, id = 2 }
                    );
            }
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            Env.Initialize();
            client = new HttpClient();
            string testMethodName = TestContext.CurrentContext.Test.Name;
            if (!(testMethodName.Contains("Cookies") || testMethodName.Contains("WithoutAuth")))
                GetToken();
        }
    }
}
