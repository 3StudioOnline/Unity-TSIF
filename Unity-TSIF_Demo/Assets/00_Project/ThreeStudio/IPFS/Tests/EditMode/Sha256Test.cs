// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using NUnit.Framework;
using ThreeStudio.IPFS.Internal;

namespace ThreeStudio.IPFS.Tests
{
    [TestFixture]
    public class Sha256Test
    {
        private struct Sha256TestData
        {
            public readonly string InputString;
            public readonly byte[] InputBytes;
            public readonly string Output;

            public Sha256TestData(string inputString, byte[] inputBytes, string output)
            {
                InputString = inputString;
                InputBytes = inputBytes;
                Output = output;
            }
        }

        [Test]
        public void Sha256()
        {
            Sha256TestData[] sha256Tests =
            {
                new(
                    "78a6273103d17c39a0b6126e226cec70e33337f4bc6a38067401b54a33e78ead",
                    null,
                    "ca244c7cf492084796d8287b70c736e8fcf45e25bb6c4e8f96f699505bfac956"),

                new(
                    "iashgfoaghsfoiknaodfgjgafinaslkfhalsjkfhalskfn",
                    null,
                    "3acb11d290778448f73ae77db9ae8bbd906344dd38b7b87a69af2ad55672081e"),

                new(
                    "",
                    new byte[] { 82, 105, 100, 97, 32, 84, 101, 115, 116, 32, 48, 53, 45, 48, 57, 45, 50, 48, 50, 50 },
                    "e1da45629bae9fbaf062a02880db8822a0ce8e7644a372226fcfa7c8402a02e6"),
            };

            foreach(Sha256TestData sha256Test in sha256Tests)
            {
                string actual = sha256Test.InputBytes != null
                    ? StringUtils.BytesToHex(HashUtils.Sha256FromBytes(sha256Test.InputBytes))
                    : StringUtils.BytesToHex(HashUtils.Sha256FromString(sha256Test.InputString));

                string expected = sha256Test.Output;

                Assert.That(actual, Is.EqualTo(expected), $"Sha256 on {sha256Test.InputString} is {sha256Test.Output} but was {actual}");
            }
        }
    }
}