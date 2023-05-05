using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI.Manager
{
    public class SettingUIManager : MonoBehaviour
    {
        [SerializeField] private Game.SettingPanel m_SettingPanel;

        public bool IsActiveSettingUI { get; private set; }

        private void Awake() => PauseMode(false);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DoChangePanel();
            }
        }

        private void DoChangePanel()
        {
            IsActiveSettingUI = !IsActiveSettingUI;
            m_SettingPanel.TryActive(IsActiveSettingUI);
            PauseMode(IsActiveSettingUI);
        }

        private void PauseMode(bool isActive)
        {
            Cursor.visible = isActive;
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
            Time.timeScale = isActive ? 0 : 1;
        }

        public void Resume()
        {
            DoChangePanel();
        }

        public void ReturnLobby()
        {
            Debug.Log("ReturnLobby");
        }
    }
}
