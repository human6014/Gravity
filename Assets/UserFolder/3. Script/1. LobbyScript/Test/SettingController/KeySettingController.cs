using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KeySettingController : SettingController
{
    [SerializeField] private UnityEvent BindStartEvent;
    [SerializeField] private UnityEvent BindEndEvent;

    [SerializeField] private KeyBind[] m_KeyBinds;

    private GameControlSetting m_GameControlSetting;

    private Action<KeyCode> CurrentKeyBindingAction;
    private bool m_ReceiveKeyMode;
    private int m_CurrentReceiveKey;

    private void Start()
    {
        if (DataManager.Instance == null) return;
        m_GameControlSetting = (GameControlSetting)DataManager.Instance.Settings[1];
        UpdateSettings();
    }

    private bool IsIgnoreKey(KeyCode keyCode)
    {
        if (keyCode >= KeyCode.Mouse0 && keyCode <= KeyCode.Mouse6) return true;

        return false;
    }

    public void OnClickBindKey(int index, Action<KeyCode> keyBindingAction)
    {
        if (m_ReceiveKeyMode) return;

        m_ReceiveKeyMode = true;
        CurrentKeyBindingAction = keyBindingAction;
        m_CurrentReceiveKey = index;
        BindStartEvent?.Invoke();
    }

    public void OnClickModalCancel()
    {
        Debug.Log("BindEndEvent");
        BindEndEvent?.Invoke();
        m_ReceiveKeyMode = false;
    }

    private void LateUpdate()
    {
        if (!m_ReceiveKeyMode) return;
        if (!Input.anyKey) return;

        Array keyCodeArray = Enum.GetValues(typeof(KeyCode));
        KeyCode input = KeyCode.None;

        foreach (KeyCode keyCode in keyCodeArray)
        {
            if (IsIgnoreKey(keyCode)) return;
            if (Input.GetKeyDown(keyCode))
            {
                input = keyCode;
                break;
            }
        }
        if (input != KeyCode.Escape)
        {
            Debug.Log("KeyBindEvent");
            m_GameControlSetting[m_CurrentReceiveKey] = input;
            CurrentKeyBindingAction?.Invoke(input);
        }
        OnClickModalCancel();
    }

    public override void UpdateSettings()
    {
        for (int i = 0; i < m_LoadableSettingComponents.Length; i++)
            m_LoadableSettingComponents[i].LoadComponent(m_GameControlSetting[i]);
    }

    public override void SaveSettings() { }
}
