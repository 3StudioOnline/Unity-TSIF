// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.Threading.Tasks;
using UnityEngine;

namespace ThreeStudio.IPFS
{
    public static partial class IpfsFunctionLibrary
    {
        public delegate void IpfsGetDataAsImageDelegate(bool success, string errorMessage, HttpResponse response, Texture2D texture);

        /// <summary>
        /// Gets data as image from the IPFS network for the specified CID/Path.
        /// </summary>
        /// <param name="ipfsHttpGatewayConfig">The gateway to send the request to.</param>
        /// <param name="ipfsAddress">The IPFS address.</param>
        /// <param name="responseDelegate">
        /// Delegate to handle the response. Will also be called if a request could not be sent
        /// successfully.
        /// </param>
        public static async void GetDataAsImage(
            IpfsHttpGatewayConfig ipfsHttpGatewayConfig,
            IpfsAddress ipfsAddress,
            IpfsGetDataAsImageDelegate responseDelegate)
        {
            (bool success, string errorMessage, HttpResponse response, Texture2D texture) result = await GetDataAsImageAsync(
                ipfsHttpGatewayConfig,
                ipfsAddress);
            responseDelegate?.Invoke(result.success, result.errorMessage, result.response, result.texture);
        }

        /// <summary>
        /// Gets data as image from the IPFS network for the specified CID/Path.
        /// </summary>
        /// <param name="ipfsHttpGatewayConfig">The gateway to send the request to.</param>
        /// <param name="ipfsAddress">The IPFS address.</param>
        /// <returns>Whether the request was successful, an optional error message, a HTTP response, and data as texture.</returns>
        public static async Task<(bool success, string errorMessage, HttpResponse response, Texture2D texture)> GetDataAsImageAsync(
            IpfsHttpGatewayConfig ipfsHttpGatewayConfig,
            IpfsAddress ipfsAddress)
        {
            var result = await GetDataAsync(
                ipfsHttpGatewayConfig,
                ipfsAddress);

            Texture2D texture = null;
            if(result.success)
            {
                texture = new Texture2D(0, 0);
                if(!texture.LoadImage(result.data))
                {
                    texture = null;
                }
            }

            return (texture != null, result.errorMessage, result.response, texture);
        }
    }
}