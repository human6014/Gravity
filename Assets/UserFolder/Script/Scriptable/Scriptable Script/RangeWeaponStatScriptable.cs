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
        [Tooltip("일반 공격 속도")]
        public float m_AttackTime = 0.15f;

        [Tooltip("점사 공격 속도")]
        public float m_BurstAttackTime = 0.1f;

        [Tooltip("최대 사거리")]
        public float m_MaxRange = 100;

        [Header("Attack Recoil Info")]
        [Tooltip("상하 기본 반동값")]
        public float m_UpAxisRecoil = 1.7f;

        [Tooltip("좌우 기본 반동값")]
        public float m_RightAxisRecoil = 0.9f;

        [Tooltip("상하 랜덤 반동값")]
        public Vector2 m_UpRandomRecoil = new Vector2(-0.2f, 0.4f);

        [Tooltip("좌우 랜덤 반동값")]
        public Vector2 m_RightRandomRecoil = new Vector2(-0.15f, 0.2f);

        [Tooltip("사격 정확도")]
        public float m_Accuracy;

        [Header("Pos")]
        [Tooltip("조준, 조준 해제 증감값")]
        public float m_AimingPosTimeRatio = 0.07f;

        [Header("Spawning")]
        [Tooltip("총알 소환 랜덤 회전값")]
        public float m_SpinValue = 17;
    }
}
