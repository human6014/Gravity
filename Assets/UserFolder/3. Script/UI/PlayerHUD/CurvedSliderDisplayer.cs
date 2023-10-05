using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurvedSliderDisplayer : MonoBehaviour
{
    [SerializeField] private Image m_BarImage;

    public void UpdateBarImage(float value)
    {
        m_BarImage.fillAmount = value * 0.2f;
    }
}
