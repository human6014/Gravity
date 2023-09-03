using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameImageController : MonoBehaviour
{
    private Image m_FrameImage;

    [SerializeField] private Color m_NormalColor = new Color(128, 128, 128, 0);
    [SerializeField] private Color m_HighlightColor = new Color(128, 128, 128, 255);

    private void Awake() => m_FrameImage = GetComponent<Image>();
    
    public void ChangeHighlightColor(bool isHighlight) 
        => m_FrameImage.color = isHighlight ? m_HighlightColor : m_NormalColor;
}
