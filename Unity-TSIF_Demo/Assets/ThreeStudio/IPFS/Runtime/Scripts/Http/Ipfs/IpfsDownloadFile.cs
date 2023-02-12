// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using ThreeStudio.IPFS.Http;
using ThreeStudio.IPFS.Internal;
using UnityEngine;

namespace ThreeStudio.IPFS
{
    public static partial class IpfsFunctionLibrary
    {
        public delegate void IpfsDownloadFileDelegate(bool success, string errorMessage, HttpResponse response);

        /// <summary>
        /// Downloads a file from the IPFS network for the specified CID/Path.
        /// </summary>
        /// <param name="ipfsHttpGatewayConfig">The gateway to send the request to.</param>
        /// <param name="ipfsAddress">The IPFS address.</param>
        /// <param name="writeToFilepath">The filepath where the downloaded data is written to.</param>
        /// <param name="createPathIfMissing">
        /// Creates the filepath to where the downloaded data should be written to
        /// if it is missing.
        /// </param>
        /// <param name="overwriteExistingFile">
        /// If set to false and the file exists this function will abort with
        /// failure. Otherwise, an existing file will be overwritten.
        /// </param>
        /// <param name="responseDelegate">
        /// Delegate to handle the response. Will also be called if a request could not be sent
        /// successfully.
        /// </param>
        public static async void DownloadFile(
            IpfsHttpGatewayConfig ipfsHttpGatewayConfig,
            IpfsAddress ipfsAddress,
            string writeToFilepath,
            bool createPathIfMissing,
            bool overwriteExistingFile,
            IpfsDownloadFileDelegate responseDelegate)
        {
            var result = await DownloadFileAsync(
                ipfsHttpGatewayConfig,
                ipfsAddress,
                writeToFilepath,
                createPathIfMissing,
                overwriteExistingFile);
            responseDelegate?.Invoke(result.success, result.errorMessage, result.response);
        }

        /// <summary>
        /// Downloads a file from the IPFS network for the specified CID/Path.
        /// </summary>
        /// <param name="ipfsHttpGatewayConfig">The gateway to send the request to.</param>
        /// <param name="ipfsAddress">The IPFS address.</param>
        /// <param name="writeToFilepath">The filepath where the downloaded data is written to.</param>
        /// <param name="createPathIfMissing">
        /// Creates the filepath to where the downloaded data should be written to
        /// if it is missing.
        /// </param>
        /// <param name="overwriteExistingFile">
        /// If set to false and the file exists this function will abort with
        /// failure. Otherwise, an existing file will be overwritten.
        /// </param>
        /// <returns>Whether the request was successful, an optional error message, and a HTTP response.</returns>
        public static async Task<(bool success, string errorMessage, HttpResponse response)> DownloadFileAsync(
            IpfsHttpGatewayConfig ipfsHttpGatewayConfig,
            IpfsAddress ipfsAddress,
            string writeToFilepath,
            bool createPathIfMissing,
            bool overwriteExistingFile)
        {
            string id = HttpUtils.GenerateHttpCorrelationID();

            if(createPathIfMissing)
            {
                string path = Path.GetDirectoryName(writeToFilepath);
                try
                {
                    if(path != null && !Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                catch(Exception e)
                {
                    return (false, e.Message, null);
                }
            }

            if(!overwriteExistingFile)
            {
                if(File.Exists(writeToFilepath))
                {
                    return (false, $"Cannot overwrite already existing file: {writeToFilepath}", null);
                }
            }

            string cidOrPath = ipfsAddress.Cid;
            if(!string.IsNullOrEmpty(ipfsAddress.Path))
            {
                string path = HttpUtility.UrlPathEncode(ipfsAddress.Path);
                cidOrPath += $"/{path}";
            }

            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.DownloadFileOrGetData))
            {
                Debug.Log($"[{id}] IPFS Download File: CID<{cidOrPath}> WriteToFilepath<{writeToFilepath}>");
            }
            #endif

            var result = await HttpRequest.SendGetRequestAsync(
                $"{ipfsHttpGatewayConfig.Url}/ipfs/{cidOrPath}",
                id);


            if(!result.success)
            {
                return (false, result.errorMessage, result.response);
            }

            try
            {
                await File.WriteAllBytesAsync(writeToFilepath, result.response.Body);
            }
            catch(Exception e)
            {
                #if DEVELOPMENT_BUILD || UNITY_EDITOR
                if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.DownloadFileOrGetData))
                {
                    Debug.Log($"[{id}] Could not write to file: {e.Message}");
                }
                #endif

                return (false, e.Message, null);
            }

            return (true, "", result.response);
        }
    }
}