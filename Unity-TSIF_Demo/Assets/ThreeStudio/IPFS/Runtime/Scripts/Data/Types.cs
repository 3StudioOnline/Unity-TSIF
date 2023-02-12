// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System;
using System.Collections.Generic;

namespace ThreeStudio.IPFS
{
    /// <summary>
    /// IPFS HTTP Gateways.
    /// </summary>
    public enum IpfsHttpGateway
    {
        Localhost,
        IpfsIo,
        GatewayIpfsIo,
        CloudflareIpfsCom,
        NftstorageLink,
        DwebLink,
        GatewayPinataCloud,
        IpfsInfuraIo,
        InfuraIpfsIo,
        IpfsGatewayCloud,
    };

    /// <summary>
    /// IPFS Pinning Service.
    /// </summary>
    public enum IpfsPinningService
    {
        Web3Storage,
        NftStorage,
    };

    /// <summary>
    /// Holds the response data for a HTTP request.
    /// </summary>
    public class HttpResponse
    {
        /// <summary>
        /// True if a response was received from a server and the status code is 2xx -> "Success".
        /// NOTE: If this is true it does only mean that there was no error on the data transport layer (HTTP).
        /// </summary>
        public bool Success;
        
        /// <summary>
        /// Response status code (HTTP status code, e.g. 200 -> "OK").
        /// </summary>
        public long StatusCode = -1;
        
        /// <summary>
        /// Response headers in the format of "Key: Value".
        /// </summary>
        public Dictionary<string, string> Headers;
        
        /// <summary>
        /// Response body.
        /// </summary>
        public byte[] Body;
    };

    /// <summary>
    /// Defines properties about an IPFS HTTP Gateway.
    /// </summary>
    public class IpfsHttpGatewayConfig
    {
        /// <summary>
        /// The Gateway URL.
        /// </summary>
        public readonly string Url;

        public IpfsHttpGatewayConfig(string url)
        {
            Url = url;
        }
    };

    /// <summary>
    /// Defines properties about an IPFS Pinning Service API endpoint.
    /// </summary>
    public class IpfsPinningServiceConfig
    {
        /// <summary>
        /// The Pinning Service API endpoint URL.
        /// </summary>
        public readonly string Url;

        /// <summary>
        /// The name of the Pinning Service API endpoint provider.
        /// </summary>
        public readonly string Name;

        public IpfsPinningServiceConfig(string name, string url)
        {
            Name = name;
            Url = url;
        }
    };

    /// <summary>
    /// Holds info for addressing IPFS content using either a CID or CID with path.
    /// </summary>
    public class IpfsAddress
    {
        /// <summary>
        /// The content identifier.
        /// </summary>
        public readonly string Cid;
        
        /// <summary>
        /// The optional path related to root CID.
        /// </summary>
        public readonly string Path;

        public IpfsAddress(string cid, string path = null)
        {
            Cid = cid;
            Path = path;
        }
    };
    
    /// <summary>
    /// Holds info about a file or directory, e.g. its creation time, file size, etc.
    /// </summary>
    public struct FileStatData
    {
        /// <summary>
        /// The time that the file or directory was originally created, or FDateTime::MinValue if the creation time is unknown.
        /// </summary>
        public DateTime CreationTime;

        /// <summary>
        /// The time that the file or directory was last accessed, or FDateTime::MinValue if the access time is unknown.
        /// </summary>
        public DateTime AccessTime;

        /// <summary>
        /// The time the the file or directory was last modified, or FDateTime::MinValue if the modification time is unknown.
        /// </summary>
        public DateTime ModificationTime;

        /// <summary>
        /// Size of the file (in bytes), or -1 if the file size is unknown.
        /// </summary>
        public long FileSize;

        /// <summary>
        /// True if this data is for a directory, false if it's for a file.
        /// </summary>
        public bool IsDirectory;

        /// <summary>
        /// True if this file is read-only.
        /// </summary>
        public bool IsReadOnly;

        /// <summary>
        /// True if file or directory was found, false otherwise.
        /// Note that this value being true does not ensure that the other members are filled in with meaningful data,
        /// as not all file systems have access to all of this data.
        /// </summary>
        public bool IsValid;
    };
}