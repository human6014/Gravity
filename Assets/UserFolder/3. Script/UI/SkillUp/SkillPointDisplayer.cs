using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPointDisplayer : MonoBehaviour
{
    private Text m_PointText;

    private void Awake()
        => m_PointText = GetComponent<Text>();

    public void UpdatePointText(int point)
        => m_PointText.text = string.Format("Point : {0}", point);
}
