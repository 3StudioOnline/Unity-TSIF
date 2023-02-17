// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using NUnit.Framework;
using ThreeStudio.IPFS.Tests.Shared;

namespace ThreeStudio.IPFS.Tests
{
    [TestFixture]
    public class FileUtilsTest
    {
        [Test]
        public void GetStatData_Directory()
        {
            bool isValid = FileFunctionLibrary.GetStatData(
                TestUtils.GetTestResourceFilepath("FileUtilsTest", "FindThisDirectory"),
                out FileStatData fileStatData);
            Assert.That(isValid, Is.True, "Directory exists and stats data is valid");

            Assert.That(fileStatData.CreationTime, Is.Not.Null, "CreationTime");
            Assert.That(fileStatData.AccessTime, Is.Not.Null, "AccessTime");
            Assert.That(fileStatData.ModificationTime, Is.Not.Null, "ModificationTime");
            Assert.That(fileStatData.FileSize, Is.Zero, "FileSize");
            Assert.That(fileStatData.IsDirectory, Is.True, "IsDirectory");
            Assert.That(fileStatData.IsReadOnly, Is.True, "IsReadOnly");
            Assert.That(fileStatData.IsValid, Is.True, "IsValid");
        }

        [Test]
        public void GetStatData_File()
        {
            bool isValid = FileFunctionLibrary.GetStatData(
                TestUtils.GetTestResourceFilepath("FileUtilsTest", "FindThisDirectory", "FindThisFile.txt"),
                out FileStatData fileStatData);
            Assert.That(isValid, Is.True, "File exists and stats data is valid");

            Assert.That(fileStatData.CreationTime, Is.Not.Null, "CreationTime");
            Assert.That(fileStatData.AccessTime, Is.Not.Null, "AccessTime");
            Assert.That(fileStatData.ModificationTime, Is.Not.Null, "ModificationTime");
            Assert.That(fileStatData.FileSize, Is.EqualTo(76), "FileSize");
            Assert.That(fileStatData.IsDirectory, Is.False, "IsDirectory");
            Assert.That(fileStatData.IsReadOnly, Is.False, "IsReadOnly");
            Assert.That(fileStatData.IsValid, Is.True, "IsValid");
        }
    }
}