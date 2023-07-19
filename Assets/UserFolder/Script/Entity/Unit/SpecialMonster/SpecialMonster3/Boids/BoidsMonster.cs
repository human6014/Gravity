using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.AI;

namespace Entity.Unit.Flying
{
    public class BoidsMonster : PoolableScript, IMonster
    {
        [SerializeField] private Scriptable.Monster.FlyingMonsterScriptable m_Settings;

        private BoidsMovement m_BoidsMovement;
        private PlayerData m_PlayerData;

        private int m_CurrentHP;
        private float m_CurrentAttackTimer;
        private bool m_IsAlive;

        public System.Action<bool> TracePatternAction { get; set; }
        public System.Action<bool> PatrolPatternAction { get; set; }
        public System.Action<PoolableScript> ReturnAction { get; set; }

        private void Awake()
        {
            m_BoidsMovement = GetComponent<BoidsMovement>();
            TracePatternAction += m_BoidsMovement.TryTracePlayer;
            PatrolPatternAction += m_BoidsMovement.TryPatrol;
        }

        private void Start()
        {
            m_PlayerData = AIManager.PlayerTransform.GetComponent<PlayerData>();
        }

        public void Init(Transform moveCenter)
        {
            m_IsAlive = true;
            m_CurrentHP = m_Settings.m_HP;
            m_CurrentAttackTimer = 0;

            m_BoidsMovement.Init(moveCenter);
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

        public void Attack()
        {
            if (m_CurrentAttackTimer < m_Settings.m_AttackSpeed) return;
            m_CurrentAttackTimer = 0;
            m_PlayerData.PlayerHit(transform, m_Settings.m_Damage, m_Settings.m_NoramlAttackType);
        }

        public void Hit(int damage, AttackType bulletType)
        {
            if (!m_IsAlive) return;
            m_CurrentHP -= damage - m_Settings.m_Def;

            if (m_CurrentHP <= 0) Die();
        }

        public void Die()
        {
            m_BoidsMovement.Dispose();
            m_IsAlive = false;
            ReturnObject();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) Attack();
        }

        public override void ReturnObject()
        {
            ReturnAction?.Invoke(this);
        }
    }
}
