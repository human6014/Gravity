using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CrossHair
{
    None = 0,           //없음
    Normal = 1,         //중앙점
    Reticle = 2,        //십자선
    Circle = 3,         //원형선
}
public enum BulletType
{
    None = 0,
    Generic = 1,
    HandGun = 2,
    ShotGun = 3,
    Sinper = 4
}
namespace Scriptable
{

    public class WeaponStatScriptable : ScriptableObject
    {
        [Header("Parent")]
        [Header("Attack info")]
        [Tooltip("일반 공격 속도")]
        public float m_AttackTime = 0.15f;
        [Tooltip("공격 데미지")]
        public int m_Damage;

        [Tooltip("공격에 걸릴 레이어")]
        public LayerMask m_AttackableLayer;


        [Header("Pos")]
        [Tooltip("달리기 전환 시간")]
        public float m_RunningPosTime = 0.5f;
        

        [Header("UI")]
        [Tooltip("크로스 헤어 UI 종류")]
        public CrossHair m_DefaultCrossHair = 0;


        [Header("RunningAnimPos")]
        [Tooltip("달릴 때 피봇 위치")]
        public Vector3 m_RunningPivotPosition;

        [Tooltip("달릴 때 피봇 각도")]
        public Vector3 m_RunningPivotDirection;

        [Tooltip("달릴 때 FOV 가감 속도")]
        public float m_FOVMultiplier = 4;

        [Space(10)]
        [Header("UI")]
        [Tooltip("Weapon Icon")]
        public Sprite m_WeaponIcon;

        [Tooltip("Bullet Type")]
        public BulletType m_BulletType;
    }
}
