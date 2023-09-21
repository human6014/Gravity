using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeyBind : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private KeyBindController m_KeyBindController;
    [SerializeField] private TextMeshProUGUI m_TextMesh;
    [SerializeField] private int Index;

    private Action<KeyCode> KeyTextAction;

    private void Awake()
        => KeyTextAction = (KeyCode keyCode) => m_TextMesh.text = keyCode.ToString().ToUpper();

    public void OnPointerClick(PointerEventData eventData)
        => m_KeyBindController.OnClickBindKey(Index,KeyTextAction);

    public void SetText(string text) => m_TextMesh.text = text.ToUpper();
}
