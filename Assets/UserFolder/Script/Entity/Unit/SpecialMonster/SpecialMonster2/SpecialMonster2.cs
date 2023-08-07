using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Entity.Unit.Special
{
    public class SpecialMonster2 : MonoBehaviour, IMonster
    {
        [Header("Data")]
        [SerializeField] private Scriptable.Monster.SpecialMonster2Scriptable m_Settings;

        private SP2AnimationController m_SP2AnimationController;
        private NavMeshAgent m_NavMeshAgent;

        private float m_NormalAttackTimer;

        private bool m_IsAlive;

        private int m_RealMaxHP;
        private int m_RealDef;
        private int m_RealDamage;
        private int m_CurrentHP;

        private void Awake()
        {
            m_SP2AnimationController = GetComponentInChildren<SP2AnimationController>();
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void Init(float statMultiplier)
        {
            SetRealStat(statMultiplier);
        }

        private void SetRealStat(float statMultiplier)
        {
            m_IsAlive = true;

            m_RealMaxHP = m_Settings.m_HP + (int)(statMultiplier * m_Settings.m_HPMultiplier);
            m_RealDef = m_Settings.m_Def + (int)(statMultiplier * m_Settings.m_DefMultiplier);
            m_RealDamage = (int)(statMultiplier * m_Settings.m_DamageMultiplier);

            m_CurrentHP = m_RealMaxHP;
        }

        private void Update()
        {
            if (!m_IsAlive) return;
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            m_NormalAttackTimer += Time.deltaTime;

        }

        public void Move()
        {
            
        }

        public void Attack()
        {
            
        }

        public void Die()
        {
            m_IsAlive = false;
        }

        public void Hit(int damage, AttackType bulletType)
        {
            if (!m_IsAlive) return;

            int realDamage;
            if (bulletType == AttackType.Explosion) realDamage = damage / m_Settings.m_ExplosionResistance;
            else if (bulletType == AttackType.Melee) realDamage = damage / m_Settings.m_MeleeResistance;
            else realDamage = damage - m_RealDef;

            m_CurrentHP -= realDamage;

            if (m_CurrentHP <= 0) Die();
        }
    }
}
