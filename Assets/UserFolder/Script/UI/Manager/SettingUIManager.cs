using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI.Manager
{
    public class SettingUIManager : MonoBehaviour
    {
        [SerializeField] private Game.SettingPanel m_SettingPanel;

        bool m_IsPause;
        bool m_IsActivePauseUI;
        public bool IsPause 
        {
            get =>  m_IsPause;
            set
            {
                m_IsPause = value;
                PauseMode(value);
            }
        }

        public bool IsActivePauseUI 
        {
            get => m_IsActivePauseUI;
            set
            {
                m_IsActivePauseUI = value;
                m_SettingPanel.TryActive(value);
                IsPause = value;      
            }
        }
        //버그 있음

        private void Awake() => PauseMode(false);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) IsActivePauseUI = !IsActivePauseUI;
        }

        private void DoChangePanel()
        {
            IsActivePauseUI = !IsActivePauseUI;
            m_SettingPanel.TryActive(IsActivePauseUI);
            PauseMode(IsActivePauseUI);
        }

        public void PauseMode(bool isActive)
        {
            Cursor.visible = isActive;
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
            Time.timeScale = isActive ? 0 : 1;
        }

        public void Resume()
        {
            //DoChangePanel();
        }

        public void ReturnLobby()
        {
            Debug.Log("ReturnLobby");
        }
    }
}
