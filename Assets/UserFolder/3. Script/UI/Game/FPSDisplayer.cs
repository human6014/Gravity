using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class FPSDisplayer : MonoBehaviour {

        private Text m_Text;
        private GamePlaySetting m_GamePlaySetting;

        private float m_Frame;
        private float m_TimeElapsed;
        private float m_FrameTime;

        private bool m_HasData;

        private void Awake() => m_Text = GetComponent<Text>();

        private void Start()
        {
            if (DataManager.Instance == null) m_HasData = false;
            else
            {
                m_HasData = true;
                m_GamePlaySetting = (GamePlaySetting)DataManager.Instance.Settings[0];
                ApplySetting();
            }
        }

        private void Update()
        {
            m_Frame++;
            m_TimeElapsed += Time.unscaledDeltaTime;
            if (m_TimeElapsed > 1)
            {
                m_FrameTime = m_TimeElapsed / m_Frame;
                m_TimeElapsed -= 1;
                UpdateText();
                m_Frame = 0;
            }
        }

        private void UpdateText() =>
            m_Text.text = string.Format("FPS : {0}, FrameTime : {1:F2} ms", m_Frame, m_FrameTime * 1000.0f);

        public void ApplySetting()
        {
            if (!m_HasData) return;

            gameObject.SetActive(m_GamePlaySetting.m_DisplayFrameRate == 1);
        }
    }
}
