using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualSettingController : SettingController
{
    [SerializeField] private TMP_Dropdown m_TMP_Dropdown;

    private VisualSetting m_VisualSetting;

    private Resolution[] m_Resolutions;
    private int m_CurrentResolutionIndex;
    private void Awake()
    {
        m_Resolutions = Screen.resolutions;
        m_TMP_Dropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < m_Resolutions.Length; i++)
        {
            string option = m_Resolutions[i].width + "x" + m_Resolutions[i].height + " " + m_Resolutions[i].refreshRate + "hz";
            options.Add(option);

            if (m_Resolutions[i].width == Screen.currentResolution.width
                && m_Resolutions[i].height == Screen.currentResolution.height)
                m_CurrentResolutionIndex = i;
        }
        m_TMP_Dropdown.AddOptions(options);
        m_TMP_Dropdown.value = m_CurrentResolutionIndex;
        m_TMP_Dropdown.RefreshShownValue();
    }

    private void Start()
    {
        if (DataManager.Instance == null) return;
        m_VisualSetting = (VisualSetting)DataManager.Instance.Settings[3];
        if((int)m_VisualSetting[0] == -1) m_VisualSetting[0] = m_CurrentResolutionIndex;
        m_TMP_Dropdown.onValueChanged.AddListener(ChangeResolution);
        UpdateSettings();
    }

    public void ChangeResolution(int value)
    {
        //Screen.SetResolution(
        //    m_Resolutions[value].width, 
        //    m_Resolutions[value].height, 
        //    Screen.fullScreen,
        //    m_Resolutions[value].refreshRate);
        m_VisualSetting[0] = value;
    }

    public void ChangeWindowMode(int value)
    {
        m_VisualSetting[1] = value;
    }

    public void ChangeVSync(bool value)
    {
        m_VisualSetting[2] = value;
    }

    public void ChangeMotionBlur(bool value)
    {
        m_VisualSetting[3] = value;
    }

    public void ChangeFOV(float value)
    {
        m_VisualSetting[4] = value;
    }

    public void ChangeDrawDistance(float value)
    {
        m_VisualSetting[5] = value;
    }

    public void ChangeFrameRate(int value)
    {
        m_VisualSetting[6] = value;
    }

    public void ChangeAntiAliasing(int value)
    {
        m_VisualSetting[7] = value;
    }

    public void ChangeTextureQuality(int value)
    {
        m_VisualSetting[8] = value;
    }

    public void ChangeShadowQuality(int value)
    {
        m_VisualSetting[9] = value;
    }

    public void ChangeAnisotropicFiltering(bool value)
    {
        m_VisualSetting[10] = value;
    }

    public void ChangeSoftParticle(bool value)
    {
        m_VisualSetting[11] = value;
    }

    public override void UpdateSettings()
    {
        for (int i = 0; i < m_LoadableSettingComponents.Length; i++)
            m_LoadableSettingComponents[i].LoadComponent(m_VisualSetting[i]);
    }
}
