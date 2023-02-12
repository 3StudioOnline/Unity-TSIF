// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.Text;
using UnityEngine;

namespace ThreeStudio.IPFS.Internal
{
    public static class StringUtils
    {
        public static void HexToBytes(string hex, byte[] outBytes)
        {
            int hexCount = hex.Length;

            int outPos = 0;
            int inPos = 0;
            if(hexCount % 2 == 1)
            {
                outBytes[outPos++] = CharToNibble(hex[inPos++]);
            }

            while(inPos < hexCount)
            {
                byte hiNibble = (byte)(CharToNibble(hex[inPos++]) << 4);
                outBytes[outPos++] = (byte)(hiNibble | CharToNibble(hex[inPos++]));
            }
        }

        public static string BytesToHex(byte[] input)
        {
            StringBuilder result = new(input.Length * 2);
            for(int i = 0; i < input.Length; ++i)
            {
                ByteToHex(input[i], result);
            }

            return result.ToString();
        }
        
        /// <summary>
        /// Converts string into a byte array using UTF-8 encoding.
        /// </summary>
        /// <param name="text">String to convert.</param>
        /// <returns>Byte array from string.</returns>
        public static byte[] StringToBytes(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }
        
        /// <summary>
        /// Tries to convert a byte array into string using UTF-8 encoding.
        /// </summary>
        /// <param name="data">UTF8-encoded data.</param>
        /// <returns>String from byte array; will be empty if conversion fails.</returns>
        public static string BytesToString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        private static void ByteToHex(byte input, StringBuilder result)
        {
            result.Append(NibbleToTChar((byte)(input >> 4)));
            result.Append(NibbleToTChar((byte)(input & 15)));
        }

        private static string NibbleToTChar(byte num)
        {
            return num.ToString("x1");
        }

        private static byte CharToNibble(char hex)
        {
            switch(hex)
            {
            case >= '0' and <= '9': return (byte)(hex - '0');
            case >= 'A' and <= 'F': return (byte)(hex - 'A' + 10);
            case >= 'a' and <= 'f': return (byte)(hex - 'a' + 10);
            default:
                Debug.LogError($"{hex}' is not a valid hexadecimal digit");
                return 0;
            }
        }
    }
}