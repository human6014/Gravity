using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] m_UIGroup;

    private GamePlaySetting m_GamePlaySetting;

    private bool m_HasData;

    private void Awake()
    {
        if (DataManager.Instance == null) m_HasData = false;
        else
        {
            m_HasData = true;
            m_GamePlaySetting = (GamePlaySetting)DataManager.Instance.Settings[0];
            ApplySetting();
        }
    }

    public void ApplySetting()
    {
        if (!m_HasData) return;

        bool enable = m_GamePlaySetting.m_EnableHUD;

        foreach (CanvasGroup cg in m_UIGroup)
        {
            cg.alpha = enable ? 1 : 0;
            cg.interactable = enable;
            cg.blocksRaycasts = enable;
        }
    }
}
