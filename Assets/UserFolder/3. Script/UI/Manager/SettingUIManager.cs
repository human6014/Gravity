using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UI.Controller;
using UnityEngine.SceneManagement;
using Michsky.UI.Dark;

namespace UI.Manager
{
    public class SettingUIManager : MonoBehaviour
    {
        [SerializeField] private PanelController m_PanelController;

        [SerializeField] private GameObject m_SettingPanel;

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
            m_PanelController.TryActive(isActive);
        }

        #region UnityEvent
        public void Resume()
        {
            SetActiveUI(false);
        }
        
        public void Setting()
        {
            Debug.Log("Click Setting");

        }

        public void ReturnLobby()
        {
            SceneManager.LoadScene("LobbyScene");
        }
        #endregion
    }
}
