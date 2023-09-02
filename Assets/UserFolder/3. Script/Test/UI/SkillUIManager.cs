using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Manager
{
    public class SkillUIManager : MonoBehaviour
    {
        [SerializeField] private PanelController m_SkillPanel;

        private PauseModeController m_PauseModeController;

        private void Awake()
        {
            m_PauseModeController = transform.root.GetComponent<PauseModeController>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab)) 
                SetActiveUI(!m_PauseModeController.IsActiveNewSkillEventUI);
        }

        private void SetActiveUI(bool isActive)
        {
            m_PauseModeController.IsActiveNewSkillEventUI = isActive;
            m_SkillPanel.TryActive(isActive);
        }

        public void Resume()
        {
            SetActiveUI(false);
        }
    }
}
