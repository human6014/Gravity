using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainButtonController : MonoBehaviour
{
    [SerializeField] private GameObject m_SettingPanelObject;

    private bool isActiveSettingPanel;

    public void SettingsButtonClicked()
    {
        isActiveSettingPanel = !isActiveSettingPanel;
        m_SettingPanelObject.SetActive(isActiveSettingPanel);
    }

    public void ExitButtonClicked()
    {
        Application.Quit();
    }
}
