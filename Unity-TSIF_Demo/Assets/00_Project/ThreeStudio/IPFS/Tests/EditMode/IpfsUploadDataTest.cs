// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using ThreeStudio.IPFS.Tests.Shared;
using NUnit.Framework;
using ThreeStudio.IPFS.Internal;
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
    public class IpfsUploadDataTest
    {
        private static string[] _saveAsValues = new string[] { null, "", "test.txt", "sub-folder/test.txt" };

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
        public async Task IpfsUploadDataAsync([ValueSource(nameof(_saveAsValues))] string saveAs)
        {
            await Task.Delay(500);

            await UploadDataAndDownloadFileWithVerification(saveAs);
        }

        private async Task UploadDataAndDownloadFileWithVerification(string ipfsPath)
        {
            // =================================================================
            // Prepare data for uploading
            // =================================================================

            string dataContent = "Test content " + DateTime.Now + " - " + Random.Range(0, int.MaxValue);
            byte[] tmpData = StringUtils.StringToBytes(dataContent);

            // =================================================================
            // Upload data
            // =================================================================

            (bool success, string errorMessage, HttpResponse response, string cid) resultUpload = await IpfsFunctionLibrary.UploadData(
                TestIpfsConstants.DefaultIpfsPinningServiceConfig,
                TestIpfsConstants.BearerToken_Web3Storage,
                tmpData,
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

            (bool success, string errorMessage, HttpResponse response) resultDownload = await IpfsFunctionLibrary.DownloadFile(
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

            byte[] downloadedData = await File.ReadAllBytesAsync(writeToFilepath);

            VerifyDataIsEqual(tmpData, downloadedData);

            // We want to verify that downloading a file will fail if we don't use a path if that given data was
            // originally uploaded using a path.
            await VerifyDownloadWithoutPathWillFailIfPathSpecified(ipfsAddress, tmpData);
        }

        /// <summary>
        /// This function verifies that two given byte[] are equal in length and content.
        /// </summary>
        /// <param name="dataA">Data A</param>
        /// <param name="dataB">Data B</param>
        private void VerifyDataIsEqual(byte[] dataA, byte[] dataB)
        {
            Assert.That(dataA, Is.Not.Null, "dataA.NotNull");
            Assert.That(dataB, Is.Not.Null, "dataB.NotNull");
            Assert.That(dataA.Length, Is.EqualTo(dataB.Length), "size of dataA and dataB are the same");

            bool sameBytes = true;
            for(int index = 0; index < dataA.Length; index++)
            {
                bool isSameByte = dataA[index] == dataB[index];
                if(!isSameByte)
                {
                    sameBytes = false;
                    break;
                }
            }

            Assert.That(sameBytes, Is.True, "sameBytes");
        }

        /// <summary>
        /// This function verifies that two given byte[] are not equal in length and content.
        /// </summary>
        /// <param name="dataA">Data A</param>
        /// <param name="dataB">Data B</param>
        private void VerifyFilesAreNotEqual(byte[] dataA, byte[] dataB)
        {
            Assert.That(dataA, Is.Not.Null, "dataOfFileA.NotNull");
            Assert.That(dataB, Is.Not.Null, "dataOfFileB.NotNull");

            bool sameLength = dataA.Length == dataB.Length;
            if(!sameLength)
            {
                return;
            }

            // Data of same length can still have different bytes so we need to check this next
            bool sameBytes = true;
            for(int index = 0; index < dataA.Length; index++)
            {
                bool isSameByte = dataA[index] == dataB[index];
                if(!isSameByte)
                {
                    sameBytes = false;
                    break;
                }
            }

            Assert.That(sameBytes, Is.False, "sameBytes");
        }

        private async Task VerifyDownloadWithoutPathWillFailIfPathSpecified(IpfsAddress goodIpfsAddress, byte[] originalData)
        {
            if(string.IsNullOrEmpty(goodIpfsAddress.Path))
            {
                return;
            }

            IpfsAddress badIpfsAddress = new IpfsAddress(goodIpfsAddress.Cid, null);

            string writeToFilepath = Path.Combine(_tmpDir, "downloaded-file.using-no-path");
            (bool success, string errorMessage, HttpResponse response) resultVerify = await IpfsFunctionLibrary.DownloadFile(
                TestIpfsConstants.DefaultIpfsHttpGatewayConfig,
                badIpfsAddress,
                writeToFilepath,
                false,
                false);

            Assert.That(resultVerify.success, Is.True, $"resultVerify.success. Error Message: {resultVerify.errorMessage}");
            Assert.That(resultVerify.errorMessage, Is.Empty, "resultVerify.errorMessage");
            Assert.That(resultVerify.response, Is.Not.Null, "resultVerify.response");

            Assert.That(File.Exists(writeToFilepath), Is.True, "writeToFilepath.Exists");

            byte[] downloadedData = await File.ReadAllBytesAsync(writeToFilepath);

            VerifyFilesAreNotEqual(originalData, downloadedData);
        }
    }
}