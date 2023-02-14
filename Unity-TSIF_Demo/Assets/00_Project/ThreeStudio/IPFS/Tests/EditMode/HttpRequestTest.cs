// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using _00_Project.ThreeStudio.IPFS.Tests.Shared;
using NUnit.Framework;
using ThreeStudio.IPFS.Http;
using ThreeStudio.IPFS.Internal;

namespace ThreeStudio.IPFS.Tests
{
    /// <summary>
    /// These tests reach out to an example REST API.
    ///
    /// This mean that all tests need to be run single-threaded (one after another) to avoid hammering the service by
    /// sending too many requests in a short time frame and therefore get temporarily IP-banned.
    /// </summary>
    [TestFixture]
    [SingleThreaded]
    public class HttpRequestTest
    {
        private struct HttpRequestTestData
        {
            public readonly string Url;
            public readonly string RequestBody;
            public readonly string ResponseBody;

            public HttpRequestTestData(string url, string requestBody, string responseBody)
            {
                Url = url;
                RequestBody = requestBody;
                ResponseBody = responseBody;
            }
        }

        private HttpRequestTestData _testGet;
        private HttpRequestTestData _testPost;
        private Dictionary<string, string> _defaultHeader;

        [OneTimeSetUp]
        public void SetupOnce()
        {
            Ipfs.SetDebugLogEnabled(true, Ipfs.DebugMode.UploadFileOrData, Ipfs.DebugMode.DownloadFileOrGetData);

            _defaultHeader = new Dictionary<string, string>
            {
                { "accept", "application/json" },
                { "Content-Type", "application/json" },
            };

            _testGet = new HttpRequestTestData(
                "https://reqres.in/api/users/2",
                null,
                File.ReadAllText(TestUtils.GetTestResourceFilepath("HttpRequestTest", "GET_Response_200.txt")));

            _testPost = new HttpRequestTestData(
                "https://reqres.in/api/login",
                File.ReadAllText(TestUtils.GetTestResourceFilepath("HttpRequestTest", "POST_Request.txt")),
                File.ReadAllText(TestUtils.GetTestResourceFilepath("HttpRequestTest", "POST_Response_200.txt")));
        }

        [Test]
        public async Task HttpRequestGet()
        {
            HttpRequestTestData testData = _testGet;

            await Task.Delay(500);

            bool wait = true;
            HttpRequest.SendGetRequest(
                testData.Url,
                "1",
                delegate(bool success, string errorMessage, HttpResponse response)
                {
                    Assert.That(success, Is.True, $"success. Error Message: {errorMessage}");
                    Assert.That(errorMessage, Is.Empty, "errorMessage");
                    Assert.That(response, Is.Not.Null, "response");
                    Assert.That(response.StatusCode, Is.EqualTo(200), "response.StatusCode");
                    Assert.That(StringUtils.BytesToString(response.Body), Is.EqualTo(testData.ResponseBody), "response.Body");

                    wait = false;
                });

            while(wait)
            {
                await Task.Yield();
            }
        }

        [Test]
        public async Task HttpRequestGetAsync()
        {
            HttpRequestTestData testData = _testGet;

            await Task.Delay(500);

            (bool success, string errorMessage, HttpResponse response) result = await HttpRequest.SendGetRequestAsync(testData.Url, "1");

            Assert.That(result.success, Is.True, $"success. Error Message: {result.errorMessage}");
            Assert.That(result.errorMessage, Is.Empty, "errorMessage");
            Assert.That(result.response, Is.Not.Null, "response");
            Assert.That(result.response.StatusCode, Is.EqualTo(200), "response.StatusCode");
            Assert.That(StringUtils.BytesToString(result.response.Body), Is.EqualTo(testData.ResponseBody), "response.Body");
        }

        [Test]
        public async Task HttpRequestPost()
        {
            HttpRequestTestData testData = _testPost;

            await Task.Delay(500);

            bool wait = true;
            HttpRequest.SendPostRequest(
                testData.Url,
                _defaultHeader,
                StringUtils.StringToBytes(testData.RequestBody),
                "1",
                delegate(bool success, string errorMessage, HttpResponse response)
                {
                    Assert.That(success, Is.True, $"success. Error Message: {errorMessage}");
                    Assert.That(errorMessage, Is.Empty, "errorMessage");
                    Assert.That(response, Is.Not.Null, "response");
                    Assert.That(response.StatusCode, Is.EqualTo(200), "response.StatusCode");
                    Assert.That(StringUtils.BytesToString(response.Body), Is.EqualTo(testData.ResponseBody), "response.Body");

                    wait = false;
                });

            while(wait)
            {
                await Task.Yield();
            }
        }

        [Test]
        public async Task HttpRequestPostAsync()
        {
            HttpRequestTestData testData = _testPost;

            await Task.Delay(500);

            (bool success, string errorMessage, HttpResponse response) result = await HttpRequest.SendPostRequestAsync(
                testData.Url,
                _defaultHeader,
                StringUtils.StringToBytes(testData.RequestBody),
                "1");

            Assert.That(result.success, Is.True, $"success. Error Message: {result.errorMessage}");
            Assert.That(result.errorMessage, Is.Empty, "errorMessage");
            Assert.That(result.response, Is.Not.Null, "response");
            Assert.That(result.response.StatusCode, Is.EqualTo(200), "response.StatusCode");
            Assert.That(StringUtils.BytesToString(result.response.Body), Is.EqualTo(testData.ResponseBody), "response.Body");
        }
    }
}