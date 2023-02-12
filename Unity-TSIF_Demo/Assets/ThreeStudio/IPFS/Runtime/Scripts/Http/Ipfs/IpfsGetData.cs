// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.Threading.Tasks;
using System.Web;
using ThreeStudio.IPFS.Http;
using ThreeStudio.IPFS.Internal;
using UnityEngine;

namespace ThreeStudio.IPFS
{
    public static partial class IpfsFunctionLibrary
    {
        public delegate void IpfsGetDataDelegate(bool success, string errorMessage, HttpResponse response, byte[] data);

        /// <summary>
        /// Gets data from the IPFS network for the specified CID/Path.
        /// </summary>
        /// <param name="ipfsHttpGatewayConfig">The gateway to send the request to.</param>
        /// <param name="ipfsAddress">The IPFS address.</param>
        /// <param name="responseDelegate">
        /// Delegate to handle the response. Will also be called if a request could not be sent
        /// successfully.
        /// </param>
        public static async void GetData(
            IpfsHttpGatewayConfig ipfsHttpGatewayConfig,
            IpfsAddress ipfsAddress,
            IpfsGetDataDelegate responseDelegate)
        {
            (bool success, string errorMessage, HttpResponse response, byte[] data) result = await GetDataAsync(ipfsHttpGatewayConfig, ipfsAddress);
            responseDelegate?.Invoke(result.success, result.errorMessage, result.response, result.response?.Body);
        }

        /// <summary>
        /// Gets data from the IPFS network for the specified CID/Path.
        /// </summary>
        /// <param name="ipfsHttpGatewayConfig">The gateway to send the request to.</param>
        /// <param name="ipfsAddress">The IPFS address.</param>
        /// <returns>Whether the request was successful, an optional error message, a HTTP response, and the data.</returns>
        public static async Task<(bool success, string errorMessage, HttpResponse response, byte[] data)> GetDataAsync(
            IpfsHttpGatewayConfig ipfsHttpGatewayConfig,
            IpfsAddress ipfsAddress)
        {
            string cidOrPath = ipfsAddress.Cid;
            if(!string.IsNullOrEmpty(ipfsAddress.Path))
            {
                string path = HttpUtility.UrlPathEncode(ipfsAddress.Path);
                cidOrPath += $"/{path}";
            }

            string id = HttpUtils.GenerateHttpCorrelationID();

            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.DownloadFileOrGetData))
            {
                Debug.Log($"[{id}] IPFS Get Data: CID<{cidOrPath}>");
            }
            #endif

            (bool success, string errorMessage, HttpResponse response) result = await HttpRequest.SendGetRequestAsync(
                $"{ipfsHttpGatewayConfig.Url}/ipfs/{cidOrPath}",
                id);

            return result.success
                ? (true, "", result.response, result.response.Body)
                : (false, result.errorMessage, result.response, null);
        }
    }
}