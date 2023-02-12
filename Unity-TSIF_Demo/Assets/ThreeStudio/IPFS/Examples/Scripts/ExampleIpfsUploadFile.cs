// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using System.IO;
using ThreeStudio.IPFS.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeStudio.IPFS.Examples
{
    public class ExampleIpfsUploadFile : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _cmbIpfsPinningService;
        [SerializeField] private TMP_InputField _txtUploadedFile;
        [SerializeField] private Button _btnUploadFile;
        [SerializeField] private Button _btnBrowseFile;
        [SerializeField] private TMP_InputField _txtBearerToken;
        [SerializeField] private TMP_InputField _txtFileToUpload;
        [SerializeField] private TMP_InputField _txtSaveAs;
        [SerializeField] private TMP_Text _txtResult;

        private void Awake()
        {
            Ipfs.SetDebugLogEnabled(true, Ipfs.DebugMode.UploadFileOrData);

            ExampleUtils.AssertAllRefsNotNull(
                _cmbIpfsPinningService,
                _txtUploadedFile,
                _btnUploadFile,
                _btnBrowseFile,
                _txtBearerToken,
                _txtFileToUpload,
                _txtSaveAs,
                _txtResult);

            ExampleUtils.PopulateDropdown(_cmbIpfsPinningService, ExampleUtils.DefaultIpfsPinningService);
            _cmbIpfsPinningService.onValueChanged.AddListener(OnValueChanged_PinningService);
            ReloadBearerToken();

            _btnUploadFile.onClick.AddListener(OnButtonClicked_UploadFile);
            _btnBrowseFile.onClick.AddListener(OnButtonClicked_BrowseFile);
            _txtBearerToken.onDeselect.AddListener(OnDeselect_BearerToken);

            SetUploadedFile(null);
        }

        private void OnValueChanged_PinningService(int arg0)
        {
            ReloadBearerToken();
        }

        private void ReloadBearerToken()
        {
            IpfsPinningService pinningService = ExampleUtils.GetSelectedValueFromDropdown<IpfsPinningService>(_cmbIpfsPinningService);
            _txtBearerToken.text = ExampleUtils.GetBearerTokenByPinningService(pinningService);
        }

        private void OnDeselect_BearerToken(string value)
        {
            IpfsPinningService pinningService = ExampleUtils.GetSelectedValueFromDropdown<IpfsPinningService>(_cmbIpfsPinningService);
            ExampleUtils.SetBearerTokenByPinningService(pinningService, value);
        }

        private void OnButtonClicked_UploadFile()
        {
            // Clear last result
            SetUploadedFile(null);

            if(!ValidateInputs(out string errorMessage, _txtFileToUpload.text))
            {
                _txtResult.text = errorMessage;
                return;
            }

            _btnUploadFile.interactable = false;
            _txtResult.text = "Uploading ...";

            IpfsPinningService pinningService = ExampleUtils.GetSelectedValueFromDropdown<IpfsPinningService>(_cmbIpfsPinningService);

            void OnUploadFileResponse(bool success, string error, HttpResponse response, string cid)
            {
                _btnUploadFile.interactable = true;
                _txtResult.text = $"Success: {success}";

                ExampleUtils.LogResult(success, error, response);
                if(!success)
                {
                    _txtResult.text += $"\nError Message: {error}";
                    return;
                }

                _txtUploadedFile.text = $"CID of uploaded file:\n{cid}";

                // if(_service == IpfsPinningService.Web3Storage)
                // {
                // IpfsFunctionLibrary.CalculateCidFromDataForWeb3(_path, OnCalculateCidResponse);
                // }
            }

            // Download
            IpfsFunctionLibrary.UploadFile(
                Ipfs.GetIpfsPinningServiceConfig(pinningService),
                _txtBearerToken.text,
                _txtFileToUpload.text,
                _txtSaveAs.text,
                OnUploadFileResponse);
        }

        private bool ValidateInputs(out string errorMessage, string fileToUpload)
        {
            errorMessage = null;

            if(string.IsNullOrEmpty(fileToUpload))
            {
                errorMessage = "Error: 'File to upload' cannot be empty.";
                return false;
            }

            if(!File.Exists(fileToUpload))
            {
                errorMessage = $"Error: Could not find file: {fileToUpload}";
                return false;
            }

            return true;
        }

        private void OnButtonClicked_BrowseFile()
        {
            #if UNITY_EDITOR
            string filepath = UnityEditor.EditorUtility.OpenFilePanel("Select a file to upload", UnityEngine.Device.Application.dataPath, "*.*");
            if(filepath != null)
            {
                _txtFileToUpload.text = filepath;
            }
            #else
            // Unity does not provide functions to open a file dialog in standalone builds.
            _txtResult.text = "Browse File button is not available outside of Unity Editor.";
            #endif
        }

        // private void OnCalculateCidResponse(bool success, string error, string result)
        // {
        //     Debug.Log("Calculated CID: " + result);
        // }

        private void SetUploadedFile(byte[] data)
        {
            if(data == null || data.Length == 0)
            {
                _txtUploadedFile.text = "";
                return;
            }

            _txtUploadedFile.text = $"Bytes sent: {data.Length}\n";
            _txtUploadedFile.text += "\n";
            _txtUploadedFile.text += "Data (Hex):\n";
            _txtUploadedFile.text += StringUtils.BytesToHex(data);
        }
    }
}