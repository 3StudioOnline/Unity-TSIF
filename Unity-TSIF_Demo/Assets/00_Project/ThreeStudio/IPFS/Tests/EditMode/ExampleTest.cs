using System.Threading.Tasks;
using NUnit.Framework;

namespace ThreeStudio.IPFS.Tests
{
    [TestFixture]
    public class ExampleTest
    {
        [Test]
        public void ExampleSyncTest()
        {
            Assert.That(true, Is.True);
        }

        [Test]
        public async Task ExampleAsyncTest()
        {
            await Task.Delay(250);
            Assert.That(true, Is.True);
        }
    }
}