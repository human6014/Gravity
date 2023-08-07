using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GravityEnergeDisplayer : MonoBehaviour
{
    [SerializeField] private Image m_GEImage;

    public void UpdateGEImage(float value)
    {
        m_GEImage.fillAmount = value;
    }
}
