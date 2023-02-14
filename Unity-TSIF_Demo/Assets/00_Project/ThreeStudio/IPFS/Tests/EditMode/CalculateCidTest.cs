// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.IO;
using System.Threading.Tasks;
using _00_Project.ThreeStudio.IPFS.Tests.Shared;
using NUnit.Framework;

namespace ThreeStudio.IPFS.Tests
{
    [TestFixture]
    public class CalculateCidTest
    {
        [Test]
        public async Task CalculateCidFromDataForWeb3()
        {
            byte[] data = await File.ReadAllBytesAsync(TestUtils.GetTestResourceFilepath("3S Studio.png"));

            bool wait = true;
            IpfsFunctionLibrary.CalculateCidFromDataForWeb3(
                data,
                delegate(bool success, string errorMessage, string cid)
                {
                    Assert.That(success, Is.True);
                    Assert.That(errorMessage, Is.Empty);
                    Assert.That(cid, Is.EqualTo("bafkreihutddv4nrs3fj246puy72mexgbnf5bvmqug3o2sjgjubia7ka64i"));

                    wait = false;
                });

            while(wait)
            {
                await Task.Yield();
            }
        }

        [Test]
        public async Task CalculateCidFromDataForWeb3Async()
        {
            byte[] data = await File.ReadAllBytesAsync(TestUtils.GetTestResourceFilepath("3S Studio.png"));
            (bool success, string errorMessage, string cid) result = await IpfsFunctionLibrary.CalculateCidFromDataForWeb3Async(data);

            Assert.That(result.success, Is.True);
            Assert.That(result.errorMessage, Is.Empty);
            Assert.That(result.cid, Is.EqualTo("bafkreihutddv4nrs3fj246puy72mexgbnf5bvmqug3o2sjgjubia7ka64i"));
        }

        [Test]
        public async Task CalculateCidFromFileForWeb3()
        {
            string filepath = TestUtils.GetTestResourceFilepath("3S Hello.txt");

            bool wait = true;
            IpfsFunctionLibrary.CalculateCidFromFileForWeb3(
                filepath,
                delegate(bool success, string errorMessage, string cid)
                {
                    Assert.That(success, Is.True);
                    Assert.That(errorMessage, Is.Empty);
                    Assert.That(cid, Is.EqualTo("bafkreih77yzma2itvkuw3xnfaarqhi4wcihkd5fg2ihq2kmiwignuaafsm"));

                    wait = false;
                });

            while(wait)
            {
                await Task.Yield();
            }
        }

        [Test]
        public async Task CalculateCidFromFileForWeb3Async()
        {
            string filepath = TestUtils.GetTestResourceFilepath("3S Hello.txt");
            (bool success, string errorMessage, string cid) result = await IpfsFunctionLibrary.CalculateCidFromFileForWeb3Async(filepath);

            Assert.That(result.success, Is.True);
            Assert.That(result.errorMessage, Is.Empty);
            Assert.That(result.cid, Is.EqualTo("bafkreih77yzma2itvkuw3xnfaarqhi4wcihkd5fg2ihq2kmiwignuaafsm"));
        }
    }
}