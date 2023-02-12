// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using UnityEngine;
using UnityEngine.UI;

namespace ThreeStudio.IPFS.Examples
{
    public class QuitApp : MonoBehaviour
    {
        private Button _btn;

        private void Awake()
        {
            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            Application.Quit(0);
        }
    }
}