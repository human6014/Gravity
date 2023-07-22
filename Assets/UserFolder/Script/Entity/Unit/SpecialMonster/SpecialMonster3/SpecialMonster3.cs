using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit.Special
{
    public class SpecialMonster3 : MonoBehaviour, IMonster
    {
        [SerializeField] [Range(0, 2000)] private int m_BoidsInstacingCount = 500;

        private BoidsController m_BoidsController;
        private Animator m_Animator;
        private PlayerData m_PlayerData;

        private bool m_IsAlive;

        private int m_RealMaxHP;
        private int m_RealDef;
        private int m_RealDamage;
        private int m_CurrentHP;

        private float m_AttackTimer;

        public System.Action EndSpecialMonsterAction { get; set; }

        private void Awake()
        {
            m_BoidsController = GetComponent<BoidsController>();
            //m_Animator = GetComponent<Animator>();
        }

        public void Init(Vector3 pos, float statMultiplier)
        {
            SetRealStat(statMultiplier);
        }

        private void SetRealStat(float statMultiplier)
        {
            m_IsAlive = true;

            //m_RealMaxHP = settings.m_HP + (int)(statMultiplier * settings.m_HPMultiplier);
            //m_RealDef = settings.m_Def + (int)(statMultiplier * settings.m_HPMultiplier);
            //m_RealDamage = settings.m_Damage + (int)(statMultiplier * settings.m_Damage);

            m_CurrentHP = m_RealMaxHP;
        }

        void Start()
        {
            m_BoidsController.GenerateBoidMonster(m_BoidsInstacingCount);
        }

        public void Attack()
        {
            
        }

        public void Move()
        {

        }

        public void Hit(int damage, AttackType bulletType)
        {

        }

        public void Die()
        {
            m_IsAlive = false;
            EndSpecialMonsterAction?.Invoke();

        }
    }
}
