using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadableDropdown : LoadableSettingComponent
{
    private TMP_Dropdown m_TMP_Dropdown;

    private void Awake()
    {
        if (m_TMP_Dropdown == null) m_TMP_Dropdown = GetComponent<TMP_Dropdown>();
    }

    public override void LoadComponent(object value)
    {
        if(m_TMP_Dropdown == null) m_TMP_Dropdown = GetComponent<TMP_Dropdown>();

        m_TMP_Dropdown.value = (int)value;
        m_TMP_Dropdown.RefreshShownValue();
    }
}
