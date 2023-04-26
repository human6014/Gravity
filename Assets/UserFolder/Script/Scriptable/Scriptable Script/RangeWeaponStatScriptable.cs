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
        [Tooltip("점사 공격 속도")]
        public float m_BurstAttackTime = 0.1f;

        [Tooltip("최대 사거리")]
        public float m_MaxRange = 100;

        [Tooltip("최대 장탄 수")]
        public int m_MaxBullets = 30;

        [Header("Attack Recoil")]
        [Tooltip("상하 기본 반동값")]
        public float m_UpAxisRecoil = 1.7f;

        [Tooltip("좌우 기본 반동값")]
        public float m_RightAxisRecoil = 0.9f;

        [Tooltip("상하 랜덤 반동값")]
        public Vector2 m_UpRandomRecoil = new Vector2(-0.2f, 0.4f);

        [Tooltip("좌우 랜덤 반동값")]
        public Vector2 m_RightRandomRecoil = new Vector2(-0.15f, 0.2f);


        [Header("Attack Accuracy")]
        [Tooltip("기본 상태 사격 정확도")]
        public float m_IdleAccuracy;

        [Tooltip("에임 상태 사격 정확도")]
        public float m_AimingAccuracy;


        [Header("Pos")]
        [Tooltip("조준, 조준 해제 증감값")]
        public float m_AimingPosTimeRatio = 0.07f;


        [Header("Spawning")]
        [Tooltip("총알 소환 랜덤 회전값")]
        public float m_SpinValue = 17;


        [Header("AimingAnimPos")]
        [Tooltip("에임시 pivot 위치")]
        public Vector3 m_AimingPivotPosition;

        [Tooltip("에임시 pivot 각도")]
        public Vector3 m_AimingPivotDirection;

        [Tooltip("에임시 FOV 증감값")]
        public float m_AimingFOV;
    }
}
