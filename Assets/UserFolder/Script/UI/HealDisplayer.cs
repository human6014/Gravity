using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Player
{
    public class HealDisplayer : MonoBehaviour
    {
        [SerializeField] private Text m_RemainHeal;
        [SerializeField] private GameObject m_HealNotification;

        public void Init()
        {

        }

        public void UpdateRemainHeal(int value)
        {
            m_RemainHeal.text = value.ToString();
        }

        public void DisplayHealNotification(bool isActive)
        {
            m_HealNotification.SetActive(isActive);
        }
    }
}
