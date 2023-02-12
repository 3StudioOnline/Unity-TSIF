// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThreeStudio.IPFS.Internal;
using UnityEngine;
using UnityEngine.Networking;

namespace ThreeStudio.IPFS.Http
{
    public static partial class HttpRequest
    {
        public delegate void HttpPostResponseDelegate(bool success, string errorMessage, HttpResponse response);

        /// <summary>
        /// Sends an HTTP POST request to the specified URL.
        /// </summary>
        /// <param name="url">The URL the request is sent to.</param>
        /// <param name="headers">The request headers.</param>
        /// <param name="body">The request body data.</param>
        /// <param name="id">The correlational ID for the HTTP POST request.</param>
        /// <param name="responseDelegate">
        /// Delegate to handle the response. Will also be called if a request could not be sent
        /// successfully.
        /// </param>
        public static async void SendPostRequest(
            string url,
            Dictionary<string, string> headers,
            byte[] body,
            string id,
            HttpPostResponseDelegate responseDelegate)
        {
            var result = await SendPostRequestAsync(url, headers, body, id);
            responseDelegate?.Invoke(result.response.Success, result.errorMessage, result.response);
        }


        /// <summary>
        /// Sends an HTTP POST request to the specified URL.
        /// </summary>
        /// <param name="url">The URL the request is sent to.</param>
        /// <param name="headers">The request headers.</param>
        /// <param name="body">The request body data.</param>
        /// <param name="id">The correlational ID for the HTTP POST request.</param>
        /// <returns>Whether the request was successful, an optional error message and HTTP response.</returns>
        public static async Task<(bool success, string errorMessage, HttpResponse response)> SendPostRequestAsync(
            string url,
            Dictionary<string, string> headers,
            byte[] body,
            string id)
        {
            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.UploadFileOrData))
            {
                Debug.Log($"[{id}] Sending HTTP POST Request to URL: {url}");
            }
            #endif

            using var webRequest = new UnityWebRequest();
            webRequest.method = UnityWebRequest.kHttpVerbPOST;
            webRequest.url = url;
            webRequest.uploadHandler = new UploadHandlerRaw(body);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            if(headers != null)
            {
                foreach(var header in headers)
                {
                    #if DEVELOPMENT_BUILD || UNITY_EDITOR
                    if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.UploadFileOrData))
                    {
                        Debug.LogFormat(
                            "[{0}] Header: Key<{1}> Value<{2}>",
                            id,
                            header.Key,
                            header.Key == "Authorization" ? "********" : header.Value);
                    }
                    #endif

                    webRequest.SetRequestHeader(header.Key, header.Value);
                }
            }

            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.UploadFileOrData))
            {
                Debug.Log($"[{id}] Content size: {body.Length} Byte(s)");
            }
            #endif

            await webRequest.SendWebRequest().AsIpfsTask();

            bool wasSuccessful = webRequest.result == UnityWebRequest.Result.Success;
            bool statusCodeOk = webRequest.responseCode >= 200 && webRequest.responseCode <= 299;
            HttpResponse response = new();
            response.Success = wasSuccessful && statusCodeOk;
            response.StatusCode = webRequest.responseCode;
            response.Headers = webRequest.GetResponseHeaders();
            response.Body = webRequest.downloadHandler.data;

            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.UploadFileOrData))
            {
                Debug.Log($"[{id}] Response received. HTTP Status Code: {response.StatusCode}");
                Debug.Log($"[{id}] Headers:");
                foreach(var header in response.Headers)
                {
                    Debug.Log($"[{id}] {header.Key}: {header.Value}");
                }

                if(HttpUtils.IsContentPrintable(HttpUtils.GetContentType(response)))
                {
                    Debug.Log($"[{id}] Body:");
                    Debug.Log($"[{id}] {StringUtils.BytesToString(response.Body)}");
                }
                else
                {
                    Debug.Log($"[{id}] Body: {response.Body?.Length} Byte(s)");
                }
            }
            #endif

            if(!response.Success)
            {
                string errorMessage =
                    response.StatusCode == 0
                    && !response.Headers.Any()
                    && response.Body?.Length == 0
                        ? "Connection error."
                        : $"HTTP Status Code = {response.StatusCode}";

                #if DEVELOPMENT_BUILD || UNITY_EDITOR
                if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.UploadFileOrData))
                {
                    Debug.LogError($"[{id}] {errorMessage}");
                }
                #endif

                return (false, errorMessage, response);
            }

            return (true, "", response);
        }
    }
}