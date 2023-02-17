// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using ThreeStudio.IPFS.Internal;
using ThreeStudio.IPFS.Tests.Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ThreeStudio.IPFS.Tests
{
    /// <summary>
    /// These tests reach out to an IPFS HTTP Gateway.
    ///
    /// This mean that all tests need to be run single-threaded (one after another) to avoid hammering the service by
    /// sending too many requests in a short time frame and therefore get temporarily IP-banned.
    /// </summary>
    [TestFixture]
    [SingleThreaded]
    public class IpfsTest
    {
        private string _tmpDir;

        [OneTimeSetUp]
        public void SetupOnce()
        {
            Ipfs.SetDebugLogEnabled(true, Ipfs.DebugMode.UploadFileOrData, Ipfs.DebugMode.DownloadFileOrGetData);
        }

        [SetUp]
        public void Setup()
        {
            _tmpDir = TestUtils.CreateTempFilepath();
        }

        [TearDown]
        public void TearDown()
        {
            TestUtils.DeleteTempFilepath(_tmpDir);
        }

        [Test]
        public async Task IpfsGetData()
        {
            await Task.Delay(500);

            bool wait = true;
            IpfsFunctionLibrary.GetData(
                TestIpfsConstants.DefaultIpfsHttpGatewayConfig,
                TestIpfsConstants.AddressTestImage,
                delegate(bool success, string errorMessage, HttpResponse response, byte[] data)
                {
                    Assert.That(success, Is.True, $"success. Error Message: {errorMessage}");
                    Assert.That(errorMessage, Is.Empty, "errorMessage");
                    Assert.That(data, Is.Not.Null, "data");
                    Assert.That(data.Length, Is.EqualTo(55368), "data.Length");

                    wait = false;
                });

            while(wait)
            {
                await Task.Yield();
            }
        }

        [Test]
        public async Task IpfsGetDataAsync()
        {
            await Task.Delay(500);

            (bool success, string errorMessage, HttpResponse response, byte[] data) result = await IpfsFunctionLibrary.GetDataAsync(
                TestIpfsConstants.DefaultIpfsHttpGatewayConfig,
                TestIpfsConstants.AddressTestImage);

            Assert.That(result.success, Is.True, $"success. Error Message: {result.errorMessage}");
            Assert.That(result.errorMessage, Is.Empty, "errorMessage");
            Assert.That(result.data, Is.Not.Null, "data");
            Assert.That(result.data.Length, Is.EqualTo(55368), "data.Length");
        }

        [Test]
        public async Task IpfsGetDataAsString()
        {
            await Task.Delay(500);

            bool wait = true;
            IpfsFunctionLibrary.GetDataAsString(
                TestIpfsConstants.DefaultIpfsHttpGatewayConfig,
                TestIpfsConstants.AddressTestString,
                delegate(bool success, string errorMessage, HttpResponse response, string dataString)
                {
                    Assert.That(success, Is.True, $"success. Error Message: {errorMessage}");
                    Assert.That(errorMessage, Is.Empty, "errorMessage");
                    Assert.That(dataString, Is.Not.Null, "dataString");
                    Assert.That(dataString.Length, Is.EqualTo(30), "dataString.Length");

                    wait = false;
                });

            while(wait)
            {
                await Task.Yield();
            }
        }

        [Test]
        public async Task IpfsGetDataAsStringAsync()
        {
            await Task.Delay(500);

            (bool success, string errorMessage, HttpResponse response, string dataString) result = await IpfsFunctionLibrary.GetDataAsStringAsync(
                TestIpfsConstants.DefaultIpfsHttpGatewayConfig,
                TestIpfsConstants.AddressTestString);

            Assert.That(result.success, Is.True, $"success. Error Message: {result.errorMessage}");
            Assert.That(result.errorMessage, Is.Empty, "errorMessage");
            Assert.That(result.dataString, Is.Not.Null, "dataString");
            Assert.That(result.dataString.Length, Is.EqualTo(30), "dataString.Length");
        }

        [Test]
        public async Task IpfsGetDataAsImage()
        {
            await Task.Delay(500);

            bool wait = true;
            IpfsFunctionLibrary.GetDataAsImage(
                TestIpfsConstants.DefaultIpfsHttpGatewayConfig,
                TestIpfsConstants.AddressTestImage,
                delegate(bool success, string errorMessage, HttpResponse response, Texture2D texture)
                {
                    Assert.That(success, Is.True, $"success. Error Message: {errorMessage}");
                    Assert.That(errorMessage, Is.Empty, "errorMessage");
                    Assert.That(texture, Is.Not.Null, "texture");
                    Assert.That(texture.width, Is.EqualTo(1842), "texture.width");
                    Assert.That(texture.height, Is.EqualTo(1384), "texture.height");

                    wait = false;
                });

            while(wait)
            {
                await Task.Yield();
            }
        }

        [Test]
        public async Task IpfsGetDataAsImageAsync()
        {
            await Task.Delay(500);

            (bool success, string errorMessage, HttpResponse response, Texture2D texture) result = await IpfsFunctionLibrary.GetDataAsImageAsync(
                TestIpfsConstants.DefaultIpfsHttpGatewayConfig,
                TestIpfsConstants.AddressTestImage);

            Assert.That(result.success, Is.True, $"success. Error Message: {result.errorMessage}");
            Assert.That(result.errorMessage, Is.Empty, "errorMessage");
            Assert.That(result.texture, Is.Not.Null, "texture");
            Assert.That(result.texture.width, Is.EqualTo(1842), "texture.width");
            Assert.That(result.texture.height, Is.EqualTo(1384), "texture.height");
        }

        [Test]
        public async Task IpfsFileDownload()
        {
            await Task.Delay(500);

            string tmpFile = Path.Combine(_tmpDir, "downloaded-file");

            bool wait = true;
            IpfsFunctionLibrary.DownloadFile(
                TestIpfsConstants.DefaultIpfsHttpGatewayConfig,
                TestIpfsConstants.AddressTestImage,
                tmpFile,
                false,
                false,
                delegate(bool success, string errorMessage, HttpResponse response)
                {
                    Assert.That(success, Is.True, $"success. Error Message: {errorMessage}");
                    Assert.That(errorMessage, Is.Empty, "errorMessage");
                    Assert.That(response, Is.Not.Null, "response");

                    FileInfo fileInfo = new FileInfo(tmpFile);
                    Assert.That(fileInfo, Is.Not.Null, "fileInfo");
                    Assert.That(fileInfo.Exists, Is.True, "fileInfo.Exists");
                    Assert.That(fileInfo.Length, Is.EqualTo(55368), "fileInfo.Length");

                    string actualSha256 = StringUtils.BytesToHex(Sha256.Hash(File.ReadAllBytes(tmpFile)));
                    Assert.That(actualSha256, Is.EqualTo("f498c75e3632d953ae79f4c7f4c25cc1697a1ab21436dda924c9a0500fa81ee2"), "calculated SHA256");

                    wait = false;
                });

            while(wait)
            {
                await Task.Yield();
            }
        }

        [Test]
        public async Task IpfsFileDownloadAsync()
        {
            await Task.Delay(500);

            string tmpFile = Path.Combine(_tmpDir, "downloaded-file");

            (bool success, string errorMessage, HttpResponse response) result = await IpfsFunctionLibrary.DownloadFileAsync(
                TestIpfsConstants.DefaultIpfsHttpGatewayConfig,
                TestIpfsConstants.AddressTestImage,
                tmpFile,
                false,
                false);

            Assert.That(result.success, Is.True, $"success. Error Message: {result.errorMessage}");
            Assert.That(result.errorMessage, Is.Empty, "errorMessage");
            Assert.That(result.response, Is.Not.Null, "response");

            FileInfo fileInfo = new FileInfo(tmpFile);
            Assert.That(fileInfo, Is.Not.Null, "fileInfo");
            Assert.That(fileInfo.Exists, Is.True, "fileInfo.Exists");
            Assert.That(fileInfo.Length, Is.EqualTo(55368), "fileInfo.Length");

            string actualSha256 = StringUtils.BytesToHex(Sha256.Hash(await File.ReadAllBytesAsync(tmpFile)));
            Assert.That(actualSha256, Is.EqualTo("f498c75e3632d953ae79f4c7f4c25cc1697a1ab21436dda924c9a0500fa81ee2"), "calculated SHA256");
        }

        [Test]
        public async Task IpfsFileUpload()
        {
            await Task.Delay(500);

            const string saveAs = "upload.txt";
            string tmpFile = Path.Combine(_tmpDir, saveAs);

            string fileContent = "Test content " + Random.Range(0, int.MaxValue);
            await File.WriteAllTextAsync(tmpFile, fileContent);

            bool wait = true;
            IpfsFunctionLibrary.UploadFile(
                TestIpfsConstants.DefaultIpfsPinningServiceConfig,
                TestIpfsConstants.BearerToken_Web3Storage,
                tmpFile,
                saveAs,
                delegate(bool success, string errorMessage, HttpResponse response, string cid)
                {
                    Assert.That(success, Is.True, $"success. Error Message: {errorMessage}");
                    Assert.That(errorMessage, Is.Empty, "errorMessage");
                    Assert.That(response, Is.Not.Null, "response");
                    Assert.That(cid, Is.Not.Null.And.Not.Empty, "cid");

                    wait = false;
                });

            while(wait)
            {
                await Task.Yield();
            }
        }

        [Test]
        public async Task IpfsFileUploadAsync()
        {
            await Task.Delay(500);

            const string saveAs = "upload.txt";
            string tmpFile = Path.Combine(_tmpDir, saveAs);

            string fileContent = "Test content " + Random.Range(0, int.MaxValue);
            await File.WriteAllTextAsync(tmpFile, fileContent);

            (bool success, string errorMessage, HttpResponse response, string cid) result = await IpfsFunctionLibrary.UploadFileAsync(
                TestIpfsConstants.DefaultIpfsPinningServiceConfig,
                TestIpfsConstants.BearerToken_Web3Storage,
                tmpFile,
                saveAs);

            Assert.That(result.success, Is.True, $"success. Error Message: {result.errorMessage}");
            Assert.That(result.errorMessage, Is.Empty, "errorMessage");
            Assert.That(result.response, Is.Not.Null, "response");
            Assert.That(result.cid, Is.Not.Null.And.Not.Empty, "cid");
        }
    }
}