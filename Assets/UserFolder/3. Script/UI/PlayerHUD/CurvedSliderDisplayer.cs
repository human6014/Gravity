using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurvedSliderDisplayer : MonoBehaviour
{
    [SerializeField] private Image m_BarImage;
    [SerializeField] private float value = 0;

    private void Update()
    {
        SliderChange(value);
    }

    public void SliderChange(float value)
    {
        float amount = value * 0.002f;
        m_BarImage.fillAmount = amount;
    }
    /*
     * 0~100
     * ->
     * 0~0.2
     * */
}
