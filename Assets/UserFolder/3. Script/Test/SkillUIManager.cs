using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUIManager : MonoBehaviour
{
    [SerializeField] private SkillPanel m_SkillPanel;
    private bool m_IsOnOffSkillUI;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab)) SkillUI(!m_IsOnOffSkillUI);
    }

    private void SkillUI(bool isActive)
    {

    }
}
