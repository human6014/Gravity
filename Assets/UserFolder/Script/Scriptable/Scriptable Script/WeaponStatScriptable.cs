using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptable
{
    public enum CrossHair
    {
        None = 0,           //없음
        Normal = 1,         //중앙점
        Reticle = 2,        //십자선
        Circle = 3,         //원형선
    }

    [CreateAssetMenu(fileName = "WeaponStatSetting", menuName = "Scriptable Object/WeaponStatSettings", order = int.MaxValue - 11)]
    public class WeaponStatScriptable : ScriptableObject
    {
        [Header("Parent")]
        [Header("Attack info")]
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
    }
}
