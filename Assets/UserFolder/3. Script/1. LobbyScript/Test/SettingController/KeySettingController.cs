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

    private Action<KeyCode> CurrentKeyBindingAction;
    private bool m_ReceiveKeyMode;
    private int m_CurrentReceiveKey;

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
        BindEndEvent?.Invoke();
        m_ReceiveKeyMode = false;
    }

    private void Update()
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
            GameControlSetting.ChangeKey(m_CurrentReceiveKey, input);
            CurrentKeyBindingAction?.Invoke(input);
        }
        BindEndEvent?.Invoke();
        m_ReceiveKeyMode = false;
    }
}
