using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Manager.AI;
using Scriptable.Monster;

namespace Entity.Unit.Normal
{
    public class NormalMonster : PoolableScript, IMonster, IPhysicsable
    {
        [SerializeField] private NormalMonsterScriptable m_Settings;

        private CapsuleCollider m_CapsuleCollider;
        private RagDollCachedChanger m_RagDollChanger;
        private NormalMonsterAI m_NormalMonsterAI;
        private NormalMonsterState m_NormalMonsterState;
        private PlayerData m_PlayerData;

        private bool m_IsAlive;
        private bool m_CanRun;

        private int m_RealMaxHP;
        private int m_RealDef;
        private int m_RealDamage;
        private int m_CurrentHP;
        
        private float m_AttackTimer;

        public Action<NoramlMonsterType> KilledNormalMonsterAction { get; set; }

        public Action EndNormalMonsterAction { get; set; }

        public NoramlMonsterType GetMonsterType { get => m_Settings.m_MonsterType; }


        private bool CanAttack() 
            => m_AttackTimer >= m_Settings.m_AttackSpeed && m_NormalMonsterState.CanAttackState;

        private bool CanAttackRange(float additionalRange = 0)
        {
            return Vector3.Distance(AIManager.PlayerTransform.position, transform.position)
                <= m_Settings.m_AttackRange + additionalRange;
        }

        public void OnOffRagdoll(bool isActive)
        {
            m_CapsuleCollider.enabled = !isActive;
            if (isActive) m_RagDollChanger.ChangeToRagDoll();
            else m_RagDollChanger.ChangeToOriginal();
        }

        #region Only one init
        private void Awake()
        {
            m_CapsuleCollider = GetComponent<CapsuleCollider>();
            m_RagDollChanger = GetComponent<RagDollCachedChanger>();
            m_NormalMonsterAI = GetComponent<NormalMonsterAI>();

            NormalMonsterAnimController normalMonsterAnimController = GetComponentInChildren<NormalMonsterAnimController>();
            normalMonsterAnimController.DoDamageAction += DoDamage;
            m_NormalMonsterState = new NormalMonsterState(normalMonsterAnimController);

            m_NormalMonsterAI.NormalMonsterState = m_NormalMonsterState;
            m_NormalMonsterAI.RagdollOnOffAction += OnOffRagdoll;
        }

        private void Start() => m_PlayerData = AIManager.PlayerTransform.GetComponent<PlayerData>();
        #endregion

        #region Init
        public void Init(Vector3 pos, Manager.ObjectPoolManager.PoolingObject poolingObject, float statMultiplier)
        {
            OnOffRagdoll(false);
            SetRealStat(statMultiplier);

            float movementSpeed = m_CanRun ? m_Settings.m_MovementSpeed : m_Settings.m_RunningSpeed;

            m_PoolingObject = poolingObject;

            m_NormalMonsterState.Init();
            m_NormalMonsterAI.Init(pos, m_CanRun, movementSpeed);
        }

        /// <summary>
        /// ���̵�, Stage, Wave �ݿ� �ɷ�ġ ����
        /// </summary>
        /// <param name="statMultiplier">�ɷ�ġ ������ ���</param>
        private void SetRealStat(float statMultiplier)
        {
            m_IsAlive = true;

            m_RealMaxHP = m_Settings.m_HP + (int)(statMultiplier * m_Settings.m_HPMultiplier);
            m_RealDef = m_Settings.m_Def + (int)(statMultiplier * m_Settings.m_DefMultiplier);
            m_RealDamage = m_Settings.m_Damage + (int)(statMultiplier * m_Settings.m_DamageMultiplier);
            m_CanRun = statMultiplier >= m_Settings.m_CanRunStat;

            m_CurrentHP = m_RealMaxHP;
        }

        /// <summary>
        /// ���� ��ȯ �� ���帰 ���¿��� ����Ű�� �ִϸ��̼� ����
        /// </summary>
        public void PlayStartAnimation()
        {
            if (m_NormalMonsterState != null) m_NormalMonsterState.SetTriggerGettingUp();
        }
        #endregion

        private void Update()
        {
            if (!m_IsAlive) return;
            if (Manager.GameManager.IsGameClearEnd)
            {
                Die();
                return;
            }

            m_AttackTimer += Time.deltaTime;

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
            if (CanAttackRange())
            {
                if (CanAttack() && AIManager.IsInsideAngleToPlayer(transform, m_Settings.m_AttackAbleAngle)) 
                    Attack();
            }
            else m_NormalMonsterAI.AutoBehavior();
        }

        #region Attack
        public void Attack()
        {
            //m_AttackTimer = 0;
            m_AttackTimer -= m_Settings.m_AttackSpeed;

            m_NormalMonsterState.SetTriggerAttacking();
        }

        /// <summary>
        /// �ִϸ��̼ǿ��� ���� Ÿ�̹��� ���� �� ȣ��
        /// </summary>
        private void DoDamage()
        {
            if (!CanAttackRange(2) || !AIManager.IsInsideAngleToPlayer(transform, m_Settings.m_AttackAbleAngle)) return;
            m_PlayerData.PlayerHit(transform, m_RealDamage, m_Settings.m_NoramlAttackType);
        }
        #endregion

        #region Hit
        /// <summary>
        /// ������ ��� ���� �������� ó���� ��� �ش� �Լ��� Hitȣ��
        /// </summary>
        /// <param name="damage">���� ������</param>
        /// <param name="attackType">���� Ÿ��</param>
        public void Hit(int damage, AttackType attackType)
        {
            if (!m_IsAlive) return;
            TypeToDamage(damage, attackType);

            if (m_CurrentHP <= 0) Die();
        }

        /// <summary>
        /// �׾��� �� ���׵��� ���� �ݵ� �ݿ��� ��� �ش� �Լ��� Hit ȣ��
        /// </summary>
        /// <param name="damage">���� ������</param>
        /// <param name="attackType">���� Ÿ��</param>
        /// <returns></returns>
        public bool PhysicsableHit(int damage, AttackType attackType)
        {
            if (!m_IsAlive) return false;
            TypeToDamage(damage, attackType);

            if (m_CurrentHP <= 0)
            {
                Die();
                return true;
            }
            return false;
        }

        /// <summary>
        /// ���� Ÿ�Ժ��� ������ ����
        /// </summary>
        /// <param name="damage">���� ������</param>
        /// <param name="attackType">���� Ÿ��</param>
        private void TypeToDamage(int damage, AttackType attackType)
        {
            if (attackType == AttackType.Explosion) m_CurrentHP -= (damage / m_Settings.m_ExplosionResistance);
            else if (attackType == AttackType.Melee) m_CurrentHP -= (damage / m_Settings.m_MeleeResistance);
            else m_CurrentHP -= (damage - m_RealDef);
        }
        #endregion

        public void Die()
        {
            m_CurrentHP = 0;
            m_IsAlive = false;

            m_NormalMonsterAI.Dispose();
            if (!m_NormalMonsterAI.IsFalling) OnOffRagdoll(true);
            else m_CapsuleCollider.enabled = true;
            KilledNormalMonsterAction?.Invoke(m_Settings.m_MonsterType);
            Invoke(nameof(ReturnObject),10);
        }

        /// <summary>
        /// ������Ʈ Pool�� ��ȯ
        /// </summary>
        public override void ReturnObject()
        {
            EndNormalMonsterAction?.Invoke();
            m_PoolingObject.ReturnObject(this);
        }
    }
}
