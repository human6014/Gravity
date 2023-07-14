using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumType;
using Manager.AI;

namespace Entity.Unit.Normal
{
    public class NormalMonster : PoolableScript, IMonster, IPhysicsable
    {
        [SerializeField] private Scriptable.Monster.NormalMonsterScriptable settings;

        private CapsuleCollider m_CapsuleCollider;
        private RagDollChanger m_RagDollChanger;
        private NormalMonsterAI m_NormalMonsterAI;
        private NormalMonsterAnimController m_NormalMonsterAnimController;
        private NormalMonsterState m_NormalMonsterState;
        private PlayerData m_PlayerData;
        
        private int m_CurrentHP;
        private bool m_IsAlive;
        private float m_AttackTimer;

        private float m_GettingUpTimer = 5.5f;
        private float m_CurrentGettingUpTimer;

        public System.Action<int, AttackType> HitEvent { get; set; }

        private bool CanAttackRange
        {
            get => Vector3.Distance(AIManager.PlayerTransform.position, transform.position) <= settings.m_AttackRange;
        }

        private bool CanAttack
        {
            get => m_AttackTimer >= settings.m_AttackSpeed && m_NormalMonsterState.CanAttackState;
        }
        public NoramlMonsterType GetMonsterType { get => settings.m_MonsterType; }

        public void OnOffRagdoll(bool isActive)
        {
            m_CapsuleCollider.enabled = !isActive;
            if (isActive) m_RagDollChanger.ChangeToRagDoll();
            else m_RagDollChanger.ChangeToOriginal();
        }

        private void Awake()
        {
            m_CapsuleCollider = GetComponent<CapsuleCollider>();
            m_RagDollChanger = GetComponent<RagDollChanger>();
            m_NormalMonsterAI = GetComponent<NormalMonsterAI>();
            m_NormalMonsterAnimController = GetComponentInChildren<NormalMonsterAnimController>();
            
            m_NormalMonsterState = new NormalMonsterState(m_NormalMonsterAnimController);
            m_NormalMonsterAI.RagdollOnOffAction += OnOffRagdoll;
            m_NormalMonsterAI.NormalMonsterState = m_NormalMonsterState;
        }

        public void Init(Vector3 pos, Manager.ObjectPoolManager.PoolingObject poolingObject)
        {
            OnOffRagdoll(false);
            m_CurrentHP = settings.m_HP;
            m_IsAlive = true;
            m_NormalMonsterState.Init();
            m_NormalMonsterAI.Init(pos);
            m_NormalMonsterAnimController.Init();
            m_PlayerData = AIManager.PlayerTransform.GetComponent<PlayerData>();
            m_PoolingObject = poolingObject;
        }

        private void Update()
        {
            if (!m_IsAlive) return;
            m_AttackTimer += Time.deltaTime;
            //Debug.Log("Current State : " + m_NormalMonsterState.BehaviorState);

            if (m_NormalMonsterAI.CheckCanBehaviorState(out bool isMalfunction))
                Move();

            if (isMalfunction)
            {
                m_IsAlive = false;
                m_NormalMonsterAI.Dispose();
                ReturnObject();
            }
        }

        public void Move()
        {
            if (CanAttackRange)
            {
                if (CanAttack) Attack();
            }
            else m_NormalMonsterAI.AutoBehavior();
        }

        public void Attack()
        {
            m_AttackTimer = 0;

            m_NormalMonsterState.SetTriggerAttacking();
            m_PlayerData.PlayerHit(transform, settings.m_Damage, settings.m_NoramlAttackType);
        }

        public void Hit(int damage, AttackType bulletType)
        {
            Debug.Log("Hit");
            if (!m_IsAlive) return;
            if (bulletType == AttackType.Explosion) m_CurrentHP -= (damage / settings.m_ExplosionResistance);
            else if (bulletType == AttackType.Melee) m_CurrentHP -= (damage / settings.m_MeleeResistance);
            else m_CurrentHP -= (damage - settings.m_Def);

            if (m_CurrentHP <= 0) Die();
        }

        public bool PhysicsableHit(int damage, AttackType bulletType)
        {
            if (!m_IsAlive) return false;
            if (bulletType == AttackType.Explosion) m_CurrentHP -= (damage / settings.m_ExplosionResistance);
            else if (bulletType == AttackType.Melee) m_CurrentHP -= (damage / settings.m_MeleeResistance);
            else m_CurrentHP -= (damage - settings.m_Def);

            if (m_CurrentHP <= 0)
            {
                Die();
                return true;
            }
            return false;
        }

        public void Die()
        {
            m_CurrentHP = 0;
            m_IsAlive = false;

            m_NormalMonsterAI.Dispose();
            if (!m_NormalMonsterAI.IsFalling) OnOffRagdoll(true);
            else m_CapsuleCollider.enabled = true;
            Invoke(nameof(ReturnObject),10);
        }

        [ContextMenu("ReturnObject")]
        public override void ReturnObject()
        {
            Manager.SpawnManager.NormalMonsterCount--;
            m_PoolingObject.ReturnObject(this);
        }
    }
}
