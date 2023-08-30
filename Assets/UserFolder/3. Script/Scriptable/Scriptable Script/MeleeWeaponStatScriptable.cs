using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable.Equipment
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

        [Header("SphereCast")]
        [Tooltip("���� ����")]
        public float m_SwingRadius;

        [Tooltip("�ִ� ��Ÿ�")]
        public float m_MaxDistance;

        [Tooltip("��Ÿ ĵ�� ����")]
        public bool m_CanComboAttack;

        [Tooltip("���ݽ� ȭ�� ��鸲 ����")]
        public bool m_DoShake;
    }
}
