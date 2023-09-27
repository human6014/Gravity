using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualSetting : Setting
{
    public int m_ResolutionIndex { get; set; }
    public int m_WindowMode { get; set; }
    public int m_VSyncCount { get; set; }

    public int m_MotionBlur { get; set; }
    public float m_FOV { get; set; }
    public float m_FarDistance { get; set; }

    public int m_FrameRate { get; set; }
    public int m_AntiAliasing { get; set; }
    public int m_TextureQuality { get; set; }
    public int m_ShadowQuality { get; set; }
    public int m_AnisotrpicFiltering { get; set; }
    public int m_SoftParticle { get; set; }

    public object this [int index]
    {
        get
        {
            switch (index)
            {
                case 0: return m_ResolutionIndex;
                case 1: return m_WindowMode;
                case 2: return m_VSyncCount;

                case 3: return m_MotionBlur;
                case 4: return m_FOV;
                case 5: return m_FarDistance;

                case 6: return m_FrameRate;
                case 7: return m_AntiAliasing;
                case 8: return m_TextureQuality;
                case 9: return m_ShadowQuality;
                case 10: return m_AnisotrpicFiltering;
                case 11: return m_SoftParticle;
                default:Debug.Log("Indexer name is null"); return null;
            }
        }
        set
        {
            switch (index)
            {
                case 0: m_ResolutionIndex = (int)value; break;
                case 1: m_WindowMode = (int)value; break;
                case 2: m_VSyncCount = (int)value; break;

                case 3: m_MotionBlur = (int)value; break;
                case 4: m_FOV = (float)value; break;
                case 5: m_FarDistance = (float)value; break;

                case 6: m_FrameRate = (int)value; break;
                case 7: m_AntiAliasing = (int)value; break;
                case 8: m_TextureQuality = (int)value; break;
                case 9: m_ShadowQuality = (int)value; break;
                case 10: m_AnisotrpicFiltering = (int)value; break;
                case 11: m_SoftParticle = (int)value; break;
                default: Debug.Log("Indexer name is null"); break;
            }
        }
    }

    public override void LoadDefault()
    {
        Debug.Log("Load Default VisualSetting");

        m_ResolutionIndex = -1;
        m_WindowMode = 1;
        m_VSyncCount = 1;    //1

        m_MotionBlur = 1;    //1
        m_FOV = 60;
        m_FarDistance = 140;

        m_FrameRate = 2;
        m_AntiAliasing = 0;
        m_TextureQuality = 2;
        m_ShadowQuality = 3;
        m_AnisotrpicFiltering = 1;   //1
        m_SoftParticle = 1;          //1
    }

    public override void LoadData()
    {
        Debug.Log("Load VisualSetting");

        m_ResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
        m_WindowMode = PlayerPrefs.GetInt("WindowMode");
        m_VSyncCount = PlayerPrefs.GetInt("VSyncCount");

        m_MotionBlur = PlayerPrefs.GetInt("MotionBlur");
        m_FOV = PlayerPrefs.GetFloat("FOV");
        m_FarDistance = PlayerPrefs.GetFloat("FarDistance");

        m_FrameRate = PlayerPrefs.GetInt("FrameRate");
        m_AntiAliasing = PlayerPrefs.GetInt("AntiAliasing");
        m_TextureQuality = PlayerPrefs.GetInt("TextureQuality");
        m_ShadowQuality = PlayerPrefs.GetInt("ShadowQuality");
        m_AnisotrpicFiltering = PlayerPrefs.GetInt("AnisotrpicFiltering");
        m_SoftParticle = PlayerPrefs.GetInt("SoftParticle");

        FullScreenMode fullScreenMode = (FullScreenMode)m_WindowMode;
        if (fullScreenMode == FullScreenMode.MaximizedWindow) fullScreenMode = FullScreenMode.Windowed;
        Screen.fullScreenMode = fullScreenMode;

        Screen.SetResolution(
                Screen.resolutions[m_ResolutionIndex].width,
                Screen.resolutions[m_ResolutionIndex].height,
                fullScreenMode,
                Screen.resolutions[m_ResolutionIndex].refreshRate);
    }

    public override void SaveData()
    {
        Debug.Log("Save VisualSetting");

        PlayerPrefs.SetInt("ResolutionIndex", m_ResolutionIndex);
        PlayerPrefs.SetInt("WindowMode", m_WindowMode);
        PlayerPrefs.SetInt("VSyncCount", m_VSyncCount);

        PlayerPrefs.SetInt("MotionBlur", m_MotionBlur);
        PlayerPrefs.SetFloat("FOV", m_FOV);
        PlayerPrefs.SetFloat("FarDistance", m_FarDistance);

        PlayerPrefs.SetInt("FrameRate", m_FrameRate);
        PlayerPrefs.SetInt("AntiAliasing", m_AntiAliasing);
        PlayerPrefs.SetInt("TextureQuality", m_TextureQuality);
        PlayerPrefs.SetInt("ShadowQuality", m_ShadowQuality);
        PlayerPrefs.SetInt("AnisotrpicFiltering", m_AnisotrpicFiltering);
        PlayerPrefs.SetInt("SoftParticle", m_SoftParticle);

        PlayerPrefs.Save();
    }

    public void DebugAllSetting()
    {
        Debug.Log("m_ResolutionIndex : " + m_ResolutionIndex);
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
