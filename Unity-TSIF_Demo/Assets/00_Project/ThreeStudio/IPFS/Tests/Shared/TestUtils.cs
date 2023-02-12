using System;
using System.IO;
using UnityEngine;

namespace _00_Project.ThreeStudio.IPFS.Tests.Shared
{
    public static class TestUtils
    {
        public static string GetTestResourceFilepath(params string[] filepaths)
        {
            string finalFilepath = Path.Combine(Application.dataPath, "..", "Tests", "Resources");
            foreach(string filepath in filepaths)
            {
                finalFilepath = Path.Combine(finalFilepath, filepath);
            }

            return finalFilepath;
        }

        public static string CreateTempFilepath()
        {
            string baseDirectory = Path.Combine(Path.GetTempPath(), "ThreeStudio", "IPFS", "Tests", Guid.NewGuid().ToString());
            if(!Directory.Exists(baseDirectory))
            {
                DirectoryInfo directoryInfo = Directory.CreateDirectory(baseDirectory);
                if(!directoryInfo.Exists)
                {
                    throw new Exception("Could not create temp directory");
                }
            }

            return baseDirectory;
        }

        public static void DeleteTempFilepath(string tmp)
        {
            tmp = tmp.Trim();
            if(string.IsNullOrEmpty(tmp))
            {
                return;
            }

            string baseDirectory = Path.Combine(Path.GetTempPath(), "ThreeStudio", "IPFS", "Tests");
            if(!tmp.StartsWith(baseDirectory))
            {
                Debug.LogError($"Prevented deletion of an invalid temp directory. Base directory looks odd: {tmp}");
                return;
            }

            if(tmp.Contains(".."))
            {
                Debug.LogError($"Prevented deletion of an invalid temp directory. Path must not contain '..': {tmp}");
                return;
            }

            Directory.Delete(tmp, true);
        }
    }
}