using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI.Manager
{
    public class SettingUIManager : MonoBehaviour
    {
        [SerializeField] private Game.SettingPanel m_SettingPanel;

        private bool m_IsActiveSkillEventUI;
        private bool m_IsActivePauseUI;

        public bool IsPause { get => IsActiveSkillEventUI || IsActivePauseUI; private set => IsPause = value; }

        public bool IsActiveSkillEventUI
        {
            get => m_IsActiveSkillEventUI;
            set
            {
                m_IsActiveSkillEventUI = value;
                PauseMode();
            }
        }

        public bool IsActivePauseUI 
        {
            get => m_IsActivePauseUI;
            set
            {
                m_IsActivePauseUI = value;
                m_SettingPanel.TryActive(value);
                PauseMode();
            }
        }

        private void Awake() => PauseMode();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) IsActivePauseUI = !IsActivePauseUI;
        }

        private void PauseMode()
        {
            Cursor.visible = IsPause;
            Cursor.lockState = IsPause ? CursorLockMode.None : CursorLockMode.Locked;
            Time.timeScale = IsPause ? 0 : 1;
        }

        #region UnityEvent
        public void Resume() => IsActivePauseUI = false;
        
        public void Setting()
        {
            //연결 안함 아직
        }

        public void ReturnLobby()
        {
            Debug.Log("ReturnLobby");
        }
        #endregion
    }
}
