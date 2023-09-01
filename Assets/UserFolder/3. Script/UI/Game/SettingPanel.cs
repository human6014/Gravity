using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class SettingPanel : MonoBehaviour
    {
        [SerializeField] private AudioClip m_ClickAudio;

        private CanvasGroup m_CanvasGroup;
        private Animator m_Animator;

        private void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
            m_Animator = GetComponent<Animator>();

            m_CanvasGroup.alpha = 0;
            m_CanvasGroup.interactable = false;
            m_CanvasGroup.blocksRaycasts = false;
        }

        public void TryActive(bool isActive)
        {
            m_CanvasGroup.interactable = isActive;
            m_CanvasGroup.blocksRaycasts = isActive;

            if (m_Animator == null) return;
            m_Animator.SetTrigger(isActive ? "Show" : "Hide");
        }
    }
}
