// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using ThreeStudio.IPFS.Internal;

namespace ThreeStudio.IPFS
{
    /// <summary>
    /// Hash Function Blueprint Library.
    ///
    /// Available hash functions:
    /// - SHA-256
    /// </summary>
    public static class HashUtils
    {
        /// <summary>
        /// Generates a SHA-256 hash from bytes, always returns 32 bytes.
        /// </summary>
        /// <param name="byteArray">The bytes to hash.</param>
        /// <returns>Hash (32 bytes)</returns>
        public static byte[] Sha256FromBytes(byte[] byteArray)
        {
            return Sha256.Hash(byteArray, 0, byteArray.Length);
        }

        /// <summary>
        /// Generates a SHA-256 hash from string, always returns 32 bytes.
        /// </summary>
        /// <param name="text">Text to hash.</param>
        /// <returns>Hash (32 bytes)</returns>
        public static byte[] Sha256FromString(string text)
        {
            byte[] byteArray = StringUtils.StringToBytes(text);
            return Sha256.Hash(byteArray, 0, byteArray.Length);
        }
    }
}