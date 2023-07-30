using Scriptable.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.AI;
using PathCreation;

namespace Entity.Unit.Special
{
    public class SpecialMonster3 : MonoBehaviour, IMonster
    {
        [SerializeField] private SpecialMonster3Scriptable m_Setting;    

        private SP3AnimationController m_SP3AnimationController;
        private BoidsController m_BoidsController;
        private PlayerData m_PlayerData;
        private PathFollower m_PathFollower;
        private Rigidbody m_Rigidbody;

        private bool m_IsAlive;
        private bool m_IsRespawned;

        private int m_PlayerLayerNum;

        private int m_RealMaxHP;
        private int m_RespawnBoidsHP;
        private int m_RealDef;
        private int m_RealDamage;
        private int m_CurrentHP;

        private float m_AttackTimer;
        private float m_TraceAttackTimer;
        private float m_TraceBoidsTimer;
        private float m_PatrolBoidsTimer;

        private bool CanNormalAttack() => m_Setting.m_AttackSpeed <= m_AttackTimer && !DetectObstacle();
        public System.Action EndSpecialMonsterAction { get; set; }
        private bool DetectObstacle()
        {
            Vector3 dir = (AIManager.PlayerTransform.position - transform.position).normalized;

            bool isHit = Physics.Raycast(transform.position, dir, out RaycastHit hit, Mathf.Infinity, m_Setting.m_ObstacleDetectLayer);
            if (!isHit) return true;
            return hit.transform.gameObject.layer != m_PlayerLayerNum;
        }

        private void Awake()
        {
            m_SP3AnimationController = GetComponentInChildren<SP3AnimationController>();
            m_BoidsController = GetComponent<BoidsController>();
            m_PathFollower = GetComponent<PathFollower>();
            m_Rigidbody = GetComponent<Rigidbody>();
            
            m_PlayerLayerNum = LayerMask.NameToLayer("Player");
        }

        public void Init(PathCreator pathCreator, float statMultiplier)
        {
            m_PlayerData = AIManager.PlayerTransform.GetComponent<PlayerData>();

            m_BoidsController.Init();
            m_SP3AnimationController.Init();
            m_PathFollower.Init(pathCreator);

            SetRealStat(statMultiplier);
            m_BoidsController.GenerateBoidMonster(m_Setting.m_BoidsSpawnCount);
        }

        private void SetRealStat(float statMultiplier)
        {
            m_IsAlive = true;

            m_RealMaxHP = m_Setting.m_HP + (int)(statMultiplier * m_Setting.m_HPMultiplier);
            m_RealDef = m_Setting.m_Def + (int)(statMultiplier * m_Setting.m_DefMultiplier);
            m_RealDamage = m_Setting.m_Damage + (int)(statMultiplier * m_Setting.m_Damage);

            m_RespawnBoidsHP = (int)(m_RealMaxHP * 0.5f);
            m_CurrentHP = m_RealMaxHP;
        }

        private void Update()
        {
            if (!m_IsAlive) return;
            UpdateTimer();
            Move();
            m_BoidsController.Dispatch();
            Attack();
        }

        private void UpdateTimer()
        {
            m_AttackTimer += Time.deltaTime;
            m_TraceAttackTimer += Time.deltaTime;
            m_TraceBoidsTimer += Time.deltaTime;
            m_PatrolBoidsTimer += Time.deltaTime;
        }

        public void Attack()
        {
            if (CanNormalAttack())
            {
                m_AttackTimer = 0;
            }
            if (Input.GetKeyDown(KeyCode.O))
                StartCoroutine(m_BoidsController.TracePlayer());
            else if (Input.GetKeyDown(KeyCode.P))
                StartCoroutine(m_BoidsController.PatrolBoids());
            else if (Input.GetKeyDown(KeyCode.I))
                m_BoidsController.TraceAttack(true, 5);
        }

        public void Move()
        {
            m_PathFollower.FollowPath();
        }

        public void Hit(int damage, AttackType bulletType)
        {
            if (!m_IsAlive) return;

            int realDamage;
            if (bulletType == AttackType.Explosion) realDamage = 0;
            else if (bulletType == AttackType.Melee) realDamage = damage / m_Setting.m_MeleeResistance;
            else realDamage = damage - m_RealDef;

            m_CurrentHP -= realDamage;

            if (m_CurrentHP <= 0) Die();
            else if (!m_IsRespawned && m_CurrentHP <= m_RespawnBoidsHP) RespawnBoids();
        }

        private void RespawnBoids()
        {
            m_IsRespawned = true;
            m_BoidsController.GenerateBoidMonster(m_Setting.m_BoidsRespawnCount);
        }

        private IEnumerator RotateToDieMotion(float time)
        {
            m_IsAlive = false;
            
            m_BoidsController.Dispose();

            float elapsedTime = 0;
            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                transform.rotation = Quaternion.LookRotation(transform.forward, -Manager.GravityManager.GravityVector);
                yield return null;
            }

            m_Rigidbody.isKinematic = false;
            m_Rigidbody.useGravity = true;
            m_SP3AnimationController.Die();
            EndSpecialMonsterAction?.Invoke();
        }
        //
        public void Die()
        {
            StartCoroutine(RotateToDieMotion(1));
            //m_IsAlive = false;
            //m_Rigidbody.isKinematic = false;
            //m_Rigidbody.useGravity = true;

            //m_SP3AnimationController.Die();

            //m_BoidsController.Dispose();

            //EndSpecialMonsterAction?.Invoke();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (m_IsAlive) return;
            m_SP3AnimationController.DieHitGround();
            m_Rigidbody.isKinematic = true;
        }
    }
}
