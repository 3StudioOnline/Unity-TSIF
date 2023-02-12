// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using ThreeStudio.IPFS.Internal;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeStudio.IPFS.Examples
{
    public class ExampleIpfsGetData : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _cmbIpfsHttpGateway;
        [SerializeField] private TMP_InputField _txtCid;
        [SerializeField] private TMP_InputField _txtPath;
        [SerializeField] private TMP_InputField _txtDownloadedData;
        [SerializeField] private Button _btnDownloadData;
        [SerializeField] private TMP_Text _txtResult;

        private void Awake()
        {
            Ipfs.SetDebugLogEnabled(true, Ipfs.DebugMode.DownloadFileOrGetData);

            ExampleUtils.AssertAllRefsNotNull(
                _cmbIpfsHttpGateway,
                _txtCid,
                _txtPath,
                _txtDownloadedData,
                _btnDownloadData,
                _txtResult);

            ExampleUtils.PopulateDropdown(_cmbIpfsHttpGateway, ExampleUtils.DefaultIpfsHttpGateway);

            _btnDownloadData.onClick.AddListener(OnButtonClicked_DownloadData);

            SetDownloadedData(null);
        }

        private void OnButtonClicked_DownloadData()
        {
            // Clear last result
            SetDownloadedData(null);

            _btnDownloadData.interactable = false;
            _txtResult.text = "Downloading ...";

            // Download
            IpfsHttpGateway gateway = ExampleUtils.GetSelectedValueFromDropdown<IpfsHttpGateway>(_cmbIpfsHttpGateway);
            IpfsFunctionLibrary.GetData(
                Ipfs.GetIpfsHttpGatewayConfig(gateway),
                new IpfsAddress(_txtCid.text, _txtPath.text),
                OnGetDataResponse);
        }

        private void OnGetDataResponse(bool success, string errorMessage, HttpResponse response, byte[] dataString)
        {
            _btnDownloadData.interactable = true;
            _txtResult.text = $"Success: {success}";

            ExampleUtils.LogResult(success, errorMessage, response);
            if(!success)
            {
                _txtResult.text += $"\nError Message: {errorMessage}";
                return;
            }

            SetDownloadedData(dataString);
        }


        private void SetDownloadedData(byte[] data)
        {
            if(data == null || data.Length == 0)
            {
                _txtDownloadedData.text = "";
                return;
            }

            _txtDownloadedData.text = $"Bytes received: {data.Length}\n";
            _txtDownloadedData.text += "\n";
            _txtDownloadedData.text += "Data (Hex):\n";
            _txtDownloadedData.text += StringUtils.BytesToHex(data);
        }
    }
}