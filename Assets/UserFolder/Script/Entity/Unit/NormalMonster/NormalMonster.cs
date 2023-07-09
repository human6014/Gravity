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
        private NormalMonsterAnimController m_NormalMonsterAnimController;
        private PlayerData m_PlayerData;
        
        private int m_CurrentHP;
        private bool m_IsAlive;
        private float m_AttackTimer;

        public System.Action<int, AttackType> HitEvent { get; set; }
        private bool CanAttack
        {
            get => m_AttackTimer >= settings.m_AttackSpeed &&
                    Vector3.Distance(Manager.AI.AIManager.PlayerTransform.position, transform.position)
                    <= settings.m_AttackRange;
        }
        public NoramlMonsterType GetMonsterType() => settings.m_MonsterType;

        public void OnOffRagdoll(bool isActive)
        {
            if (isActive) m_RagDollChanger.ChangeToRagDoll();
            else m_RagDollChanger.ChangeToOriginal();
        }

        private void Awake()
        {
            m_RagDollChanger = GetComponent<RagDollChanger>();
            m_NormalMonsterAI = GetComponent<NormalMonsterAI>();
            m_NormalMonsterAnimController = GetComponent<NormalMonsterAnimController>();
            m_NormalMonsterAI.RagdollOnOffAction += OnOffRagdoll;
        }

        public void Init(Vector3 pos, Manager.ObjectPoolManager.PoolingObject poolingObject)
        {
            m_RagDollChanger.ChangeToOriginal();
            m_CurrentHP = settings.m_HP;
            m_IsAlive = true;
            m_NormalMonsterAI.Init(pos);
            m_NormalMonsterAnimController.Init();
            m_PlayerData = Manager.AI.AIManager.PlayerTransform.GetComponent<PlayerData>();
            m_PoolingObject = poolingObject;
        }

        private void Update()
        {
            if (!m_IsAlive) return;
            m_AttackTimer += Time.deltaTime;

            //¼º´É issue
            Move();

            if (CanAttack) Attack();

            if (m_NormalMonsterAI.IsMalfunction)
            {
                m_IsAlive = false;
                m_NormalMonsterAI.Dispose();
                ReturnObject();
            }
        }

        public void Move()
        {
            m_NormalMonsterAI.Move();
            if (m_NormalMonsterAI.IsClimbing) m_NormalMonsterAnimController.PlayCrawl(true);
            else m_NormalMonsterAnimController.PlayWalk(true);
        }

        public void Attack()
        {
            m_AttackTimer = 0;

            m_NormalMonsterAnimController.PlayAttack();
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
            m_CurrentHP = 0;
            m_IsAlive = false;

            m_NormalMonsterAI.Dispose();
            m_RagDollChanger.ChangeToRagDoll();
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
