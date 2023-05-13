using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumType;

namespace Entity.Unit.Normal
{
    public class NormalMonster : PoolableScript, IMonster, IPhysicsable
    {
        [SerializeField] private Scriptable.Monster.NormalMonsterScriptable settings;

        private RagDollChanger m_RagDollChanger;
        private NormalMonsterAI m_NormalMonsterAI;
        private PlayerData m_PlayerData;
        private Animator m_Animator;
        
        private int m_CurrentHP;
        private bool m_IsAlive;
        private float m_AttackTimer;

        public System.Action<int, AttackType> HitEvent { get; set; }

        public NoramlMonsterType GetMonsterType() => settings.m_MonsterType;

        private void Awake()
        {
            m_RagDollChanger = GetComponent<RagDollChanger>();
            m_NormalMonsterAI = GetComponent<NormalMonsterAI>();
            m_Animator = GetComponentInChildren<Animator>();
        }

        public void Init(Vector3 pos, Manager.ObjectPoolManager.PoolingObject poolingObject)
        {
            m_RagDollChanger.ChangeToOriginal();
            m_CurrentHP = settings.m_HP;
            m_IsAlive = true;
            m_NormalMonsterAI.Init(pos);
            m_PlayerData = Manager.AI.AIManager.PlayerTransform.GetComponent<PlayerData>();
            m_PoolingObject = poolingObject;
        }

        private void Update()
        {
            if (!m_IsAlive) return;
            m_AttackTimer += Time.deltaTime;

            //¼º´É issue
            Move();

            if (CanAttack()) Attack();

            if (m_NormalMonsterAI.IsMalfunction)
            {
                m_IsAlive = false;
                ReturnObject();
            }
        }

        private bool CanAttack()
        {
            if (m_AttackTimer >= settings.m_AttackSpeed &&
                Vector3.Distance(Manager.AI.AIManager.PlayerTransform.position, transform.position) <= settings.m_AttackRange)
                return true;
            return false;
        }

        public void Move()
        {
            m_NormalMonsterAI.Move();
            //animator.SetBool("isMove", true);
        }

        public void Attack()
        {
            m_AttackTimer = 0;

            m_PlayerData.PlayerHit(transform, settings.m_Damage, settings.m_NoramlAttackType);
        }

        public void Hit(int damage, AttackType bulletType)
        {
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
            Debug.Log("Die");
            m_CurrentHP = 0;
            m_IsAlive = false;

            m_NormalMonsterAI.Dispose();
            m_RagDollChanger.ChangeToRagDoll();
            Invoke(nameof(ReturnObject),10);
            //ReturnObject();
        }

        [ContextMenu("ReturnObject")]
        public override void ReturnObject()
        {
            
            Manager.SpawnManager.NormalMonsterCount--;
            m_PoolingObject.ReturnObject(this);
        }
    }
}
