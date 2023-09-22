using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualSetting : Setting
{
    public static int m_ResolutionWidth { get; set; }
    public static int m_ResolutionHeight { get; set; }
    public static int m_RefreshRate { get; set; }
    public static int m_WindowMode { get; set; }
    public static int m_VSyncCount { get; set; }

    public static bool m_MotionBlur { get; set; }
    public static float m_FOV { get; set; }
    public static float m_FarDistance { get; set; }

    public static int m_FrameRate { get; set; }
    public static int m_AntiAliasing { get; set; }
    public static int m_TextureQuality { get; set; }
    public static int m_ShadowQuality { get; set; }
    public static int m_AnisotrpicFiltering { get; set; }
    public static bool m_SoftParticle { get; set; }

    #region General
    public int WindowMode
    {
        get => m_WindowMode;
        set
        {
            m_WindowMode = value;
            if (value == 0)
            {
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            }
            else if (value == 1)
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
            else
            {
                Screen.fullScreen = false;
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
        }
    }

    public int VSyncCount
    {
        get => m_VSyncCount;
        set
        {
            m_VSyncCount = value;
            QualitySettings.vSyncCount = value;
        }
    }
    #endregion

    #region Rendering
    public bool MotionBlur
    {
        get => m_MotionBlur;
        set => m_MotionBlur = value;
    }

    public float FOV
    {
        get => m_FOV;
        set => m_FOV = value;
    }

    public float FarDistance
    {
        get => m_FarDistance;
        set => m_FarDistance = value;
    }
    #endregion

    #region Quality
    public int FrameRate
    {
        get => m_FrameRate;
        set => m_FrameRate = value;
    }

    public int AntiAliasing
    {
        get => m_AntiAliasing;
        set
        {
            m_AntiAliasing = value;
            QualitySettings.antiAliasing = value;
        }
    }

    public int TextureQuality
    {
        get => m_TextureQuality;
        set
        {
            m_TextureQuality = value;
            QualitySettings.masterTextureLimit = value;
        }
    }

    public int ShadowQuality
    {
        get => m_ShadowQuality;
        set
        {
            m_ShadowQuality = value;
            if (value == 3) QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
            else if (value == 2) QualitySettings.shadowResolution = ShadowResolution.High;
            else if (value == 1) QualitySettings.shadowResolution = ShadowResolution.Medium;
            else if (value == 0) QualitySettings.shadowResolution = ShadowResolution.Low;
            else QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;
        }
    }

    public int AnisotrpicFiltering
    {
        get => m_AnisotrpicFiltering;
        set
        {
            m_AnisotrpicFiltering = value;
            if (value == 0) QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            else QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        }
    }

    public bool SoftParticle
    {
        get => m_SoftParticle;
        set
        {
            m_SoftParticle = value;
            QualitySettings.softParticles = value;
        }
    }
    #endregion

    public override void LoadDefault()
    {
        m_MotionBlur = true;
    }

    public override void LoadData()
    {
        m_ResolutionWidth= PlayerPrefs.GetInt("ResolutionWidth");
        m_ResolutionHeight = PlayerPrefs.GetInt("ResolutionHeight");
        m_WindowMode = PlayerPrefs.GetInt("WindowMode");
        m_VSyncCount = PlayerPrefs.GetInt("VSyncCount");

        m_MotionBlur = PlayerPrefs.GetInt("MotionBlur") == 1;
        m_FOV = PlayerPrefs.GetInt("FOV");
        m_FarDistance = PlayerPrefs.GetInt("FarDistance");

        m_FrameRate = PlayerPrefs.GetInt("FrameRate");
        m_AntiAliasing = PlayerPrefs.GetInt("AntiAliasing");
        m_TextureQuality = PlayerPrefs.GetInt("TextureQuality");
        m_ShadowQuality = PlayerPrefs.GetInt("ShadowQuality");
        m_AnisotrpicFiltering = PlayerPrefs.GetInt("AnisotrpicFiltering");
        m_SoftParticle = PlayerPrefs.GetInt("SoftParticle") == 1;
    }

    public override void SaveData()
    {
        PlayerPrefs.SetInt("ResolutionWidth", m_ResolutionWidth);
        PlayerPrefs.SetInt("ResolutionHeight", m_ResolutionHeight);
        PlayerPrefs.SetInt("WindowMode", m_WindowMode);
        PlayerPrefs.SetInt("VSyncCount", m_VSyncCount);

        PlayerPrefs.SetInt("MotionBlur", m_MotionBlur ? 1 : 0);
        PlayerPrefs.SetFloat("FOV", m_FOV);
        PlayerPrefs.SetFloat("FarDistance", m_FarDistance);

        PlayerPrefs.SetInt("FrameRate", m_FrameRate);
        PlayerPrefs.SetInt("AntiAliasing", m_AntiAliasing);
        PlayerPrefs.SetInt("TextureQuality", m_TextureQuality);
        PlayerPrefs.SetInt("ShadowQuality", m_ShadowQuality);
        PlayerPrefs.SetInt("AnisotrpicFiltering", m_AnisotrpicFiltering);
        PlayerPrefs.SetInt("SoftParticle", m_SoftParticle ? 1 : 0);
    }

    public void DebugAllSetting()
    {
        Debug.Log("m_ResolutionWidth : " + m_ResolutionWidth);
        Debug.Log("m_ResolutionHeight : " + m_ResolutionHeight);
        Debug.Log("m_WindowMode : " + m_WindowMode);
        Debug.Log("m_VSyncCount : " + m_VSyncCount);

        Debug.Log("m_MotionBlur : " + m_MotionBlur);
        Debug.Log("m_FOV : " + m_FOV);
        Debug.Log("m_FarDistance : " + m_FarDistance);

        Debug.Log("m_FrameRate : " + m_FrameRate);
        Debug.Log("m_AntiAliasing : " + m_AntiAliasing);
        Debug.Log("m_TextureQuality : " + m_TextureQuality);
        Debug.Log("m_ShadowQuality : " + m_ShadowQuality);
        Debug.Log("m_AnisotrpicFiltering : " + m_AnisotrpicFiltering);
        Debug.Log("m_SoftParticle : " + m_SoftParticle);
    }
}
