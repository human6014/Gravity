using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Controller
{
    public class PanelController : MonoBehaviour
    {
        [SerializeField] private AudioClip m_ClickAudio;

        private CanvasGroup m_CanvasGroup;
        private Animator m_Animator;

        private void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_Animator = GetComponent<Animator>();
        }

        public void TryActive(bool isActive)
        {
            m_CanvasGroup.interactable = isActive;
            m_CanvasGroup.blocksRaycasts = isActive;

            if (m_Animator == null) return;
            m_Animator.SetTrigger(isActive ? "Show" : "Hide");
        }

        public void BlockPanel(bool isActive)
        {
            m_CanvasGroup.interactable = isActive;
            m_CanvasGroup.blocksRaycasts = isActive;
        }
    }
}
