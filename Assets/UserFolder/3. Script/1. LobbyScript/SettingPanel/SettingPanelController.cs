using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPanelController : MonoBehaviour
{
    [SerializeField] private GameObject[] ControllingPanel;

    private int m_CurrentActiveIndex;

    public void ChangeSettingPanel(int panelIndex)
    {
        ControllingPanel[m_CurrentActiveIndex].SetActive(false);
        ControllingPanel[panelIndex].SetActive(true);
        m_CurrentActiveIndex = panelIndex;
    }
}
