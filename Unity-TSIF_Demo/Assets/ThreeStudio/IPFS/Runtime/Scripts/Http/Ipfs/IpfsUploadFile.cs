// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.IO;
using System.Threading.Tasks;
using ThreeStudio.IPFS.Internal;
using UnityEngine;

namespace ThreeStudio.IPFS
{
    public static partial class IpfsFunctionLibrary
    {
        public delegate void IpfsUploadFileDelegate(bool success, string errorMessage, HttpResponse response, string cid);

        /// <summary>
        /// Uploads a local file to the IPFS network.
        /// </summary>
        /// <param name="ipfsPinningServiceConfig">The pinning service to send the request to.</param>
        /// <param name="bearerToken">The API token for authentication.</param>
        /// <param name="fileToUpload">The file to upload.</param>
        /// <param name="saveAs">
        /// Optional alternate filename for the uploaded file on IPFS.
        /// If left empty, the original filename will be used.
        /// </param>
        /// <param name="responseDelegate">
        /// Delegate to handle the response. Will also be called if a request could not be sent
        /// successfully.
        /// </param>
        public static async void UploadFile(
            IpfsPinningServiceConfig ipfsPinningServiceConfig,
            string bearerToken,
            string fileToUpload,
            string saveAs,
            IpfsUploadFileDelegate responseDelegate)
        {
            (bool success, string errorMessage, HttpResponse response, string cid) result = await UploadFile(
                ipfsPinningServiceConfig,
                bearerToken,
                fileToUpload,
                saveAs);
            responseDelegate?.Invoke(
                result.success,
                result.errorMessage,
                result.response,
                result.cid);
        }

        /// <summary>
        /// Uploads a local file to the IPFS network.
        /// </summary>
        /// <param name="ipfsPinningServiceConfig">The pinning service to send the request to.</param>
        /// <param name="bearerToken">The API token for authentication.</param>
        /// <param name="fileToUpload">The file to upload.</param>
        /// <param name="saveAs">
        /// Optional alternate filename for the uploaded file on IPFS.
        /// If left empty, the original filename will be used.
        /// </param>
        /// <returns>Whether the request was successful, an optional error message, a HTTP response, and a CID.</returns>
        public static async Task<(bool success, string errorMessage, HttpResponse response, string cid)> UploadFile(
            IpfsPinningServiceConfig ipfsPinningServiceConfig,
            string bearerToken,
            string fileToUpload,
            string saveAs)
        {
            if(!File.Exists(fileToUpload))
            {
                string error = $"Could not find file: {fileToUpload}";
                Debug.LogError(error);
                return (false, error, null, string.Empty);
            }

            if(string.IsNullOrEmpty(saveAs))
            {
                saveAs = Path.GetFileName(fileToUpload);
            }

            saveAs = saveAs?.Trim();

            string id = HttpUtils.GenerateHttpCorrelationID();

            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.UploadFileOrData))
            {
                Debug.Log($"[{id}] IPFS Upload File: FileToUpload<{fileToUpload}> SaveAs<{saveAs}>");
            }
            #endif

            byte[] data = await File.ReadAllBytesAsync(fileToUpload);

            (bool success, string errorMessage, HttpResponse response, string cid) result = await UploadData(
                ipfsPinningServiceConfig,
                bearerToken,
                data,
                saveAs);

            return (result.success, result.errorMessage, result.response, result.cid);
        }
    }
}