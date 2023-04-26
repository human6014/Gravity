using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Illuminant : MonoBehaviour
{
    private Light m_Light;

    private void Awake() => m_Light = GetComponent<Light>();
   
    public void LightOnOff(bool isActive) => m_Light.enabled = isActive;
    
    public void ChangeLightMode(Scriptable.FlashLightStatScriptable.Angle angle)
    {
        m_Light.innerSpotAngle = angle.m_InnerAngle;
        m_Light.spotAngle = angle.m_OuterAngle;
        m_Light.intensity = angle.m_Intensity;
        m_Light.range = angle.m_Range;
    }
}
