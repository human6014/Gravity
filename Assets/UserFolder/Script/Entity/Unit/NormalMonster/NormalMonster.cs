using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumType;

namespace Entity.Unit.Normal
{
    public class NormalMonster : PoolableScript, IMonster
    {
        [SerializeField] private Scriptable.Monster.NormalMonsterScriptable settings;

        private PlayerData m_PlayerData;
        private Animator m_Animator;
        private NormalMonsterAI m_NormalMonsterAI;
        private int m_CurrentHP;
        private bool m_IsAlive;
        private float m_AttackTimer;

        public NoramlMonsterType GetMonsterType() => settings.m_MonsterType;

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_NormalMonsterAI = GetComponent<NormalMonsterAI>();
        }

        public void Init(Vector3 pos, Manager.ObjectPoolManager.PoolingObject poolingObject)
        {
            m_CurrentHP = settings.m_HP;
            m_IsAlive = true;
            m_NormalMonsterAI.Init(pos);
            m_PoolingObject = poolingObject;
        }

        private void Update()
        {
            if (!m_IsAlive) return;
            m_AttackTimer += Time.deltaTime;

            //¼º´É issue
            Move();

            if (CanAttack()) Attack();

            if (m_NormalMonsterAI.IsMalfunction) ReturnObject();
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

            if (m_PlayerData == null)
            {
                Manager.AI.AIManager.PlayerTransform.TryGetComponent(out PlayerData playerData);
                m_PlayerData = playerData;
            }

            m_PlayerData.PlayerHit(transform, settings.m_Damage, settings.m_AttackType);
        }

        public void Hit(int damage, AttackType bulletType)
        {
            if (!m_IsAlive) return;
            if (bulletType == AttackType.Explosion) m_CurrentHP -= (damage / settings.m_ExplosionResistance);
            else m_CurrentHP -= (damage - settings.m_Def);

            if (m_CurrentHP <= 0) Die();
        }

        public void Die()
        {
            Debug.Log("Die");
            m_IsAlive = false;
            Debug.Log(m_CurrentHP);
            m_CurrentHP = 0;
            ReturnObject();
        }

        [ContextMenu("ReturnObject")]
        public override void ReturnObject()
        {
            m_NormalMonsterAI.Dispose();
            Manager.SpawnManager.NormalMonsterCount--;
            m_PoolingObject.ReturnObject(this);
        }
    }
}
