// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.Threading.Tasks;
using ThreeStudio.IPFS.Internal;
using UnityEngine;
using UnityEngine.Networking;

namespace ThreeStudio.IPFS.Http
{
    public static partial class HttpRequest
    {
        public delegate void HttpGetResponseDelegate(bool success, string errorMessage, HttpResponse response);


        /// <summary>
        /// Sends an HTTP GET request to the specified URL.
        /// </summary>
        /// <param name="url">The URL the request is sent to.</param>
        /// <param name="id">The correlational ID for the HTTP GET request.</param>
        /// <param name="responseDelegate">
        /// Delegate to handle the response. Will also be called if a request could not be sent successfully.
        /// </param>
        public static async void SendGetRequest(
            string url,
            string id,
            HttpGetResponseDelegate responseDelegate)
        {
            (bool success, string errorMessage, HttpResponse response) result = await SendGetRequestAsync(url, id);
            responseDelegate?.Invoke(result.success, result.errorMessage, result.response);
        }

        /// <summary>
        /// Sends an HTTP GET request to the specified URL.
        /// </summary>
        /// <param name="url">The URL the request is sent to.</param>
        /// <param name="id">The correlational ID for the HTTP GET request.</param>
        /// <returns>Whether the request was successful, an optional error message and HTTP response.</returns>
        public static async Task<(bool success, string errorMessage, HttpResponse response)> SendGetRequestAsync(
            string url,
            string id)
        {
            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.DownloadFileOrGetData))
            {
                Debug.Log($"[{id}] Sending HTTP GET Request to URL: {url}");
            }
            #endif

            using UnityWebRequest webRequest = new UnityWebRequest();
            webRequest.method = UnityWebRequest.kHttpVerbGET;
            webRequest.url = url;
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            await webRequest.SendWebRequest().AsIpfsTask();

            bool wasSuccessful = webRequest.result == UnityWebRequest.Result.Success;
            bool statusCodeOk = webRequest.responseCode >= 200 && webRequest.responseCode <= 299;
            HttpResponse response = new HttpResponse();
            response.Success = wasSuccessful && statusCodeOk;
            response.StatusCode = webRequest.responseCode;
            response.Headers = webRequest.GetResponseHeaders();
            response.Body = webRequest.downloadHandler.data;

            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.DownloadFileOrGetData))
            {
                Debug.Log($"[{id}] Response received. HTTP Status Code: {response.StatusCode}");
                if(response.Headers != null)
                {
                    Debug.Log($"[{id}] Headers:");
                    foreach(var header in response.Headers)
                    {
                        Debug.Log($"[{id}] {header.Key}: {header.Value}");
                    }
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
                #if DEVELOPMENT_BUILD || UNITY_EDITOR
                if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.DownloadFileOrGetData))
                {
                    Debug.LogError($"[{id}] {webRequest.error}");
                }
                #endif

                return (false, webRequest.error, response);
            }

            return (true, "", response);
        }
    }
}