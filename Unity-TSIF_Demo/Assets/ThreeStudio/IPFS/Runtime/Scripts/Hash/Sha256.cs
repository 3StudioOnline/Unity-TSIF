// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.Runtime.CompilerServices;

namespace ThreeStudio.IPFS.Internal
{
    /// <summary>
    /// This class implements the SHA-256 hash function.
    /// </summary>
    public class Sha256
    {
        private const int BlockSize = 64;

        private readonly uint[] _k =
        {
            0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5,
            0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
            0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3,
            0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
            0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc,
            0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
            0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7,
            0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
            0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13,
            0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
            0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3,
            0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
            0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5,
            0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
            0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208,
            0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2,
        };

        private uint[] _w;

        private uint _h0 = 0x6a09e667;
        private uint _h1 = 0xbb67ae85;
        private uint _h2 = 0x3c6ef372;
        private uint _h3 = 0xa54ff53a;
        private uint _h4 = 0x510e527f;
        private uint _h5 = 0x9b05688c;
        private uint _h6 = 0x1f83d9ab;
        private uint _h7 = 0x5be0cd19;

        private int _count;
        private byte[] _buffer;

        private void Initialize()
        {
            _w = new uint[64];
            _buffer = new byte[64];
        }

        private void Update(byte[] bytes, int offset, int len)
        {
            int n = _count % BlockSize;
            _count += len;
            int partLen = BlockSize - n;
            int i = 0;

            if(len >= partLen)
            {
                for(int index = 0; index < partLen; index++)
                {
                    _buffer[n + index] = bytes[offset + index];
                }

                Sha(_buffer, 0);

                for(i = partLen; i + BlockSize - 1 < len; i += BlockSize)
                {
                    Sha(bytes, offset + i);
                }

                n = 0;
            }

            if(i < len)
            {
                for(int index = 0; index < len - i; index++)
                {
                    _buffer[n + index] = bytes[offset + i + index];
                }
            }
        }

        private byte[] Digest()
        {
            byte[] tail = PadBuffer();
            Update(tail, 0, tail.Length);

            return GetResult();
        }

        private byte[] PadBuffer()
        {
            int n = _count % BlockSize;
            int padding = n < 56 ? 56 - n : 120 - n;
            byte[] result = new byte[padding + 8];
            result[0] = 0x80;

            long bits = _count << 3;
            result[padding + 0] = (byte)(bits >> 56);
            result[padding + 1] = (byte)(bits >> 48);
            result[padding + 2] = (byte)(bits >> 40);
            result[padding + 3] = (byte)(bits >> 32);
            result[padding + 4] = (byte)(bits >> 24);
            result[padding + 5] = (byte)(bits >> 16);
            result[padding + 6] = (byte)(bits >> 8);
            result[padding + 7] = (byte)bits;

            return result;
        }

        private byte[] GetResult()
        {
            byte[] result = new byte[32];

            result[0] = (byte)(_h0 >> 24);
            result[1] = (byte)(_h0 >> 16);
            result[2] = (byte)(_h0 >> 8);
            result[3] = (byte)_h0;

            result[4] = (byte)(_h1 >> 24);
            result[5] = (byte)(_h1 >> 16);
            result[6] = (byte)(_h1 >> 8);
            result[7] = (byte)_h1;

            result[8] = (byte)(_h2 >> 24);
            result[9] = (byte)(_h2 >> 16);
            result[10] = (byte)(_h2 >> 8);
            result[11] = (byte)_h2;

            result[12] = (byte)(_h3 >> 24);
            result[13] = (byte)(_h3 >> 16);
            result[14] = (byte)(_h3 >> 8);
            result[15] = (byte)_h3;

            result[16] = (byte)(_h4 >> 24);
            result[17] = (byte)(_h4 >> 16);
            result[18] = (byte)(_h4 >> 8);
            result[19] = (byte)_h4;

            result[20] = (byte)(_h5 >> 24);
            result[21] = (byte)(_h5 >> 16);
            result[22] = (byte)(_h5 >> 8);
            result[23] = (byte)_h5;

            result[24] = (byte)(_h6 >> 24);
            result[25] = (byte)(_h6 >> 16);
            result[26] = (byte)(_h6 >> 8);
            result[27] = (byte)_h6;

            result[28] = (byte)(_h7 >> 24);
            result[29] = (byte)(_h7 >> 16);
            result[30] = (byte)(_h7 >> 8);
            result[31] = (byte)_h7;

            return result;
        }

        private void Sha(byte[] input, int offset)
        {
            uint a = _h0;
            uint b = _h1;
            uint c = _h2;
            uint d = _h3;
            uint e = _h4;
            uint f = _h5;
            uint g = _h6;
            uint h = _h7;
            uint u;
            uint t2;
            uint r;

            for(r = 0; r < 16; r++)
            {
                _w[r] = ((uint)input[offset + 0] << 24)
                        | (((uint)input[offset + 1] & 0xFF) << 16)
                        | (((uint)input[offset + 2] & 0xFF) << 8)
                        | ((uint)input[offset + 3] & 0xFF);
                offset += 4;
            }

            for(r = 16; r < 64; r++)
            {
                u = _w[r - 2];
                t2 = _w[r - 15];

                _w[r] = ((ShiftRight(u, 17) | (u << 15)) ^ (ShiftRight(u, 19) | (u << 13)) ^ ShiftRight(u, 10))
                        + _w[r - 7]
                        + ((ShiftRight(t2, 7) | (t2 << 25)) ^ (ShiftRight(t2, 18) | (t2 << 14)) ^ ShiftRight(t2, 3))
                        + _w[r - 16];
            }

            for(r = 0; r < 64; r++)
            {
                u = h +
                    ((ShiftRight(e, 6) | (e << 26)) ^ (ShiftRight(e, 11) | (e << 21)) ^ (ShiftRight(e, 25) | (e << 7)))
                    + ((e & f) ^ (~e & g))
                    + _k[r]
                    + _w[r];

                t2 = ((ShiftRight(a, 2) | (a << 30)) ^ (ShiftRight(a, 13) | (a << 19)) ^ (ShiftRight(a, 22) | (a << 10)))
                     + ((a & b) ^ (a & c) ^ (b & c));

                h = g;
                g = f;
                f = e;
                e = d + u;
                d = c;
                c = b;
                b = a;
                a = u + t2;
            }

            _h0 += a;
            _h1 += b;
            _h2 += c;
            _h3 += d;
            _h4 += e;
            _h5 += f;
            _h6 += g;
            _h7 += h;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private uint ShiftRight(uint a, int b)
        {
            return a >> b;
        }
        
        /// <summary>
        /// Generates a SHA-256 hash, always returns 32 bytes.
        /// </summary>
        /// <param name="bytes">The bytes to hash.</param>
        /// <returns>Hash (32 bytes)</returns>
        public static byte[] Hash(byte[] bytes)
        {
            Sha256 instance = new();
            instance.Initialize();
            instance.Update(bytes, 0, bytes.Length);

            return instance.Digest();
        }
        
        
        /// <summary>
        /// Generates a SHA-256 hash, always returns 32 bytes.
        /// </summary>
        /// <param name="bytes">The bytes to hash.</param>
        /// <param name="index">Starting index.</param>
        /// <param name="length">Number of bytes.</param>
        /// <returns>Hash (32 bytes)</returns>
        public static byte[] Hash(byte[] bytes, int index, int length)
        {
            Sha256 instance = new();
            instance.Initialize();

            if(bytes.Length <= 0)
            {
                return instance.Digest();
            }

            if(index < 0)
            {
                length += index;
                index = 0;
            }

            if(length > bytes.Length - index)
            {
                length = bytes.Length - index;
            }

            if(length <= 0)
            {
                return instance.Digest();
            }

            instance.Update(bytes, index, length);

            return instance.Digest();
        }
    }
}