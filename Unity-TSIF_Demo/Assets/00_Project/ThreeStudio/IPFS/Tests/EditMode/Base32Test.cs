// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using NUnit.Framework;

namespace ThreeStudio.IPFS.Tests
{
    [TestFixture]
    public class EncodingTest
    {
        private struct Base32TestData
        {
            public readonly string Input;
            public readonly string Output;

            public Base32TestData(string input, string output)
            {
                Input = input;
                Output = output;
            }
        };

        private Base32TestData[] base32Tests =
        {
            new("", ""),
            new(" ", "EA======"),
            new("b", "MI======"),
            new("bb", "MJRA===="),
            new("Cat", "INQXI==="),
            new("test", "ORSXG5A="),
            new("tesla", "ORSXG3DB"),
            new("testtttt", "ORSXG5DUOR2HI==="),
            new("1234", "GEZDGNA="),
            new("12345", "GEZDGNBV"),
            new("12341", "GEZDGNBR"),
            new("test1", "ORSXG5BR"),
            new("1", "GE======"),
            new("12345", "GEZDGNBV"),
            new("abcde", "MFRGGZDF"),
            new("abcdef", "MFRGGZDFMY======"),
            new("abcdefghij", "MFRGGZDFMZTWQ2LK"),
            new("abcdefghi", "MFRGGZDFMZTWQ2I="),
            new("abcdefghijlmnd", "MFRGGZDFMZTWQ2LKNRWW4ZA="),
            new("ljaklsbhfjabhnfojk", "NRVGC23MONRGQZTKMFRGQ3TGN5VGW==="),
            new("ljaklsbhfjabhnfojkljaklsbhfjabhnfojk", "NRVGC23MONRGQZTKMFRGQ3TGN5VGW3DKMFVWY43CNBTGUYLCNBXGM33KNM======"),
            new(
                "ljaklsbhfjabhnfojkljaklsbhfjabhnfojkljaklsbhfjabhnfojkljaklsbhfjabhnfojkasdadada",
                "NRVGC23MONRGQZTKMFRGQ3TGN5VGW3DKMFVWY43CNBTGUYLCNBXGM33KNNWGUYLLNRZWE2DGNJQWE2DOMZXWU23MNJQWW3DTMJUGM2TBMJUG4ZTPNJVWC43EMFSGCZDB"),
        };

        [Test]
        public void Encode()
        {
            foreach(Base32TestData test in base32Tests)
            {
                string actual = EncodingFunctionLibrary.EncodeBase32(test.Input);
                string expected = test.Output;
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public void Decode()
        {
            foreach(Base32TestData test in base32Tests)
            {
                bool success = EncodingFunctionLibrary.DecodeBase32(test.Output, out string actual);
                Assert.IsTrue(success);

                string expected = test.Input;
                Assert.That(actual, Is.EqualTo(expected));
            }
        }
    }
}