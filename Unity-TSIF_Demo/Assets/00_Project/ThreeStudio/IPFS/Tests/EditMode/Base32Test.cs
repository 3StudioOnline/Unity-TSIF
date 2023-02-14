// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using NUnit.Framework;

namespace ThreeStudio.IPFS.Tests
{
    [TestFixture]
    public class Base32Test
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
            new Base32TestData("", ""),
            new Base32TestData(" ", "EA======"),
            new Base32TestData("b", "MI======"),
            new Base32TestData("bb", "MJRA===="),
            new Base32TestData("Cat", "INQXI==="),
            new Base32TestData("test", "ORSXG5A="),
            new Base32TestData("tesla", "ORSXG3DB"),
            new Base32TestData("testtttt", "ORSXG5DUOR2HI==="),
            new Base32TestData("1234", "GEZDGNA="),
            new Base32TestData("12345", "GEZDGNBV"),
            new Base32TestData("12341", "GEZDGNBR"),
            new Base32TestData("test1", "ORSXG5BR"),
            new Base32TestData("1", "GE======"),
            new Base32TestData("12345", "GEZDGNBV"),
            new Base32TestData("abcde", "MFRGGZDF"),
            new Base32TestData("abcdef", "MFRGGZDFMY======"),
            new Base32TestData("abcdefghij", "MFRGGZDFMZTWQ2LK"),
            new Base32TestData("abcdefghi", "MFRGGZDFMZTWQ2I="),
            new Base32TestData("abcdefghijlmnd", "MFRGGZDFMZTWQ2LKNRWW4ZA="),
            new Base32TestData("ljaklsbhfjabhnfojk", "NRVGC23MONRGQZTKMFRGQ3TGN5VGW==="),
            new Base32TestData("ljaklsbhfjabhnfojkljaklsbhfjabhnfojk", "NRVGC23MONRGQZTKMFRGQ3TGN5VGW3DKMFVWY43CNBTGUYLCNBXGM33KNM======"),
            new Base32TestData(
                "ljaklsbhfjabhnfojkljaklsbhfjabhnfojkljaklsbhfjabhnfojkljaklsbhfjabhnfojkasdadada",
                "NRVGC23MONRGQZTKMFRGQ3TGN5VGW3DKMFVWY43CNBTGUYLCNBXGM33KNNWGUYLLNRZWE2DGNJQWE2DOMZXWU23MNJQWW3DTMJUGM2TBMJUG4ZTPNJVWC43EMFSGCZDB"),
        };

        [Test]
        public void Base32Encode()
        {
            foreach(Base32TestData test in base32Tests)
            {
                string actual = EncodingFunctionLibrary.EncodeBase32(test.Input);
                string expected = test.Output;
                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public void Base32Decode()
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