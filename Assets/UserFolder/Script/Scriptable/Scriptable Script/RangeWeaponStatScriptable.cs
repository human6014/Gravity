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
        [Tooltip("���� ���� �ӵ�")]
        public float m_BurstAttackTime = 0.1f;

        [Tooltip("�ִ� ��Ÿ�")]
        public float m_MaxRange = 100;

        [Tooltip("�ִ� ��ź ��")]
        public int m_MaxBullets = 30;

        [Header("Attack Recoil")]
        [Tooltip("���� �⺻ �ݵ���")]
        public float m_UpAxisRecoil = 1.7f;

        [Tooltip("�¿� �⺻ �ݵ���")]
        public float m_RightAxisRecoil = 0.9f;

        [Tooltip("���� ���� �ݵ���")]
        public Vector2 m_UpRandomRecoil = new Vector2(-0.2f, 0.4f);

        [Tooltip("�¿� ���� �ݵ���")]
        public Vector2 m_RightRandomRecoil = new Vector2(-0.15f, 0.2f);


        [Header("Attack Accuracy")]
        [Tooltip("�⺻ ���� ��� ��Ȯ��")]
        public float m_IdleAccuracy;

        [Tooltip("���� ���� ��� ��Ȯ��")]
        public float m_AimingAccuracy;


        [Header("Pos")]
        [Tooltip("����, ���� ���� ������")]
        public float m_AimingPosTimeRatio = 0.07f;


        [Header("Spawning")]
        [Tooltip("�Ѿ� ��ȯ ���� ȸ����")]
        public float m_SpinValue = 17;


        [Header("AimingAnimPos")]
        [Tooltip("���ӽ� pivot ��ġ")]
        public Vector3 m_AimingPivotPosition;

        [Tooltip("���ӽ� pivot ����")]
        public Vector3 m_AimingPivotDirection;

        [Tooltip("���ӽ� FOV ������")]
        public float m_AimingFOV;
    }
}
