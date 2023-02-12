// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.Collections.Generic;

namespace ThreeStudio.IPFS
{
    public static class Ipfs
    {
        #if DEVELOPMENT_BUILD || UNITY_EDITOR
        private static readonly Dictionary<DebugMode, bool> DebugEnabledMap = new Dictionary<DebugMode, bool>();
        #endif

        private static Dictionary<IpfsHttpGateway, IpfsHttpGatewayConfig> IpfsHttpGatewayConfigs { get; } =
            new Dictionary<IpfsHttpGateway, IpfsHttpGatewayConfig>
            {
                { IpfsHttpGateway.Localhost, new IpfsHttpGatewayConfig("http://127.0.0.1:8080") },
                { IpfsHttpGateway.IpfsIo, new IpfsHttpGatewayConfig("https://ipfs.io") },
                { IpfsHttpGateway.GatewayIpfsIo, new IpfsHttpGatewayConfig("https://gateway.ipfs.io") },
                { IpfsHttpGateway.CloudflareIpfsCom, new IpfsHttpGatewayConfig("https://cloudflare-ipfs.com") },
                { IpfsHttpGateway.NftstorageLink, new IpfsHttpGatewayConfig("https://nftstorage.link") },
                { IpfsHttpGateway.DwebLink, new IpfsHttpGatewayConfig("https://dweb.link") },
                { IpfsHttpGateway.GatewayPinataCloud, new IpfsHttpGatewayConfig("https://gateway.pinata.cloud") },
                { IpfsHttpGateway.IpfsInfuraIo, new IpfsHttpGatewayConfig("https://ipfs.infura.io") },
                { IpfsHttpGateway.InfuraIpfsIo, new IpfsHttpGatewayConfig("https://infura-ipfs.io") },
                { IpfsHttpGateway.IpfsGatewayCloud, new IpfsHttpGatewayConfig("https://ipfs-gateway.cloud") },
            };

        private static Dictionary<IpfsPinningService, IpfsPinningServiceConfig> IpfsPinningServiceConfigs { get; } =
            new Dictionary<IpfsPinningService, IpfsPinningServiceConfig>
            {
                { IpfsPinningService.Web3Storage, new IpfsPinningServiceConfig("Web3.storage", "https://api.web3.storage") },
                { IpfsPinningService.NftStorage, new IpfsPinningServiceConfig("NFT.storage", "https://api.nft.storage") },
            };

        public enum DebugMode
        {
            DownloadFileOrGetData,
            UploadFileOrData,
        }

        public static bool IsDebugEnabled(DebugMode debugMode)
        {
            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            return DebugEnabledMap.GetValueOrDefault(debugMode, false);
            #else
            return false;
            #endif
        }

        public static void SetDebugLogEnabled(bool enabled, DebugMode debugMode, params DebugMode[] moreDebugModes)
        {
            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            DebugEnabledMap.TryAdd(debugMode, enabled);
            foreach(DebugMode mode in moreDebugModes)
            {
                DebugEnabledMap.TryAdd(mode, enabled);
            }
            #endif
        }

        /// <summary>
        /// Gets a IPFS HTTP Gateway.
        /// </summary>
        /// <param name="ipfsHttpGateway">The IPFS HTTP Gateway.</param>
        /// <returns>The IPFS HTTP Gateway.</returns>
        public static IpfsHttpGatewayConfig GetIpfsHttpGatewayConfig(IpfsHttpGateway ipfsHttpGateway)
        {
            return IpfsHttpGatewayConfigs[ipfsHttpGateway];
        }

        /// <summary>
        /// Gets a IPFS Pinning Service.
        /// </summary>
        /// <param name="ipfsPinningService">The IPFS Pinning Service.</param>
        /// <returns>The IPFS Pinning Service.</returns>
        public static IpfsPinningServiceConfig GetIpfsPinningServiceConfig(IpfsPinningService ipfsPinningService)
        {
            return IpfsPinningServiceConfigs[ipfsPinningService];
        }
    }
}