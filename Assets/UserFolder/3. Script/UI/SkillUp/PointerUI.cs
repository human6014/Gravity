using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class PointerUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent m_ClickEvent;
    public UnityEvent<bool> m_EnterExitEvent;

    public void OnPointerClick(PointerEventData eventData)
        => m_ClickEvent?.Invoke();
    
    public void OnPointerEnter(PointerEventData eventData)
        => m_EnterExitEvent?.Invoke(true);
    
    public void OnPointerExit(PointerEventData eventData)
        => m_EnterExitEvent?.Invoke(false);
}
