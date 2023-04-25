using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "FlashLightStatSetting", menuName = "Scriptable Object/FlashLightStatSettings", order = int.MaxValue - 13)]
    public class FlashLightStatScriptable : WeaponStatScriptable
    {
        [System.Serializable]
        public struct Angle
        {
            public float m_InnerAngle;
            public float m_OuterAngle;
            public float m_Intensity;
            public float m_Range;
        }
        public Angle[] m_ZoomAngle;
    }
}
