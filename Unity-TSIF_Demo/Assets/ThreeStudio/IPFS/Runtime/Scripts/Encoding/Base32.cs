// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.Text;

namespace ThreeStudio.IPFS.Internal
{
    public static class Base32
    {
        /// <summary>
        /// The table used to encode a 6 bit value as an ascii character.
        /// </summary>
        private static readonly char[] EncodingAlphabet =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '2', '3', '4', '5', '6', '7',
        };

        /// <summary>
        /// The table used to convert an ascii character into a 6 bit value.
        /// </summary>
        private static readonly byte[] DecodingAlphabet =
        {
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0x00-0x0f 15
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0x10-0x1f 31
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0x20-0x2f 47
            0xFF, 0xFF, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0x30-0x3f 63
            0xFF, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, // 0x40-0x4f 79
            0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0x50-0x5f 95
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0x60-0x6f 111
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0x70-0x7f 127
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0x80-0x8f 143
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0x90-0x9f 159
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0xa0-0xaf 175
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0xb0-0xbf 191
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0xc0-0xcf 207
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0xd0-0xdf 223
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0xe0-0xef 239
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, // 0xf0-0xff 255
        };

        /// <summary>
        /// Encodes a FString into a Base32 string.
        /// </summary>
        /// <param name="source">The string data to convert.</param>
        /// <returns>
        /// A string that encodes the binary data in a way that can be safely transmitted via various
        /// Internet protocols.
        /// </returns>
        public static string Encode(string source)
        {
            return Encode(StringUtils.StringToBytes(source));
        }

        /// <summary>
        /// Encodes a binary uint8 array into a Base32 string.
        /// </summary>
        /// <param name="source">The binary data to convert.</param>
        /// <returns>
        /// A string that encodes the binary data in a way that can be safely transmitted via various
        /// Internet protocols.
        /// </returns>
        public static string Encode(byte[] source)
        {
            return Encode(source, source.Length);
        }

        /// <summary>
        /// Encodes the source into a Base32 string.
        /// </summary>
        /// <param name="source">The binary data to encode.</param>
        /// <param name="length">Length of the binary data to be encoded.</param>
        /// <returns>Base32 encoded string containing the binary data.</returns>
        private static string Encode(byte[] source, int length)
        {
            int expectedLength = GetEncodedDataSize(length);

            StringBuilder outBuffer = new(expectedLength);
            Encode(source, length, outBuffer);
            return outBuffer.ToString();
        }

        /// <summary>
        /// Encodes the source into a Base32 string, storing it in a pre-allocated buffer.
        /// </summary>
        /// <param name="source">The binary data to encode.</param>
        /// <param name="length">Length of the binary data to be encoded.</param>
        /// <param name="outDest">
        /// Buffer to receive the encoded data. Must be large enough to contain the entire output
        /// data (see GetEncodedDataSize()).
        /// </param>
        private static void Encode(byte[] source, int length, StringBuilder outDest)
        {
            int sourcePos = 0;

            char[] encodedBytes = { '1', '1', '1', '1', '1', '1', '1', '1' };
            // Loop through the buffer converting 5 bytes of binary data at a time
            while(length >= 5)
            {
                long a = source[sourcePos++];
                long b = source[sourcePos++];
                long c = source[sourcePos++];
                long d = source[sourcePos++];
                long e = source[sourcePos++];
                length -= 5;

                // The algorithm takes 40 bits of data (5 bytes) and breaks it into 8 5bit chunks represented as ascii
                long byteTuple = (a << 32) | (b << 24) | (c << 16) | (d << 8) | e;

                // Use the 5bit block to find the representation ascii character for it
                encodedBytes[7] = EncodingAlphabet[byteTuple & 0x1F];
                byteTuple >>= 5;
                encodedBytes[6] = EncodingAlphabet[byteTuple & 0x1F];
                byteTuple >>= 5;
                encodedBytes[5] = EncodingAlphabet[byteTuple & 0x1F];
                byteTuple >>= 5;
                encodedBytes[4] = EncodingAlphabet[byteTuple & 0x1F];
                byteTuple >>= 5;
                encodedBytes[3] = EncodingAlphabet[byteTuple & 0x1F];
                byteTuple >>= 5;
                encodedBytes[2] = EncodingAlphabet[byteTuple & 0x1F];
                byteTuple >>= 5;
                encodedBytes[1] = EncodingAlphabet[byteTuple & 0x1F];
                byteTuple >>= 5;
                encodedBytes[0] = EncodingAlphabet[byteTuple & 0x1F];
                foreach(char encodedByte in encodedBytes)
                {
                    outDest.Append(encodedByte);
                }
            }

            // Since this algorithm operates on blocks, we may need to pad the last chunks
            if(length > 0)
            {
                long byteTuple = 0;
                long a = source[sourcePos++];
                long b = 0;
                long c = 0;
                long d = 0;
                long e = 0;

                if(length == 4)
                {
                    b = source[sourcePos++];
                    c = source[sourcePos++];
                    d = source[sourcePos++];
                    byteTuple = (a << 32) | (b << 24) | (c << 16) | (d << 8) | e;
                    encodedBytes[7] = '=';
                    byteTuple >>= 5;
                    encodedBytes[6] = EncodingAlphabet[byteTuple & 0x1F];
                    byteTuple >>= 5;
                    encodedBytes[5] = EncodingAlphabet[byteTuple & 0x1F];
                    byteTuple >>= 5;
                    encodedBytes[4] = EncodingAlphabet[byteTuple & 0x1F];
                    byteTuple >>= 5;
                    encodedBytes[3] = EncodingAlphabet[byteTuple & 0x1F];
                    byteTuple >>= 5;
                    encodedBytes[2] = EncodingAlphabet[byteTuple & 0x1F];
                }

                if(length == 3)
                {
                    b = source[sourcePos++];
                    c = source[sourcePos++];
                    byteTuple = (a << 32) | (b << 24) | (c << 16) | (d << 8) | e;
                    encodedBytes[7] = '=';
                    byteTuple >>= 5;
                    encodedBytes[6] = '=';
                    byteTuple >>= 5;
                    encodedBytes[5] = '=';
                    byteTuple >>= 5;
                    encodedBytes[4] = EncodingAlphabet[byteTuple & 0x1F];
                    byteTuple >>= 5;
                    encodedBytes[3] = EncodingAlphabet[byteTuple & 0x1F];
                    byteTuple >>= 5;
                    encodedBytes[2] = EncodingAlphabet[byteTuple & 0x1F];
                }

                if(length == 2)
                {
                    b = source[sourcePos];
                    byteTuple = (a << 32) | (b << 24) | (c << 16) | (d << 8) | e;
                    encodedBytes[7] = '=';
                    byteTuple >>= 5;
                    encodedBytes[6] = '=';
                    byteTuple >>= 5;
                    encodedBytes[5] = '=';
                    byteTuple >>= 5;
                    encodedBytes[4] = '=';
                    byteTuple >>= 5;
                    encodedBytes[3] = EncodingAlphabet[byteTuple & 0x1F];
                    byteTuple >>= 5;
                    encodedBytes[2] = EncodingAlphabet[byteTuple & 0x1F];
                }

                // If there's only one 1 uint8 left in the source, then you need 2 pad chars
                if(length == 1)
                {
                    byteTuple = (a << 32) | (b << 24) | (c << 16) | (d << 8) | e;
                    encodedBytes[7] = '=';
                    byteTuple >>= 5;
                    encodedBytes[6] = '=';
                    byteTuple >>= 5;
                    encodedBytes[5] = '=';
                    byteTuple >>= 5;
                    encodedBytes[4] = '=';
                    byteTuple >>= 5;
                    encodedBytes[3] = '=';
                    byteTuple >>= 5;
                    encodedBytes[2] = '=';
                }

                // Now encode the remaining bits the same way
                byteTuple >>= 5;
                encodedBytes[1] = EncodingAlphabet[byteTuple & 0x1F];
                byteTuple >>= 5;
                encodedBytes[0] = EncodingAlphabet[byteTuple & 0x1F];

                foreach(char encodedByte in encodedBytes)
                {
                    outDest.Append(encodedByte);
                }
            }
        }

        /// <summary>
        /// Get the encoded data size for the given number of bytes.
        /// </summary>
        /// <param name="numBytes">The number of bytes of input.</param>
        /// <returns>The number of characters in the encoded data.</returns>
        private static int GetEncodedDataSize(int numBytes)
        {
            return (numBytes + 4) * 8 / 5;
        }

        /// <summary>
        /// Decodes a Base32 string into a FString.
        /// </summary>
        /// <param name="source">The Base32 encoded string.</param>
        /// <param name="outDest">Receives the decoded string data.</param>
        /// <returns>True, if decoding was successful.</returns>
        public static bool Decode(string source, out string outDest)
        {
            outDest = null;
            if(Decode(source, out byte[] tempDest))
            {
                outDest = Encoding.ASCII.GetString(tempDest);
            }

            return outDest != null;
        }

        /// <summary>
        /// Decodes a Base32 string into an array of bytes.
        /// </summary>
        /// <param name="source">The Base32 encoded string.</param>
        /// <param name="outDest">Array to receive the decoded data.</param>
        /// <returns>True, if decoding was successful.</returns>
        public static bool Decode(string source, out byte[] outDest)
        {
            outDest = new byte[GetDecodedDataSize(source)];
            byte[] tempSource = Encoding.ASCII.GetBytes(source);
            return Decode(tempSource, tempSource.Length, outDest);
        }

        /// <summary>
        /// Decodes a Base32 string into a pre-allocated buffer.
        /// </summary>
        /// <param name="source">The Base32 encoded string.</param>
        /// <param name="length">Length of the Base32 encoded string.</param>
        /// <param name="outDest">Buffer to receive the decoded data.</param>
        /// <returns>True, if the buffer was decoded, false if it was invalid.</returns>
        private static bool Decode(byte[] source, int length, byte[] outDest)
        {
            // Remove the trailing '=' characters, so we can handle padded and non-padded input the same
            while(length > 0 && source[length - 1] == '=')
            {
                length--;
            }

            // Make sure the length is valid. Only lengths modulo 8 of 0, 2, 4, 5, and 7 are valid.
            if((length & 7) == 1 || (length & 7) == 3 || (length & 7) == 6)
            {
                return false;
            }

            int outPos = 0;
            int sourcePos = 0;
            // Convert all the full chunks of data
            for(; length >= 8; length -= 8)
            {
                // Decode the next 5 BYTEs
                ulong originalTuple = 0;
                for(int index = 0; index < 8; index++)
                {
                    byte sourceChar = source[sourcePos++];
                    byte decodedValue = DecodingAlphabet[sourceChar];
                    if(decodedValue == 0xFF)
                    {
                        return false;
                    }

                    originalTuple = (originalTuple << 5) | decodedValue;
                }

                // Now we can tear the uint32 into bytes
                outDest[outPos + 4] = (byte)originalTuple;
                originalTuple >>= 8;
                outDest[outPos + 3] = (byte)originalTuple;
                originalTuple >>= 8;
                outDest[outPos + 2] = (byte)originalTuple;
                originalTuple >>= 8;
                outDest[outPos + 1] = (byte)originalTuple;
                originalTuple >>= 8;
                outDest[outPos + 0] = (byte)originalTuple;

                // Move to the next output chunk
                outPos += 5;
            }

            // Convert the last chunk of data
            if(length > 0)
            {
                // Decode the next 5 BYTEs, or up to the end of the input buffer
                long originalTuple = 0;
                for(int index = 0; index < length; index++)
                {
                    byte sourceChar = source[sourcePos++];
                    byte decodedValue = DecodingAlphabet[sourceChar];
                    if(decodedValue == 0xFF)
                    {
                        return false;
                    }

                    originalTuple = (originalTuple << 5) | decodedValue;
                }

                // Now we can tear the uint32 into bytes
                for(int index = length; index < 8; index++)
                {
                    originalTuple = originalTuple << 5;
                }

                // Now we can tear the uint32 into bytes
                originalTuple >>= 8;
                if(length > 6)
                {
                    outDest[outPos + 3] = (byte)originalTuple;
                }

                originalTuple >>= 8;
                if(length > 4)
                {
                    outDest[outPos + 2] = (byte)originalTuple;
                }

                originalTuple >>= 8;
                if(length > 2)
                {
                    outDest[outPos + 1] = (byte)originalTuple;
                }

                originalTuple >>= 8;
                outDest[outPos + 0] = (byte)originalTuple;
            }

            return true;
        }

        /// <summary>
        /// Determine the decoded data size for the incoming Base32 encoded string.
        /// </summary>
        /// <param name="source">The Base32 encoded string.</param>
        /// <returns>The size in bytes of the decoded data.</returns>
        private static int GetDecodedDataSize(string source)
        {
            int numBytes = 0;
            if(!string.IsNullOrEmpty(source))
            {
                int length = source.Length;
                // Get the source length without the trailing padding characters
                while(length > 0 && source[length - 1] == '=')
                {
                    length--;
                }

                // Get the lower bound for number of bytes, excluding the last partial chunk
                numBytes = length / 8 * 5;
                if((length & 7) == 7)
                {
                    numBytes += 4;
                }
                else if((length & 7) == 5)
                {
                    numBytes += 3;
                }
                else if((length & 7) == 4)
                {
                    numBytes += 2;
                }
                else if((length & 7) == 2)
                {
                    numBytes++;
                }
            }

            return numBytes;
        }
    }
}