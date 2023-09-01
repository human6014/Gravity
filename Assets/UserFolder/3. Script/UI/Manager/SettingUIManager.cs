using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Manager
{
    public class SettingUIManager : MonoBehaviour
    {
        [SerializeField] private Game.SettingPanel m_SettingPanel;
        [SerializeField] private UnityEvent PauseEvent;

        private bool m_IsActiveSkillEventUI;
        private bool m_IsActivePauseUI;

        private bool m_IsActiveNewSkillEventUI;
        public bool IsPause 
        { 
            get => IsActiveSkillEventUI || IsActivePauseUI; 
            private set => IsPause = value; 
        }

        public bool IsActiveSkillEventUI
        {
            get => m_IsActiveSkillEventUI;
            set
            {
                m_IsActiveSkillEventUI = value;
                PauseMode();
            }
        }

        public bool IsActiveNewSkillEventUI
        {
            get => m_IsActiveNewSkillEventUI;
            set
            {
                m_IsActiveNewSkillEventUI = value;
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
                PauseUIAction?.Invoke(value);
                PauseMode();
            }
        }

        public System.Action<bool> PauseUIAction { get; set; }

        private void Awake() => PauseMode();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) IsActivePauseUI = !IsActivePauseUI;
            if (Input.GetKeyDown(KeyCode.Tab)) IsActiveNewSkillEventUI = !IsActiveNewSkillEventUI;
        }

        private void PauseMode()
        {
            Cursor.visible = IsPause;
            Cursor.lockState = IsPause ? CursorLockMode.None : CursorLockMode.Locked;
            Time.timeScale = IsPause ? 0 : 1;
            if(!IsPause) PauseEvent?.Invoke();
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
