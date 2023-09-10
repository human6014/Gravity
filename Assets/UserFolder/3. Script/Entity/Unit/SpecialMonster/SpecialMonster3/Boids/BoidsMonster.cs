using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Manager.AI;


namespace Entity.Unit.Flying
{
    public class BoidsMonster : PoolableScript, IMonster
    {
        [SerializeField] private Scriptable.Monster.FlyingMonsterScriptable m_Settings;

        private Rigidbody m_Rigidbody;
        private CapsuleCollider m_CapsuleCollider;
        private BoidsMovementCPU m_BoidsMovement;
        private PlayerData m_PlayerData;

        private int m_CurrentHP;
        private float m_CurrentAttackTimer;
        private bool m_IsAlive;
        public Action<bool> TracePatternAction { get; set; }
        public Action<bool> PatrolPatternAction { get; set; }
        public Action<int> DieAction { get; set; }
        public Action<PoolableScript> ReturnAction { get; set; }
        public int MaxHP { get => m_Settings.m_HP; }

        private void Awake()
        {
            m_BoidsMovement = GetComponent<BoidsMovementCPU>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();

            TracePatternAction += m_BoidsMovement.TryTracePlayer;
            PatrolPatternAction += m_BoidsMovement.TryPatrol;
        }

        private void Start()
        {
            m_PlayerData = AIManager.PlayerTransform.GetComponent<PlayerData>();
        }

        public void Init(Transform moveCenter, bool isTrace, bool isPatrol)
        {
            m_IsAlive = true;
            m_CurrentHP = m_Settings.m_HP;
            m_CurrentAttackTimer = 0;

            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
            m_CapsuleCollider.radius = 0.9f;

            m_BoidsMovement.Init(moveCenter, isTrace, isPatrol);
        }

        public void Update()
        {
            if (!m_IsAlive) return;
            
            Move();
            m_CurrentAttackTimer += Time.deltaTime;
        }

        public void Move()
        {
            m_BoidsMovement.CalcAndMove();
        }

        //Event»£√‚µ 
        public void Attack()
        {
            if (m_CurrentAttackTimer < m_Settings.m_AttackSpeed) return;
            m_CurrentAttackTimer = 0;
            m_PlayerData.PlayerHit(transform, m_Settings.m_Damage, m_Settings.m_NoramlAttackType);
        }

        public void Hit(int damage, AttackType bulletType)
        {
            if (!m_IsAlive) return;

            int realDamage;
            if (bulletType == AttackType.Explosion) realDamage = damage / m_Settings.m_ExplosionResistance;
            else realDamage = damage - m_Settings.m_Def;
            m_CurrentHP -= realDamage;

            if (m_CurrentHP <= 0) Die();
        }

        public void Die()
        {
            m_BoidsMovement.Dispose();
            m_IsAlive = false;

            m_CapsuleCollider.radius = 0.3f;
            m_Rigidbody.isKinematic = false;
            m_Rigidbody.useGravity = true;
            m_Rigidbody.AddForce(transform.forward * 12, ForceMode.Impulse);

            DieAction?.Invoke(m_Settings.m_HP);
            Invoke(nameof(ReturnObject), 10);
        }

        public override void ReturnObject()
        {
            ReturnAction?.Invoke(this);
        }
    }
}
