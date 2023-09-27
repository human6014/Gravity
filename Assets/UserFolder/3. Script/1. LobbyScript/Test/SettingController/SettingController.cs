using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SettingController : MonoBehaviour
{
    [SerializeField] protected LoadableSettingComponent[] m_LoadableSettingComponents;

    public abstract void UpdateSettings();
    public abstract void SaveSettings();
}
