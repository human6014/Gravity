using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    [CreateAssetMenu(fileName = "RangeWeaponStatSetting", menuName = "Scriptable Object/RangeWeaponStatSettings", order = int.MaxValue - 7)]
    public class RangeWeaponStatScriptable : WeaponStatScriptable
    {
        [Space(15)]
        [Header("Child")]
        [Header("Attack Info")]
        [Tooltip("�Ϲ� ���� �ӵ�")]
        public float m_AttackTime = 0.15f;

        [Tooltip("���� ���� �ӵ�")]
        public float m_BurstAttackTime = 0.1f;

        [Tooltip("�ִ� ��Ÿ�")]
        public float m_MaxRange = 100;

        [Header("Attack Recoil Info")]
        [Tooltip("���� �⺻ �ݵ���")]
        public float m_UpAxisRecoil = 1.7f;

        [Tooltip("�¿� �⺻ �ݵ���")]
        public float m_RightAxisRecoil = 0.9f;

        [Tooltip("���� ���� �ݵ���")]
        public Vector2 m_UpRandomRecoil = new Vector2(-0.2f, 0.4f);

        [Tooltip("�¿� ���� �ݵ���")]
        public Vector2 m_RightRandomRecoil = new Vector2(-0.15f, 0.2f);

        [Tooltip("��� ��Ȯ��")]
        public float m_Accuracy;

        [Header("Pos")]
        [Tooltip("����, ���� ���� ������")]
        public float m_AimingPosTimeRatio = 0.07f;

        [Header("Spawning")]
        [Tooltip("�Ѿ� ��ȯ ���� ȸ����")]
        public float m_SpinValue = 17;
    }
}
