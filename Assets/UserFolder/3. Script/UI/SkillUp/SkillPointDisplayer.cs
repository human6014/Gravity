using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPointDisplayer : MonoBehaviour
{
    private Text m_PointText;

    private void Awake()
    {
        if (m_PointText == null) m_PointText = GetComponent<Text>();
    }

    public void UpdatePointText(int point)
    {
        if (m_PointText == null) m_PointText = GetComponent<Text>();
        m_PointText.text = string.Format("Point : {0}", point);
    }
}
