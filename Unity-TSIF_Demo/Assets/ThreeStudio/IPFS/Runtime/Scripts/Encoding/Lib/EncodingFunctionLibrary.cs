// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using ThreeStudio.IPFS.Internal;

namespace ThreeStudio.IPFS
{
    /// <summary>
    /// Encoding Function Library.
    /// </summary>
    public static class EncodingFunctionLibrary
    {
        /// <summary>
        /// Encodes data using Base32 (RFC 4648).
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string EncodeBase32(string source)
        {
            return Base32.Encode(source);
        }

        /// <summary>
        /// Encodes data using Base32 (RFC 4648).
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string EncodeBase32(byte[] source)
        {
            return Base32.Encode(source);
        }

        /// <summary>
        /// Decodes a Base32-encoded string (RFC 4648).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="outDest"></param>
        /// <returns></returns>
        public static bool DecodeBase32(string source, out string outDest)
        {
            return Base32.Decode(source, out outDest);
        }

        /// <summary>
        /// Decodes a Base32-encoded string (RFC 4648).
        /// </summary>
        /// <param name="source"></param>
        /// <param name="outDest"></param>
        /// <returns></returns>
        public static bool DecodeBase32(string source, out byte[] outDest)
        {
            return Base32.Decode(source, out outDest);
        }
    }
}