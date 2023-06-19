// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace ThreeStudio.IPFS.Examples
{
    public static class ExampleUtils
    {
        public const IpfsHttpGateway DefaultIpfsHttpGateway = IpfsHttpGateway.W3SLink;
        public const IpfsPinningService DefaultIpfsPinningService = IpfsPinningService.Web3Storage;

        private class OptionDataWithUserObject<T> : TMP_Dropdown.OptionData
        {
            public readonly T UserObject;

            public OptionDataWithUserObject(T userObject, string text)
            {
                UserObject = userObject;
                this.text = text;
            }
        }

        public static void LogResult(bool success, string error, HttpResponse response)
        {
            if(success)
            {
                Debug.Log($"Response: {response}");
            }
            else
            {
                Debug.LogError($"Error: {error}. Response: {response}");
            }
        }

        public static void AssertAllRefsNotNull(params object[] objects)
        {
            foreach(object obj in objects)
            {
                Assert.IsNotNull(obj, "One or more references are unset. Check serialized fields in inspector.");
            }
        }

        public static void PopulateDropdown<T>(TMP_Dropdown dropdown, T defaultValue)
        {
            dropdown.options.Clear();

            int defaultValueInt = 0;
            int index = 0;
            foreach(T gateway in Enum.GetValues(typeof(T)))
            {
                dropdown.options.Add(new OptionDataWithUserObject<T>(gateway, gateway.ToString()));
                if(Equals(gateway, defaultValue))
                {
                    defaultValueInt = index;
                }

                index++;
            }

            dropdown.value = defaultValueInt;
            dropdown.RefreshShownValue();
        }

        public static T GetSelectedValueFromDropdown<T>(TMP_Dropdown dropdown)
        {
            return ((OptionDataWithUserObject<T>)dropdown.options[dropdown.value]).UserObject;
        }

        public static string GetBearerTokenDirectory()
        {
            return Path.Combine(Application.persistentDataPath, "bearer-tokens");
        }

        public static string GetBearerTokenByPinningService(IpfsPinningService pinningService)
        {
            switch(pinningService)
            {
            case IpfsPinningService.Web3Storage:
            case IpfsPinningService.NftStorage:
            {
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(pinningService), pinningService, null);
            }
            }

            string filename = $"{pinningService}.txt";
            string bearerTokenFilepath = Path.Combine(GetBearerTokenDirectory(), filename);
            if(!File.Exists(bearerTokenFilepath))
            {
                return null;
            }

            return File.ReadAllText(bearerTokenFilepath);
        }

        public static void SetBearerTokenByPinningService(IpfsPinningService pinningService, string bearerToken)
        {
            switch(pinningService)
            {
            case IpfsPinningService.Web3Storage:
            case IpfsPinningService.NftStorage:
            {
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(pinningService), pinningService, null);
            }
            }

            string baseDirectory = GetBearerTokenDirectory();
            if(!Directory.Exists(baseDirectory))
            {
                if(!Directory.CreateDirectory(baseDirectory).Exists)
                {
                    throw new Exception($"Could not create directory: ${baseDirectory}");
                }
            }

            string filename = $"{pinningService}.txt";
            string bearerTokenFilepath = Path.Combine(baseDirectory, filename);
            StreamWriter streamWriter = File.CreateText(bearerTokenFilepath);
            streamWriter.Write(bearerToken);
            streamWriter.Close();
        }
    }
}