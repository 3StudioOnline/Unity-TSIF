// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using ThreeStudio.IPFS.Http;
using ThreeStudio.IPFS.Internal;
using UnityEngine;

namespace ThreeStudio.IPFS
{
    public static partial class IpfsFunctionLibrary
    {
        public delegate void IpfsUploadDataDelegate(bool success, string errorMessage, HttpResponse response, string cid);

        /// <summary>
        /// Uploads data to the IPFS network.
        /// </summary>
        /// <param name="ipfsPinningServiceConfig">The pinning service to send the request to.</param>
        /// <param name="bearerToken">The API token for authentication.</param>
        /// <param name="dataToUpload">The data to upload.</param>
        /// <param name="saveAs">Mandatory filename for the uploaded data on IPFS.</param>
        /// <param name="responseDelegate">
        /// Delegate to handle the response. Will also be called if a request could not be sent
        /// successfully.
        /// </param>
        public static async void UploadData(
            IpfsPinningServiceConfig ipfsPinningServiceConfig,
            string bearerToken,
            byte[] dataToUpload,
            string saveAs,
            IpfsUploadDataDelegate responseDelegate)
        {
            (bool success, string errorMessage, HttpResponse response, string cid) result = await UploadDataAsync(
                ipfsPinningServiceConfig,
                bearerToken,
                dataToUpload,
                saveAs);
            responseDelegate?.Invoke(
                result.success,
                result.errorMessage,
                result.response,
                result.cid);
        }

        /// <summary>
        /// Uploads data to the IPFS network.
        /// </summary>
        /// <param name="ipfsPinningServiceConfig">The pinning service to send the request to.</param>
        /// <param name="bearerToken">The API token for authentication.</param>
        /// <param name="dataToUpload">The data to upload.</param>
        /// <param name="saveAs">Optional filename or filepath for the uploaded data on IPFS.</param>
        /// <returns>Whether the request was successful, an optional error message, a HTTP response, and a CID.</returns>
        public static async Task<(bool success, string errorMessage, HttpResponse response, string cid)> UploadDataAsync(
            IpfsPinningServiceConfig ipfsPinningServiceConfig,
            string bearerToken,
            byte[] dataToUpload,
            string saveAs)
        {
            string id = HttpUtils.GenerateHttpCorrelationID();

            saveAs = saveAs?.Trim();

            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.UploadFileOrData))
            {
                Debug.Log($"[{id}] IPFS Upload Data: SaveAs<{saveAs}>");
            }
            #endif

            string boundary = HttpUtils.MultipartFormData_GenerateBoundary();

            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.UploadFileOrData))
            {
                Debug.Log($"[{id}] Debug Multipart Form Data: ***START***");
            }
            #endif

            var body = new List<byte>();
            body.AddRange(HttpUtils.MultipartFormData_AddFile(boundary, saveAs, dataToUpload));
            body.AddRange(HttpUtils.MultipartFormData_End(boundary));

            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.UploadFileOrData))
            {
                Debug.Log($"[{id}] Debug Multipart Form Data: ***END***");
            }
            #endif

            var headers = HttpUtils.MultipartFormData_BuildHeaders(boundary, bearerToken);

            (bool success, string errorMessage, HttpResponse response) result = await HttpRequest.SendPostRequestAsync(
                ipfsPinningServiceConfig.Url + "/upload",
                headers,
                body.ToArray(),
                id);

            if(!result.success)
            {
                string errorMessageForDelegate = $"Data upload failed: Error<{result.errorMessage}>";

                #if DEVELOPMENT_BUILD || UNITY_EDITOR
                if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.UploadFileOrData))
                {
                    Debug.Log($"[{id}] {errorMessageForDelegate}");
                }
                #endif

                return (false, errorMessageForDelegate, result.response, "");
            }

            string cid = IpfsUtils.TryExtractCidFromResponse(result.response);
            return (true, "", result.response, cid);
        }
    }
}