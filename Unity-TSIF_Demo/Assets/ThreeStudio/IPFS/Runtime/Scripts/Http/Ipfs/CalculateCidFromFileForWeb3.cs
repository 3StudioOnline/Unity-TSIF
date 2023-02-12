// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System;
using System.IO;
using System.Threading.Tasks;

namespace ThreeStudio.IPFS
{
    public static partial class IpfsFunctionLibrary
    {
        public delegate void CalculateCidFromFileForWeb3Delegate(bool success, string errorMessage, string cid);

        /// <summary>
        /// Tries to calculate CID V1 from file.
        /// </summary>
        /// <param name="readFromFilepath"></param>
        /// <param name="responseDelegate">
        /// Delegate to handle the response. Will also be called if an error occured.
        /// </param>
        public static async void CalculateCidFromFileForWeb3(string readFromFilepath, CalculateCidFromFileForWeb3Delegate responseDelegate)
        {
            (bool success, string errorMessage, string cid) result = await CalculateCidFromFileForWeb3Async(readFromFilepath);
            responseDelegate?.Invoke(result.success, result.errorMessage, result.cid);
        }

        /// <summary>
        /// Tries to calculate CID V1 from file.
        /// </summary>
        /// <param name="readFromFilepath"></param>
        /// <returns>True, if calculation was successful.</returns>
        public static async Task<(bool success, string errorMessage, string cid)> CalculateCidFromFileForWeb3Async(string readFromFilepath)
        {
            byte[] bytes;
            try
            {
                bytes = await File.ReadAllBytesAsync(readFromFilepath);
            }
            catch(Exception e)
            {
                return (false, e.Message, null);
            }

            (bool success, string errorMessage, string cid) result = await CalculateCidFromDataForWeb3Async(bytes);
            return (result.success, result.errorMessage, result.cid);
        }
    }
}