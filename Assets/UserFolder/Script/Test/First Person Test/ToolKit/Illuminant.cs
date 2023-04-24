using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Illuminant : MonoBehaviour
{
    [System.Serializable]
    public struct Angle
    {
        public float m_InnerAngle;
        public float m_OuterAngle;
        public float m_Intensity;
        public float m_Range;
    }
    [SerializeField] Angle[] m_ZoomAngle;

    private Light m_Light;

    private void Awake()
    {
        m_Light = GetComponent<Light>();

    }

    public void LightOnOff(bool isActive)
    {
        gameObject.SetActive(isActive);
    }

    public void ChangeLightMode(LightMode lightMode)
    {
        int mod = (int)lightMode;
        m_Light.innerSpotAngle = m_ZoomAngle[mod].m_InnerAngle;
        m_Light.spotAngle = m_ZoomAngle[mod].m_OuterAngle;
        m_Light.intensity = m_ZoomAngle[mod].m_Intensity;
        m_Light.range = m_ZoomAngle[mod].m_Range;
    }
}
