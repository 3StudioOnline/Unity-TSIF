// Copyright 2023 3S Game Studio OU. All Rights Reserved.

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ThreeStudio.IPFS.Examples
{
    public class SwitchScene : MonoBehaviour
    {
        [SerializeField] private string _sceneToLoad;

        private Button _btn;

        private void Awake()
        {
            _btn = GetComponent<Button>();
            _btn.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            SceneManager.LoadScene(_sceneToLoad);
        }
    }
}