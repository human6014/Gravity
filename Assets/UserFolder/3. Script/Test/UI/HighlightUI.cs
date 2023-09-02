using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class HighlightUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image m_FrameImage;

    public UnityEvent m_ClickEvent;
    public UnityEvent<bool> m_EnterExitEvent;

    private void Awake()
    {
        m_FrameImage = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        m_ClickEvent?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_EnterExitEvent?.Invoke(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_EnterExitEvent?.Invoke(false);
    }
}
