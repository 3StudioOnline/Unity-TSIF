// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.IO;

namespace ThreeStudio.IPFS
{
    /// <summary>
    /// File Function Library.
    /// </summary>
    public static class FileFunctionLibrary
    {
        /// <summary>
        /// Gets stat data for the given file or directory.
        /// </summary>
        /// <param name="filepath">The path to a file or directory.</param>
        /// <param name="outFileStatData">the file stat data.</param>
        /// <returns>True, if returned data is valid.</returns>
        public static bool GetStatData(string filepath, out FileStatData outFileStatData)
        {
            if(File.Exists(filepath))
            {
                FileInfo fileInfo = new(filepath);
                outFileStatData.AccessTime = fileInfo.LastAccessTime;
                outFileStatData.CreationTime = fileInfo.CreationTime;
                outFileStatData.ModificationTime = fileInfo.LastWriteTime;
                outFileStatData.FileSize = fileInfo.Length;
                outFileStatData.IsDirectory = false;
                outFileStatData.IsReadOnly = fileInfo.IsReadOnly;
                outFileStatData.IsValid = fileInfo.Exists;
            }
            else if(Directory.Exists(filepath))
            {
                DirectoryInfo dirInfo = new(filepath);
                outFileStatData.AccessTime = dirInfo.LastAccessTime;
                outFileStatData.CreationTime = dirInfo.CreationTime;
                outFileStatData.ModificationTime = dirInfo.LastWriteTime;
                outFileStatData.FileSize = 0;
                outFileStatData.IsDirectory = true;
                outFileStatData.IsReadOnly = true;
                outFileStatData.IsValid = dirInfo.Exists;
            }
            else
            {
                outFileStatData = default;
            }

            return outFileStatData.IsValid;
        }
    }
}