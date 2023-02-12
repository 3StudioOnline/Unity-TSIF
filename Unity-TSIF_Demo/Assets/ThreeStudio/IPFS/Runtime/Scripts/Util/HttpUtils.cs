// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using UnityEngine;

namespace ThreeStudio.IPFS.Internal
{
    public static class HttpUtils
    {
        private static long _correlationIdCounter = 1000;

        public static string GenerateHttpCorrelationID()
        {
            long id = Interlocked.Increment(ref _correlationIdCounter);
            return id.ToString();
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static bool IsContentPrintable(string mimeType)
        {
            if(mimeType == null)
            {
                return false;
            }

            if(mimeType.StartsWith("text/", true, CultureInfo.InvariantCulture)) { return true; }

            if(mimeType.StartsWith("application/json", true, CultureInfo.InvariantCulture)) { return true; }

            return false;
        }

        public static string GetContentType(HttpResponse response)
        {
            if(response == null)
            {
                return null;
            }

            foreach(string key in response.Headers.Keys)
            {
                if("content-type".Equals(key.ToLowerInvariant()))
                {
                    return response.Headers[key];
                }
            }

            return null;
        }

        /**
         * Maps a file extension to a mime-type.
         *
         * Implementation according to MDN Web Docs:
         * https://github.com/mdn/content/blob/abb45969b32cc406d0381b6e3bc96f603ec606eb/files/en-us/web/http/basics_of_http/mime_types/common_types/index.md
         */
        // ReSharper disable once MemberCanBePrivate.Global
        public static string GetMimeTypeByExtension(string extension)
        {
            string extLower = extension.ToLower();

            return extLower switch
            {
                "aac" => "audio/aac",
                "abw" => "application/x-abiword",
                "arc" => "application/x-freearc",
                "avif" => "image/avif",
                "avi" => "video/x-msvideo",
                "azw" => "application/vnd.amazon.ebook",
                "bin" => "application/octet-stream",
                "bmp" => "image/bmp",
                "bz" => "application/x-bzip",
                "bz2" => "application/x-bzip2",
                "cda" => "application/x-cdf",
                "csh" => "application/x-csh",
                "css" => "text/css",
                "csv" => "text/csv",
                "doc" => "application/msword",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "eot" => "application/vnd.ms-fontobject",
                "epub" => "application/epub+zip",
                "gz" => "application/gzip",
                "gif" => "image/gif",
                "htm" => "text/html",
                "html" => "text/html",
                "ico" => "image/vnd.microsoft.icon",
                "ics" => "text/calendar",
                "jar" => "application/java-archive",
                "jpeg" => "image/jpeg",
                "jpg" => "image/jpeg",
                "js" => "text/javascript",
                "json" => "application/json",
                "jsonld" => "application/ld+json",
                "mid" => "audio/midi",
                "midi" => "audio/midi",
                "mjs" => "text/javascript",
                "mp3" => "audio/mpeg",
                "mp4" => "video/mp4",
                "mpeg" => "video/mpeg",
                "mpkg" => "application/vnd.apple.installer+xml",
                "odp" => "application/vnd.oasis.opendocument.presentation",
                "ods" => "application/vnd.oasis.opendocument.spreadsheet",
                "odt" => "application/vnd.oasis.opendocument.text",
                "oga" => "audio/ogg",
                "ogv" => "video/ogg",
                "ogx" => "application/ogg",
                "opus" => "audio/opus",
                "otf" => "font/otf",
                "png" => "image/png",
                "pdf" => "application/pdf",
                "php" => "application/x-httpd-php",
                "ppt" => "application/vnd.ms-powerpoint",
                "pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                "rar" => "application/vnd.rar",
                "rtf" => "application/rtf",
                "sh" => "application/x-sh",
                "svg" => "image/svg+xml",
                "swf" => "application/x-shockwave-flash",
                "tar" => "application/x-tar",
                "tif" => "image/tiff",
                "tiff" => "image/tiff",
                "ts" => "video/mp2t",
                "ttf" => "font/ttf",
                "txt" => "text/plain",
                "vsd" => "application/vnd.visio",
                "wav" => "audio/wav",
                "weba" => "audio/webm",
                "webm" => "video/webm",
                "webp" => "image/webp",
                "woff" => "font/woff",
                "woff2" => "font/woff2",
                "xhtml" => "application/xhtml+xml",
                "xls" => "application/vnd.ms-excel",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "xml" => "application/xml",
                "xul" => "application/vnd.mozilla.xul+xml",
                "zip" => "application/zip",
                "3gp" => "video/3gpp",
                "3g2" => "video/3gpp2",
                "7z" => "application/x-7z-compressed",
                _ => "application/octet-stream",
            };
        }

        public static string MultipartFormData_GenerateBoundary()
        {
            return "--------" + Guid.NewGuid();
        }

        public static Dictionary<string, string> MultipartFormData_BuildHeaders(string boundary, string bearerToken)
        {
            return new Dictionary<string, string>
            {
                { "Accept", "application/json" },
                { "Authorization", $"Bearer {bearerToken}" },
                { "Content-Type", $"multipart/form-data; boundary={boundary}" },
            };
        }

        public static byte[] MultipartFormData_AddFile(string boundary, string filename, byte[] contentAsBytes)
        {
            const string ipfsPathPrefix = "root/";

            filename ??= "";

            string ext = Path.GetExtension(filename).TrimStart('.');
            string mimeType = GetMimeTypeByExtension(ext);

            string entry = "";
            entry += $"--{boundary}\r\n";
            entry += $"Content-Disposition: form-data; name=\"file\"; filename=\"{ipfsPathPrefix + filename.TrimStart().TrimEnd()}\"\r\n";
            entry += $"Content-Type: {mimeType}\r\n";
            entry += "\r\n";

            var buffer = new List<byte>();
            buffer.AddRange(StringUtils.StringToBytes(entry));
            buffer.AddRange(contentAsBytes);
            buffer.AddRange(StringUtils.StringToBytes("\r\n"));

            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.UploadFileOrData))
            {
                if(IsContentPrintable(mimeType))
                {
                    Debug.Log(entry + StringUtils.BytesToString(contentAsBytes));
                }
                else
                {
                    Debug.Log($"{entry}[{contentAsBytes.Length} Bytes(s)]");
                }
            }
            #endif

            return buffer.ToArray();
        }

        public static byte[] MultipartFormData_AddFile(string boundary, string filename, string contentAsString)
        {
            return MultipartFormData_AddFile(
                boundary,
                filename,
                StringUtils.StringToBytes(contentAsString));
        }

        public static byte[] MultipartFormData_End(string boundary)
        {
            string entry = $"--{boundary}--";

            #if DEVELOPMENT_BUILD || UNITY_EDITOR
            if(Ipfs.IsDebugEnabled(Ipfs.DebugMode.UploadFileOrData))
            {
                Debug.Log(entry);
            }
            #endif

            return StringUtils.StringToBytes(entry);
        }
    }
}