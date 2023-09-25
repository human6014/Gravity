using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UI.Manager;
using UnityEngine.Rendering;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private bool m_IsFixedFrameRate;
        [SerializeField] private int m_FrameRate = 60;
        [SerializeField] private Volume m_Volume;

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

            QualitySettings.vSyncCount = m_VisualSetting.m_VSyncCount ? 1 : 0;
            if (m_Volume.profile.TryGet(out m_MotionBlur)) m_MotionBlur.active = m_VisualSetting.MotionBlur;

            //FOV
            //DrawDistance


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
            QualitySettings.antiAliasing = m_VisualSetting.m_AntiAliasing;

            switch (m_VisualSetting.m_TextureQuality)
            {
                case 0:
                    break;
            }

            switch (m_VisualSetting.m_ShadowQuality)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }

            QualitySettings.anisotropicFiltering = m_VisualSetting.m_AnisotrpicFiltering ? 
                AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Disable;

            QualitySettings.softParticles = m_VisualSetting.m_SoftParticle;

            //해상도, Windowmode마지막에
        }
    }
}
