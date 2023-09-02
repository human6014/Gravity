using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPanelController : MonoBehaviour
{
    [SerializeField] private GameObject[] m_SkillPanels;

    private int currentPanelNumber = 0;

    public void ChangePanel(int number)
    {
        m_SkillPanels[currentPanelNumber].SetActive(false);
        m_SkillPanels[number].SetActive(true);
        currentPanelNumber = number;
    }
}
