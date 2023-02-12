// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace ThreeStudio.IPFS.Internal
{
    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class Web3StorageResponse
    {
        public string cid;
    }

    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class NftStorageResponseValue
    {
        public string cid;
    }

    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class NftStorageResponse
    {
        public bool ok;
        public NftStorageResponseValue value;
    }

    public static class IpfsUtils
    {
        public static string TryExtractCidFromResponse(HttpResponse response)
        {
            string json = StringUtils.BytesToString(response.Body);

            Web3StorageResponse web3Response = JsonUtility.FromJson<Web3StorageResponse>(json);
            if(!string.IsNullOrEmpty(web3Response.cid))
            {
                return web3Response.cid;
            }

            NftStorageResponse nftResponse = JsonUtility.FromJson<NftStorageResponse>(json);
            if(!string.IsNullOrEmpty(nftResponse.value.cid))
            {
                return nftResponse.value.cid;
            }

            return "";
        }
    }
}