using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Event
{
    public abstract class SkillUp : MonoBehaviour
    {
        [SerializeField] private Text m_UpgradeText;
        [SerializeField] private Text m_PointCostText;

        [SerializeField] private int m_SkillPointCost = 5;
        [SerializeField] private int m_SkillPointIncreseRate = 0;
        [SerializeField] private int m_MaxLevel = 10;

        private PlayerData m_PlayerData;
        private int m_CurrentLevel;
        protected PlayerSkillReceiver PlayerSkillReceiver { get; private set; }


        private void Awake()
        {
            m_PlayerData = FindObjectOfType<PlayerData>();
            PlayerSkillReceiver = FindObjectOfType<PlayerSkillReceiver>();
            UpdateText();
        }

        private void UpdateText()
        {
            m_UpgradeText.text = string.Format("{0} / {1}", m_CurrentLevel, m_MaxLevel);
            m_PointCostText.text = string.Format("Point : {0}", m_SkillPointCost);
        }

        public void OnClick()
        {
            if (m_CurrentLevel >= m_MaxLevel || !m_PlayerData.CanSkillUpPoint(m_SkillPointCost)) return;

            m_CurrentLevel++;
            m_SkillPointCost += m_SkillPointIncreseRate;

            UpdateText();
            DoSkillUp();
        }

        protected abstract void DoSkillUp();
    }
}
