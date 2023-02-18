// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using ThreeStudio.IPFS.Tests.Shared;
using NUnit.Framework;
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
    public class IpfsUploadTest
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
        public async Task IpfsDataUploadWithoutPathAsync()
        {
            await Task.Delay(500);

            await UploadAndDownloadWithVerification(null);
        }

        [Test]
        public async Task IpfsDataUploadWithPath1Async()
        {
            await Task.Delay(500);

            await UploadAndDownloadWithVerification("test.txt");
        }

        [Test]
        public async Task IpfsDataUploadWithPath2Async()
        {
            await Task.Delay(500);

            await UploadAndDownloadWithVerification("sub-folder/test.txt");
        }

        private async Task UploadAndDownloadWithVerification(string ipfsPath)
        {
            // =================================================================
            // Prepare data for uploading
            // =================================================================

            string fileContent = "Test content " + DateTime.Now + " - " + Random.Range(0, int.MaxValue);
            string fileToUpload = Path.Combine(_tmpDir, "file-to-upload");
            await File.WriteAllTextAsync(fileToUpload, fileContent);
            Assert.That(File.Exists(fileToUpload), Is.True, "FileToUpload.Exists");

            // =================================================================
            // Upload data
            // =================================================================

            (bool success, string errorMessage, HttpResponse response, string cid) resultUpload = await IpfsFunctionLibrary.UploadFileAsync(
                TestIpfsConstants.DefaultIpfsPinningServiceConfig,
                TestIpfsConstants.BearerToken_Web3Storage,
                fileToUpload,
                ipfsPath);

            Assert.That(resultUpload.success, Is.True, $"resultUpload.success. Error Message: {resultUpload.errorMessage}");
            Assert.That(resultUpload.errorMessage, Is.Empty, "resultUpload.errorMessage");
            Assert.That(resultUpload.response, Is.Not.Null, "resultUpload.response");
            Assert.That(resultUpload.cid, Is.Not.Null.And.Not.Empty, "resultUpload.cid");

            // =================================================================
            // Download the just uploaded data
            // =================================================================

            string writeToFilepath = Path.Combine(_tmpDir, "downloaded-file");

            IpfsAddress ipfsAddress = new IpfsAddress(resultUpload.cid, ipfsPath);

            (bool success, string errorMessage, HttpResponse response) resultDownload = await IpfsFunctionLibrary.DownloadFileAsync(
                TestIpfsConstants.DefaultIpfsHttpGatewayConfig,
                ipfsAddress,
                writeToFilepath,
                false,
                false);

            Assert.That(resultDownload.success, Is.True, $"resultDownload.success. Error Message: {resultDownload.errorMessage}");
            Assert.That(resultDownload.errorMessage, Is.Empty, "resultDownload.errorMessage");
            Assert.That(resultDownload.response, Is.Not.Null, "resultDownload.response");

            Assert.That(File.Exists(writeToFilepath), Is.True, "writeToFilepath.Exists");

            // =================================================================
            // Verify downloaded data with data we uploaded earlier
            // =================================================================

            await VerifyFilesAreEqual(fileToUpload, writeToFilepath);

            // We want to verify that downloading a file will fail if we don't use a path if that given file was
            // originally uploaded using a path.
            await VerifyDownloadWithoutPathWillFailIfPathSpecified(ipfsAddress, fileToUpload);
        }

        /// <summary>
        /// This function verifies that two given files are equal in length and content.
        /// </summary>
        /// <param name="filepathA">File A</param>
        /// <param name="filepathB">File B</param>
        private async Task VerifyFilesAreEqual(string filepathA, string filepathB)
        {
            byte[] dataOfFileA = await File.ReadAllBytesAsync(filepathA);
            byte[] dataOfFileB = await File.ReadAllBytesAsync(filepathB);

            Assert.That(dataOfFileA, Is.Not.Null, "dataOfFileA.NotNull");
            Assert.That(dataOfFileB, Is.Not.Null, "dataOfFileB.NotNull");
            Assert.That(dataOfFileA.Length, Is.EqualTo(dataOfFileB.Length), "size of file A and B are the same");

            bool sameBytes = true;
            for(int index = 0; index < dataOfFileA.Length; index++)
            {
                bool isSameByte = dataOfFileA[index] == dataOfFileB[index];
                if(!isSameByte)
                {
                    sameBytes = false;
                    break;
                }
            }

            Assert.That(sameBytes, Is.True, "sameBytes");
        }

        /// <summary>
        /// This function verifies that two given files are not equal in length and content.
        /// </summary>
        /// <param name="filepathA">File A</param>
        /// <param name="filepathB">File B</param>
        private async Task VerifyFilesAreNotEqual(string filepathA, string filepathB)
        {
            byte[] dataOfFileA = await File.ReadAllBytesAsync(filepathA);
            byte[] dataOfFileB = await File.ReadAllBytesAsync(filepathB);

            Assert.That(dataOfFileA, Is.Not.Null, "dataOfFileA.NotNull");
            Assert.That(dataOfFileB, Is.Not.Null, "dataOfFileB.NotNull");

            bool sameLength = dataOfFileA.Length == dataOfFileB.Length;
            if(!sameLength)
            {
                return;
            }

            // Files of same length can still have different bytes so we need to check this next
            bool sameBytes = true;
            for(int index = 0; index < dataOfFileA.Length; index++)
            {
                bool isSameByte = dataOfFileA[index] == dataOfFileB[index];
                if(!isSameByte)
                {
                    sameBytes = false;
                    break;
                }
            }

            Assert.That(sameBytes, Is.False, "sameBytes");
        }

        private async Task VerifyDownloadWithoutPathWillFailIfPathSpecified(IpfsAddress goodIpfsAddress, string originalFile)
        {
            if(goodIpfsAddress.Path == null)
            {
                return;
            }

            IpfsAddress badIpfsAddress = new IpfsAddress(goodIpfsAddress.Cid, null);

            string writeToFilepath = Path.Combine(_tmpDir, "downloaded-file.using-no-path");
            (bool success, string errorMessage, HttpResponse response) resultVerify = await IpfsFunctionLibrary.DownloadFileAsync(
                TestIpfsConstants.DefaultIpfsHttpGatewayConfig,
                badIpfsAddress,
                writeToFilepath,
                false,
                false);

            Assert.That(resultVerify.success, Is.True, $"resultVerify.success. Error Message: {resultVerify.errorMessage}");
            Assert.That(resultVerify.errorMessage, Is.Empty, "resultVerify.errorMessage");
            Assert.That(resultVerify.response, Is.Not.Null, "resultVerify.response");

            Assert.That(File.Exists(writeToFilepath), Is.True, "writeToFilepath.Exists");

            await VerifyFilesAreNotEqual(originalFile, writeToFilepath);
        }
    }
}