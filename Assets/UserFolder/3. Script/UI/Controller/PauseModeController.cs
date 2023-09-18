using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Controller
{
    public class PauseModeController : MonoBehaviour
    {
        [SerializeField] private UnityEvent PauseEvent;

        private bool m_IsActivePauseUI;
        private bool m_IsActiveSkillEventUI;

        public System.Action<bool> BlockRaycastAction { get; set; }

        public bool IsPause
        {
            get => IsActiveSettingUI || m_IsActiveSkillEventUI;
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

        public bool IsActiveSettingUI
        {
            get => m_IsActivePauseUI;
            set
            {
                m_IsActivePauseUI = value;
                if(m_IsActiveSkillEventUI) BlockRaycastAction?.Invoke(!value);
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
