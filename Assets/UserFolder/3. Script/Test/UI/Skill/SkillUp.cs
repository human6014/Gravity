using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SkillUp : MonoBehaviour
{
    [SerializeField] private Text m_UpgradeText;
    [SerializeField] private Text m_PointCostText;

    [SerializeField] private int m_SkillPointCost = 5;
    [SerializeField] private int m_MaxLevel = 10;

    private int m_CurrentLevel;
    protected PlayerSkillReceiver PlayerSkillReceiver { get; private set; }

    private void Awake()
    {
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
        //if 돈 되면 && 현재 레벨 < 최대 레벨
        if (m_CurrentLevel >= m_MaxLevel) return;

        m_CurrentLevel++;

        UpdateText();
        DoSkillUp();
    }

    protected abstract void DoSkillUp();
}
