using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Controller
{
    public class PauseModeController : MonoBehaviour
    {
        [SerializeField] private UnityEvent PauseEvent;

        private bool m_IsActiveSkillEventUI;
        private bool m_IsActivePauseUI;
        private bool m_IsActiveNewSkillEventUI;

        public bool IsPause
        {
            get => IsActiveSkillEventUI || IsActiveSettingUI || m_IsActiveNewSkillEventUI;
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

        public bool IsActiveSettingUI
        {
            get => m_IsActivePauseUI;
            set
            {
                m_IsActivePauseUI = value;
                PauseMode();
            }
        }

        private void Awake() => PauseMode();

        private void PauseMode()
        {
            Cursor.visible = IsPause;
            Cursor.lockState = IsPause ? CursorLockMode.None : CursorLockMode.Locked;
            Time.timeScale = IsPause ? 0 : 1;
            if (!IsPause) PauseEvent?.Invoke();
        }
    }
}
