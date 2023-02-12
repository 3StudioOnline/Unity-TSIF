// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeStudio.IPFS.Examples
{
    public class ExampleIpfsGetDataAsString : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _cmbIpfsHttpGateway;
        [SerializeField] private TMP_InputField _txtCid;
        [SerializeField] private TMP_InputField _txtPath;
        [SerializeField] private TMP_InputField _txtDownloadedString;
        [SerializeField] private Button _btnDownloadString;
        [SerializeField] private TMP_Text _txtResult;

        private void Awake()
        {
            Ipfs.SetDebugLogEnabled(true, Ipfs.DebugMode.DownloadFileOrGetData);

            ExampleUtils.AssertAllRefsNotNull(
                _cmbIpfsHttpGateway,
                _txtCid,
                _txtPath,
                _txtDownloadedString,
                _btnDownloadString,
                _txtResult);

            ExampleUtils.PopulateDropdown(_cmbIpfsHttpGateway, ExampleUtils.DefaultIpfsHttpGateway);

            _btnDownloadString.onClick.AddListener(OnButtonClicked_DownloadString);

            SetDownloadedString(null);
        }

        private void OnButtonClicked_DownloadString()
        {
            // Clear last result
            SetDownloadedString(null);

            _btnDownloadString.interactable = false;
            _txtResult.text = "Downloading ...";

            // Download
            IpfsHttpGateway gateway = ExampleUtils.GetSelectedValueFromDropdown<IpfsHttpGateway>(_cmbIpfsHttpGateway);
            IpfsFunctionLibrary.GetDataAsString(
                Ipfs.GetIpfsHttpGatewayConfig(gateway),
                new IpfsAddress(_txtCid.text, _txtPath.text),
                OnGetDataAsStringResponse);
        }

        private void OnGetDataAsStringResponse(bool success, string errorMessage, HttpResponse response, string dataString)
        {
            _btnDownloadString.interactable = true;
            _txtResult.text = $"Success: {success}";

            ExampleUtils.LogResult(success, errorMessage, response);
            if(!success)
            {
                _txtResult.text += $"\nError Message: {errorMessage}";
                return;
            }

            SetDownloadedString(dataString);
        }


        private void SetDownloadedString(string text)
        {
            _txtDownloadedString.text = text;
        }
    }
}