// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.Threading.Tasks;
using ThreeStudio.IPFS.Internal;

namespace ThreeStudio.IPFS
{
    public static partial class IpfsFunctionLibrary
    {
        public delegate void IpfsGetDataAsStringDelegate(bool success, string errorMessage, HttpResponse response, string dataString);

        /// <summary>
        /// Gets data as image from the IPFS network for the specified CID/Path.
        /// </summary>
        /// <param name="ipfsHttpGatewayConfig">The gateway to send the request to.</param>
        /// <param name="ipfsAddress">The IPFS address.</param>
        /// <param name="responseDelegate">
        /// Delegate to handle the response. Will also be called if a request could not be sent
        /// successfully.
        /// </param>
        public static async void GetDataAsString(
            IpfsHttpGatewayConfig ipfsHttpGatewayConfig,
            IpfsAddress ipfsAddress,
            IpfsGetDataAsStringDelegate responseDelegate)
        {
            (bool success, string errorMessage, HttpResponse response, byte[] data) result = await GetDataAsync(ipfsHttpGatewayConfig, ipfsAddress);
            responseDelegate?.Invoke(
                result.success,
                result.errorMessage,
                result.response,
                result.success ? StringUtils.BytesToString(result.data) : null);
        }

        /// <summary>
        /// Gets data as image from the IPFS network for the specified CID/Path.
        /// </summary>
        /// <param name="ipfsHttpGatewayConfig">The gateway to send the request to.</param>
        /// <param name="ipfsAddress">The IPFS address.</param>
        /// <returns>Whether the request was successful, an optional error message, a HTTP response, and data as string.</returns>
        public static async Task<(bool success, string errorMessage, HttpResponse response, string dataString)> GetDataAsStringAsync(
            IpfsHttpGatewayConfig ipfsHttpGatewayConfig,
            IpfsAddress ipfsAddress)
        {
            (bool success, string errorMessage, HttpResponse response, byte[] data) result = await GetDataAsync(
                ipfsHttpGatewayConfig,
                ipfsAddress);

            return (
                result.success,
                result.errorMessage,
                result.response,
                result.success ? StringUtils.BytesToString(result.data) : null
            );
        }
    }
}