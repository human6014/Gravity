using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.Controller;

namespace UI.Manager
{
    public class SkillUIManager : MonoBehaviour
    {
        [SerializeField] private PanelController m_SkillPanel;

        private PauseModeController m_PauseModeController;

        private void Awake()
        {
            m_PauseModeController = transform.root.GetComponent<PauseModeController>();
            m_PauseModeController.BlockRaycastAction += (bool isActive) => m_SkillPanel.BlockPanel(isActive);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab) && !m_PauseModeController.IsActiveSettingUI) 
                SetActiveUI(!m_PauseModeController.IsActiveSkillEventUI);
        }

        private void SetActiveUI(bool isActive)
        {
            m_PauseModeController.IsActiveSkillEventUI = isActive;
            m_SkillPanel.TryActive(isActive);
        }

        public void Resume()
        {
            SetActiveUI(false);
        }
    }
}
