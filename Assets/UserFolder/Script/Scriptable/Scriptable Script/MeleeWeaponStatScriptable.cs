using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "MeleeWeaponStatSetting", menuName = "Scriptable Object/MeleeWeaponStatSettings", order = int.MaxValue - 8)]
    public class MeleeWeaponStatScriptable : WeaponStatScriptable
    {
        [Space(15)]
        [Header("Child")]
        [Header("Attack Info")]
        [Tooltip("����� �ӵ�")]
        public float m_LightFireTime = 1f;

        [Tooltip("������ �ӵ�")]
        public float m_HeavyFireTime = 1.5f;
    }
}
