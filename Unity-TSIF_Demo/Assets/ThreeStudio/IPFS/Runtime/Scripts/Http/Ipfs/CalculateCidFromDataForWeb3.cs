// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.Threading.Tasks;
using ThreeStudio.IPFS.Internal;

namespace ThreeStudio.IPFS
{
    public static partial class IpfsFunctionLibrary
    {
        public delegate void CalculateCidFromDataForWeb3Delegate(bool success, string errorMessage, string cid);

        private const string CidVersion = "01";
        private const string CodecVersionRawBinary = "55";
        private const string HashFunction = "12";
        private const string HashFunctionDigestLength = "20";

        /// <summary>
        /// Calculates CID V1 from data for Web3.
        /// </summary>
        /// <param name="bytes">Data to calculate CID V1 from.</param>
        /// <param name="responseDelegate">
        /// Delegate to handle the response. Will also be called if an error occured.
        /// </param>
        public static async void CalculateCidFromDataForWeb3(byte[] bytes, CalculateCidFromDataForWeb3Delegate responseDelegate)
        {
            (bool success, string errorMessage, string cid) result = await CalculateCidFromDataForWeb3Async(bytes);
            responseDelegate?.Invoke(result.success, result.errorMessage, result.cid);
        }

        /// <summary>
        /// Calculates CID V1 from data for Web3.
        /// </summary>
        /// <param name="bytes">Data to calculate CID V1 from.</param>
        /// <returns>True, if calculation was successful.</returns>
        public static async Task<(bool success, string errorMessage, string cid)> CalculateCidFromDataForWeb3Async(byte[] bytes)
        {
            string outCid = await Task<string>.Factory.StartNew(
                delegate()
                {
                    byte[] hashedDataAsBytes = HashUtils.Sha256FromBytes(bytes);

                    string hashedDataAsHex = StringUtils.BytesToHex(hashedDataAsBytes);

                    string cidNonBased =
                        CidVersion +
                        CodecVersionRawBinary +
                        HashFunction +
                        HashFunctionDigestLength +
                        hashedDataAsHex;

                    byte[] cidNonBasedBytes = new byte[cidNonBased.Length / 2];
                    StringUtils.HexToBytes(cidNonBased, cidNonBasedBytes);

                    string cid = "b" + Base32.Encode(cidNonBasedBytes).ToLower();

                    cid = cid.Replace('=', ' ');
                    cid = cid.TrimEnd();

                    return cid;
                });

            return (true, "", outCid);
        }
    }
}