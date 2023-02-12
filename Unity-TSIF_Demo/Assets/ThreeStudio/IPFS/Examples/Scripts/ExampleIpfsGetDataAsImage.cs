// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ThreeStudio.IPFS.Examples
{
    public class ExampleIpfsGetDataAsImage : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _cmbIpfsHttpGateway;
        [SerializeField] private TMP_InputField _txtCid;
        [SerializeField] private TMP_InputField _txtPath;
        [SerializeField] private Image _txtDownloadedImage;
        [SerializeField] private Button _btnDownloadImage;
        [SerializeField] private TMP_Text _txtResult;

        private void Awake()
        {
            Ipfs.SetDebugLogEnabled(true, Ipfs.DebugMode.DownloadFileOrGetData);

            ExampleUtils.AssertAllRefsNotNull(
                _cmbIpfsHttpGateway,
                _txtCid,
                _txtPath,
                _txtDownloadedImage,
                _btnDownloadImage,
                _txtResult);

            ExampleUtils.PopulateDropdown(_cmbIpfsHttpGateway, ExampleUtils.DefaultIpfsHttpGateway);

            _btnDownloadImage.onClick.AddListener(OnButtonClicked_DownloadImage);

            SetDownloadedImage(null);
        }

        private void OnButtonClicked_DownloadImage()
        {
            // Clear last result
            SetDownloadedImage(null);

            _btnDownloadImage.interactable = false;
            _txtResult.text = "Downloading ...";

            // Download
            IpfsHttpGateway gateway = ExampleUtils.GetSelectedValueFromDropdown<IpfsHttpGateway>(_cmbIpfsHttpGateway);
            IpfsFunctionLibrary.GetDataAsImage(
                Ipfs.GetIpfsHttpGatewayConfig(gateway),
                new IpfsAddress(_txtCid.text, _txtPath.text),
                OnGetDataAsImageResponse);
        }

        private void OnGetDataAsImageResponse(bool success, string errorMessage, HttpResponse response, Texture2D texture)
        {
            _btnDownloadImage.interactable = true;
            _txtResult.text = $"Success: {success}";

            ExampleUtils.LogResult(success, errorMessage, response);
            if(!success)
            {
                _txtResult.text += $"\nError Message: {errorMessage}";
                return;
            }

            _txtResult.text += $"\nImage Dimensions: {texture.width} x {texture.height}";
            _txtResult.text += $"\nImage Pixel Data Format: {texture.format}";
            SetDownloadedImage(texture);
        }

        private void SetDownloadedImage(Texture2D texture)
        {
            _txtDownloadedImage.preserveAspect = true;
            _txtDownloadedImage.enabled = texture != null;
            _txtDownloadedImage.sprite = texture == null
                ? null
                : Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    Vector2.zero);
        }
    }
}