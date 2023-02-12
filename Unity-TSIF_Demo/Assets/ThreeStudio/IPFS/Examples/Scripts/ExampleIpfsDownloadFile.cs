// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System;
using System.IO;
using ThreeStudio.IPFS.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeStudio.IPFS.Examples
{
    public class ExampleIpfsDownloadFile : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _cmbIpfsHttpGateway;
        [SerializeField] private TMP_InputField _txtCid;
        [SerializeField] private TMP_InputField _txtPath;
        [SerializeField] private Toggle _tgCreatePathIfMissing;
        [SerializeField] private Toggle _tgOverwriteExistingFile;
        [SerializeField] private TMP_InputField _txtDownloadedFile;
        [SerializeField] private Button _btnDownloadFile;
        [SerializeField] private Button _btnOpenIpfsCacheDirectory;
        [SerializeField] private TMP_Text _txtResult;

        private void Awake()
        {
            Ipfs.SetDebugLogEnabled(true, Ipfs.DebugMode.DownloadFileOrGetData);

            ExampleUtils.AssertAllRefsNotNull(
                _cmbIpfsHttpGateway,
                _txtCid,
                _txtPath,
                _tgCreatePathIfMissing,
                _tgOverwriteExistingFile,
                _txtDownloadedFile,
                _btnDownloadFile,
                _btnOpenIpfsCacheDirectory,
                _txtResult);

            ExampleUtils.PopulateDropdown(_cmbIpfsHttpGateway, ExampleUtils.DefaultIpfsHttpGateway);

            _btnDownloadFile.onClick.AddListener(OnButtonClicked_DownloadFile);
            _btnOpenIpfsCacheDirectory.onClick.AddListener(OnButtonClicked_OpenIpfsCacheDirectory);

            SetDownloadedData(null);
        }

        private void OnButtonClicked_DownloadFile()
        {
            // Clear last result
            SetDownloadedData(null, null);

            IpfsAddress ipfsAddress = new(_txtCid.text?.Trim(), _txtPath.text?.Trim());

            if(!ValidateInputs(out string validationErrorMessage, _txtCid.text))
            {
                _txtResult.text = validationErrorMessage;
                return;
            }

            string writeToFilepath = buildDownloadFilepath(ipfsAddress);

            _btnDownloadFile.interactable = false;
            _txtResult.text = "Downloading ...";

            void OnDownloadFileResponse(bool success, string errorMessage, HttpResponse response)
            {
                _btnDownloadFile.interactable = true;
                _txtResult.text = $"Success: {success}";

                ExampleUtils.LogResult(success, errorMessage, response);
                if(!success)
                {
                    _txtResult.text += $"\nError Message: {errorMessage}";
                    return;
                }

                byte[] data = File.ReadAllBytes(writeToFilepath);
                SetDownloadedData(data, writeToFilepath);
            }

            // Download
            var gateway = ExampleUtils.GetSelectedValueFromDropdown<IpfsHttpGateway>(_cmbIpfsHttpGateway);
            IpfsFunctionLibrary.DownloadFile(
                Ipfs.GetIpfsHttpGatewayConfig(gateway),
                new IpfsAddress(_txtCid.text, _txtPath.text),
                writeToFilepath,
                _tgCreatePathIfMissing.isOn,
                _tgOverwriteExistingFile.isOn,
                OnDownloadFileResponse);
        }

        private bool ValidateInputs(out string validationErrorMessage, string cid)
        {
            validationErrorMessage = null;

            cid = cid?.Trim();
            if(string.IsNullOrEmpty(cid))
            {
                validationErrorMessage = "Error: CID cannot be empty";
                return false;
            }

            return true;
        }

        private void OnButtonClicked_OpenIpfsCacheDirectory()
        {
            string cacheDirectory = getIpfsCacheDirectoryFilepath();
            if(!Directory.Exists(cacheDirectory))
            {
                _txtResult.text = "Error: IPFS-Cache directory does not exist yet. Download a file first.";
                return;
            }

            Application.OpenURL(cacheDirectory);
        }

        private string getIpfsCacheDirectoryFilepath()
        {
            return Path.Combine(Application.temporaryCachePath, "IPFS-Cache");
        }

        private string buildDownloadFilepath(IpfsAddress ipfsAddress)
        {
            if(ipfsAddress == null || string.IsNullOrEmpty(ipfsAddress.Cid))
            {
                return null;
            }

            string filepath = getIpfsCacheDirectoryFilepath();
            filepath = Path.Combine(
                filepath,
                string.IsNullOrEmpty(ipfsAddress.Path)
                    ? ipfsAddress.Cid
                    : Path.GetFileName(ipfsAddress.Path));

            return filepath;
        }

        private void SetDownloadedData(byte[] data, string filepath = "")
        {
            if(data == null || data.Length == 0)
            {
                _txtDownloadedFile.text = "";
                return;
            }

            _txtDownloadedFile.text = $"File written to: {filepath}\n";
            _txtDownloadedFile.text += "\n";
            _txtDownloadedFile.text += $"File size (Bytes): {data.Length}\n";
            _txtDownloadedFile.text += "\n";
            _txtDownloadedFile.text += "Data (Hex):\n";
            _txtDownloadedFile.text += StringUtils.BytesToHex(data);
        }
    }
}