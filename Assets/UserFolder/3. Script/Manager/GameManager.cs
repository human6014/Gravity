using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UI.Manager;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Volume m_Volume;
        [SerializeField] private UniversalRenderPipelineAsset []m_UniversalRenderPipelineAsset;

        private VisualSetting m_VisualSetting;

        private bool m_HasData;

        public static bool IsGameEnd { get; private set; }

        private UnityEngine.Rendering.Universal.MotionBlur m_MotionBlur;
        
        public static void GameClear()
        {
            if (IsGameEnd) return;
            IsGameEnd = true;
            Debug.Log("GameClear");
        }

        public static void GameEnd()
        {
            if (IsGameEnd) return;
            IsGameEnd = true;
            Debug.Log("GameEnd");
        }

        private void Awake()
        {
            IsGameEnd = false;

            if (DataManager.Instance == null) m_HasData = false;
            else
            {
                m_HasData = true;
                m_VisualSetting = (VisualSetting)DataManager.Instance.Settings[3];
                ApplySetting();
            }
        }

        public void ApplySetting()
        {
            if (!m_HasData) return;

            QualitySettings.vSyncCount = m_VisualSetting.m_VSyncCount;
            if (m_Volume.profile.TryGet(out m_MotionBlur)) m_MotionBlur.active = m_VisualSetting.m_MotionBlur == 1;

            Application.targetFrameRate = m_VisualSetting.m_FrameRate;
            switch (m_VisualSetting.m_FrameRate)
            {
                case 0:
                    Application.targetFrameRate = 30;
                    break;
                case 1:
                    Application.targetFrameRate = 60;
                    break;
                case 2:
                    Application.targetFrameRate = 120;
                    break;
            }

            switch (m_VisualSetting.m_AntiAliasing)
            {
                case 0:
                    QualitySettings.antiAliasing = 0;
                    break;
                case 1:
                    QualitySettings.antiAliasing = 2;
                    break;
                case 2:
                    QualitySettings.antiAliasing = 4;
                    break;
                case 3:
                    QualitySettings.antiAliasing = 8;
                    break;
            }

            switch (m_VisualSetting.m_TextureQuality)
            {
                case 0:
                    QualitySettings.masterTextureLimit = 2;
                    break;
                case 1:
                    QualitySettings.masterTextureLimit = 1;
                    break;
                case 2:
                    QualitySettings.masterTextureLimit = 0;
                    break;
            }

            switch (m_VisualSetting.m_ShadowQuality)
            {
                case 0:
                    QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;
                    QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Low;
                    break;
                case 1:
                    QualitySettings.shadows = UnityEngine.ShadowQuality.HardOnly;
                    QualitySettings.shadowResolution = UnityEngine.ShadowResolution.Medium;
                    break;
                case 2:
                    QualitySettings.shadows = UnityEngine.ShadowQuality.All;
                    QualitySettings.shadowResolution = UnityEngine.ShadowResolution.High;
                    break;
                case 3:
                    QualitySettings.shadows = UnityEngine.ShadowQuality.All;
                    QualitySettings.shadowResolution = UnityEngine.ShadowResolution.VeryHigh;
                    break;
            }
            QualitySettings.renderPipeline = m_UniversalRenderPipelineAsset[m_VisualSetting.m_ShadowQuality];

            switch (m_VisualSetting.m_AnisotrpicFiltering)
            {
                case 0:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                    break;
                case 1:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                    break;
            }

            QualitySettings.softParticles = m_VisualSetting.m_SoftParticle == 1;
        }
    }
}
