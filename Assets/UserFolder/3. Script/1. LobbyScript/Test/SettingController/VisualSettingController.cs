using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualSettingController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown m_TMP_Dropdown;
    private Resolution[] m_Resolutions;

    private void Awake()
    {
        m_Resolutions = Screen.resolutions;
        m_TMP_Dropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < m_Resolutions.Length; i++)
        {
            string option = m_Resolutions[i].width + "x" + m_Resolutions[i].height + " " + m_Resolutions[i].refreshRate + "hz";
            options.Add(option);

            if (m_Resolutions[i].width == Screen.currentResolution.width
                && m_Resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        m_TMP_Dropdown.AddOptions(options);
        m_TMP_Dropdown.onValueChanged.AddListener(ChangeResolution);
        m_TMP_Dropdown.value = currentResolutionIndex;
        m_TMP_Dropdown.RefreshShownValue();
    }

    public void ChangeResolution(int value)
    {
        //Screen.SetResolution(
        //    m_Resolutions[value].width, 
        //    m_Resolutions[value].height, 
        //    Screen.fullScreen,
        //    m_Resolutions[value].refreshRate);

        VisualSetting.m_ResolutionWidth = m_Resolutions[value].width;
        VisualSetting.m_ResolutionHeight = m_Resolutions[value].height;
        VisualSetting.m_RefreshRate = m_Resolutions[value].refreshRate;
    }

    public void ChangeWindowMode(int value)
    {
        VisualSetting.m_WindowMode = value;
    }

    public void ChangeVSync(int value)
    {
        VisualSetting.m_VSyncCount = value;
    }

    public void ChangeMotionBlur(bool value)
    {
        VisualSetting.m_MotionBlur = value;
    }

    public void ChangeFOV(float value)
    {
        VisualSetting.m_FOV = value;
    }

    public void ChangeDrawDistance(float value)
    {
        VisualSetting.m_FarDistance = value;
    }

    public void ChangeFrameRate(int value)
    {
        VisualSetting.m_FrameRate = value;
    }

    public void ChangeAntiAliasing(int value)
    {
        VisualSetting.m_AntiAliasing = value;
    }

    public void ChangeTextureQuality(int value)
    {
        VisualSetting.m_TextureQuality = value;
    }

    public void ChangeShadowQuality(int value)
    {
        VisualSetting.m_ShadowQuality = value;
    }

    public void ChangeAnisotropicFiltering(int value)
    {
        VisualSetting.m_AnisotrpicFiltering = value;
    }

    public void ChangeSoftParticle(bool value)
    {
        VisualSetting.m_SoftParticle = value;
    }
}
