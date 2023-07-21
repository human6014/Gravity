using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Monster
{
    [CreateAssetMenu(fileName = "SP1LegSetting", menuName = "Scriptable Object/SP1LegSettings", order = int.MaxValue - 15)]
    public class SP1MonsterLegScriptable : ScriptableObject
    {
        [Header("Legs info")]
        public AnimationCurve m_SpeedCurve;
        public AnimationCurve m_HeightCurve;

        public LayerMask m_FDLayerMask;
        public LayerMask m_BLayerMask;
    }
}
