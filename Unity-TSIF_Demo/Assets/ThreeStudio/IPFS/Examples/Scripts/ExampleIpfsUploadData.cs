// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using ThreeStudio.IPFS.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeStudio.IPFS.Examples
{
    public class ExampleIpfsUploadData : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _cmbIpfsPinningService;
        [SerializeField] private TMP_InputField _txtUploadedData;
        [SerializeField] private Button _btnUploadData;
        [SerializeField] private TMP_InputField _txtBearerToken;
        [SerializeField] private TMP_InputField _txtDataToUpload;
        [SerializeField] private TMP_InputField _txtSaveAs;
        [SerializeField] private TMP_Text _txtResult;

        private void Awake()
        {
            Ipfs.SetDebugLogEnabled(true, Ipfs.DebugMode.UploadFileOrData);

            ExampleUtils.AssertAllRefsNotNull(
                _cmbIpfsPinningService,
                _txtUploadedData,
                _btnUploadData,
                _txtBearerToken,
                _txtDataToUpload,
                _txtSaveAs,
                _txtResult);

            ExampleUtils.PopulateDropdown(_cmbIpfsPinningService, ExampleUtils.DefaultIpfsPinningService);
            _cmbIpfsPinningService.onValueChanged.AddListener(OnValueChanged_PinningService);
            ReloadBearerToken();

            _btnUploadData.onClick.AddListener(OnButtonClicked_UploadData);
            _txtBearerToken.onDeselect.AddListener(OnDeselect_BearerToken);

            SetUploadedData(null);
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

        private void OnButtonClicked_UploadData()
        {
            // Clear last result
            SetUploadedData(null);

            _btnUploadData.interactable = false;
            _txtResult.text = "Uploading ...";

            IpfsPinningService pinningService = ExampleUtils.GetSelectedValueFromDropdown<IpfsPinningService>(_cmbIpfsPinningService);
            byte[] data = StringUtils.StringToBytes(_txtDataToUpload.text);

            void OnUploadDataResponse(bool success, string error, HttpResponse response, string cid)
            {
                _btnUploadData.interactable = true;
                _txtResult.text = $"Success: {success}";

                ExampleUtils.LogResult(success, error, response);
                if(!success)
                {
                    _txtResult.text += $"\nError Message: {error}";
                    return;
                }

                _txtUploadedData.text = $"CID of uploaded file:\n{cid}";
            }

            // Download
            IpfsFunctionLibrary.UploadData(
                Ipfs.GetIpfsPinningServiceConfig(pinningService),
                _txtBearerToken.text,
                data,
                _txtSaveAs.text,
                OnUploadDataResponse);
        }

        private void SetUploadedData(byte[] data)
        {
            if(data == null || data.Length == 0)
            {
                _txtUploadedData.text = "";
                return;
            }

            _txtUploadedData.text = $"Bytes sent: {data.Length}\n";
            _txtUploadedData.text += "\n";
            _txtUploadedData.text += "Data (Hex):\n";
            _txtUploadedData.text += StringUtils.BytesToHex(data);
        }
    }
}