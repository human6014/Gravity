using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UI.Controller;

namespace UI.Manager
{
    public class SettingUIManager : MonoBehaviour
    {
        [SerializeField] private PanelController m_SettingPanel;

        private PauseModeController m_PauseModeController;

        private void Awake()
        {
            m_PauseModeController = transform.root.GetComponent<PauseModeController>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SetActiveUI(!m_PauseModeController.IsActiveSettingUI);
        }

        private void SetActiveUI(bool isActive)
        {
            m_PauseModeController.IsActiveSettingUI = isActive;
            m_SettingPanel.TryActive(isActive);
        }

        #region UnityEvent
        public void Resume()
        {
            SetActiveUI(false);
        }
        
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
