using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Controller
{
    public class SkillPanelController : MonoBehaviour
    {
        [SerializeField] private GameObject[] m_SkillPanels;
        [SerializeField] private Animator[] m_Animators;

        private int currentPanelNumber = 0;
        string panelFadeIn = "Panel In";
        string panelFadeOut = "Panel Out";
        string buttonFadeIn = "Hover to Pressed";
        string buttonFadeOut = "Pressed to Normal";

        private void Awake()
        {
            m_Animators[currentPanelNumber].Play(buttonFadeIn);
        }

        public void ChangePanel(int number)
        {
            if (currentPanelNumber == number) return;

            StopAllCoroutines();

            m_Animators[currentPanelNumber].Play(buttonFadeOut);
            m_Animators[number].Play(buttonFadeIn);
            //Buttons

            Animator currentAnimator = m_SkillPanels[currentPanelNumber].GetComponent<Animator>();
            Animator nextAnimator = m_SkillPanels[number].GetComponent<Animator>();

            m_SkillPanels[number].SetActive(true);

            currentAnimator.SetFloat("Anim Speed", 1);
            currentAnimator.CrossFade(panelFadeOut , 1);
            nextAnimator.SetFloat("Anim Speed", 1);
            nextAnimator.CrossFade(panelFadeIn, 1);

            StartCoroutine(DisablePreviousPanel(m_SkillPanels[currentPanelNumber]));
            
            currentPanelNumber = number;
        }

        private IEnumerator DisablePreviousPanel(GameObject panel)
        {
            yield return new WaitForSecondsRealtime(1);
            panel.SetActive(false);
        }
    }
}
