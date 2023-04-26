using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI.Manager
{
    public class SettingUIManager : MonoBehaviour
    {
        [SerializeField] private Game.SettingPanel m_SettingPanel;

        public bool IsActiveSettingUI { get; private set; }

        private void Awake() => MouseModeSetting(false);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                IsActiveSettingUI = !IsActiveSettingUI;
                m_SettingPanel.TryActive(IsActiveSettingUI);
                MouseModeSetting(IsActiveSettingUI);
            }
        }

        private void MouseModeSetting(bool isActiveSettingUI)
        {
            Cursor.visible = isActiveSettingUI;
            Cursor.lockState = isActiveSettingUI ? CursorLockMode.None : CursorLockMode.Locked;
            Time.timeScale = isActiveSettingUI ? 0 : 1;
        }

        public void Resume()
        {
            Debug.Log("Resume");
        }

        public void ReturnLobby()
        {
            Debug.Log("ReturnLobby");
        }
    }
}
