using Scriptable.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.AI;
using PathCreation;
using Manager;

namespace Entity.Unit.Special
{
    public class SpecialMonster3 : MonoBehaviour, IMonster
    {
        [SerializeField] private SpecialMonster3Scriptable m_Setting;
        [SerializeField] private PoisonSphere m_PoisonSphere;
        [SerializeField] private Transform m_AttackStartPoint;

        private WaitForSeconds m_NormalAttackWait;
        private ObjectPoolManager.PoolingObject m_PollingObject;
        private SP3AnimationController m_SP3AnimationController;
        //private BoidsController m_BoidsController;
        private BoidsContollerCPU m_BoidsController;
        private ParticleEndSystem m_ParticleEndSystem;
        private PathFollower m_PathFollower;
        private Rigidbody m_Rigidbody;

        private bool m_IsAlive;
        private bool m_IsRespawned;
        private bool m_CanMove = true;

        private int m_RealMaxHP;
        private int m_RespawnBoidsHP;
        private int m_RealDef;
        private int m_RealDamage;
        private int m_CurrentHP;

        private float m_AttackTimer;
        private float m_TraceAttackTimer;
        private float m_TraceAndBackTimer;
        private float m_PatrolBoidsTimer;

        private readonly float m_PatternBetweenTime = 7;
        private readonly float m_NormalAttackRotateTime = 1;
        private readonly float m_DieRotateTime = 0.75f;

        private bool CanPatrolBoids() => 
            m_Setting.CanBoidsPatrolTime(m_PatrolBoidsTimer) && 
            !m_BoidsController.IsTraceAndBackPlayer;
        //확률 따로 처리
        private bool CanTraceAndBack() => 
            m_Setting.CanBoidsTraceAndBackTime(m_TraceAndBackTimer) && 
            !m_BoidsController.IsPatrolBoids;

        private bool CanTraceAttack() => 
            m_Setting.m_BoidsMonsterAttackSpeed <= m_TraceAttackTimer &&
            !m_BoidsController.IsTraceAndBackPlayer &&
            !m_BoidsController.IsPatrolBoids;

        private bool CanNormalAttack() => m_Setting.m_AttackSpeed <= m_AttackTimer;

        public System.Action EndSpecialMonsterAction { get; set; }

        private bool DetectObstacle()
        {
            Vector3 dir = (AIManager.PlayerTransform.position - transform.position).normalized;

            bool isHit = Physics.Raycast(transform.position, dir, out RaycastHit hit, Mathf.Infinity, m_Setting.m_ObstacleDetectLayer);
            if (!isHit) return true;
            return hit.transform.gameObject.layer != AIManager.PlayerLayerNum;
        }

        private void Awake()
        {
            m_SP3AnimationController = GetComponentInChildren<SP3AnimationController>();
            m_ParticleEndSystem = GetComponentInChildren<ParticleEndSystem>();
            //m_BoidsController = GetComponent<BoidsController>();
            m_BoidsController = GetComponent<BoidsContollerCPU>();
            m_PathFollower = GetComponent<PathFollower>();
            m_Rigidbody = GetComponent<Rigidbody>();

            m_NormalAttackWait = new WaitForSeconds(1);
        }

        public void Init(PathCreator pathCreator, float statMultiplier)
        {
            m_BoidsController.Init(m_Setting.m_BoidsMonsterTraceTime,m_Setting.m_BoidsPatrolTime);
            m_SP3AnimationController.Init();
            m_PathFollower.Init(pathCreator);

            m_BoidsController.ReturnChildObject += (int HP) => Hit(HP, AttackType.None);
            m_SP3AnimationController.EndDieHitGroundAnimation += () => m_Rigidbody.isKinematic = true;

            SetRealStat(statMultiplier);

            m_PollingObject = ObjectPoolManager.Register(m_PoisonSphere, GameObject.Find("ActiveObjectPool").transform);
            m_PollingObject.GenerateObj(3);

            m_CurrentHP += m_BoidsController.GenerateBoidMonster(m_Setting.m_BoidsSpawnCount);
            
            m_RespawnBoidsHP = (int)(m_CurrentHP * 0.3f);
        }

        private void SetRealStat(float statMultiplier)
        {
            m_IsAlive = true;

            m_RealMaxHP = m_Setting.m_HP + (int)(statMultiplier * m_Setting.m_HPMultiplier);
            m_RealDef = m_Setting.m_Def + (int)(statMultiplier * m_Setting.m_DefMultiplier);
            m_RealDamage = m_Setting.m_Damage + (int)(statMultiplier * m_Setting.m_Damage);

            m_CurrentHP = m_RealMaxHP;
        }

        private void Update()
        {
            if (!m_IsAlive) return;
            UpdateTimer();
            Attack();
            if(m_CanMove) Move();
            //m_BoidsController.BoidsDispatch();
        }

        private void UpdateTimer()
        {
            m_AttackTimer += Time.deltaTime;
            m_TraceAttackTimer += Time.deltaTime;
            m_TraceAndBackTimer += Time.deltaTime;
            m_PatrolBoidsTimer += Time.deltaTime;
        }

        public void Attack()
        {
            if (!DetectObstacle())
            {
                if (CanNormalAttack())
                {
                    m_AttackTimer = 0;
                    NormalAttack();
                }
                else if (CanTraceAndBack())
                {
                    if (m_Setting.CanBoidsTraceAndBackPercentage()) m_BoidsController.StartTraceAndBackPlayer();
                    m_TraceAndBackTimer = 0;
                    m_PatrolBoidsTimer = Mathf.Min(m_PatrolBoidsTimer, m_Setting.m_BoidsPatrolTime - m_PatternBetweenTime);
                    m_TraceAttackTimer = Mathf.Min(m_TraceAttackTimer, m_Setting.m_BoidsMonsterTraceTime - m_PatternBetweenTime);
                }
            }
            if (CanPatrolBoids())
            {
                if (m_Setting.CanBoidsPatrolPercentage()) m_BoidsController.StartPatrolBoids();
                m_PatrolBoidsTimer = 0;
                m_TraceAndBackTimer = Mathf.Min(m_TraceAndBackTimer, m_Setting.m_BoidsMonsterTraceAndBackSpeed - m_PatternBetweenTime);
                m_TraceAttackTimer = Mathf.Min(m_TraceAttackTimer, m_Setting.m_BoidsMonsterTraceTime - m_PatternBetweenTime);
            }
            else if (CanTraceAttack())
            {
                m_TraceAttackTimer = 0;
                m_BoidsController.TraceAttack(true);
            }
        }

        private void NormalAttack()
        {
            m_CanMove = false;
            StartCoroutine(NormalAttackCoroutine(m_NormalAttackRotateTime));
        }

        private IEnumerator NormalAttackCoroutine(float time)
        {
            float elapsedTime = 0;
            Quaternion startRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation((AIManager.PlayerTransform.position - transform.position).normalized);
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / time);
                yield return null;
            }
 
            PoisonSphere poisonSphere = (PoisonSphere)m_PollingObject.GetObject(false);
            poisonSphere.Init(m_PollingObject, m_AttackStartPoint.position, transform.rotation, m_RealDamage);
            poisonSphere.gameObject.SetActive(true);
            m_SP3AnimationController.Attack();

            yield return m_NormalAttackWait;
            elapsedTime = 0;
            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(targetRotation, startRotation, elapsedTime / time);
                yield return null;
            }

            m_CanMove = true;
        }

        public void Move()
        {
            m_PathFollower.FollowPath(m_Setting.m_MovementSpeed);
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
            m_CurrentHP += m_BoidsController.GenerateBoidMonster(m_Setting.m_BoidsRespawnCount);
        }

        private IEnumerator RotateToDieMotion(float time)
        {
            float elapsedTime = 0;
            Quaternion startRotation = transform.rotation;
            Vector3 projectedVector = Vector3.ProjectOnPlane(transform.forward, GravityManager.GetCurrentGravityNormalDirection()).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(projectedVector, -GravityManager.GravityVector);
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / time);
                yield return null;
            }

            m_Rigidbody.isKinematic = false;
            m_Rigidbody.useGravity = true;
            m_SP3AnimationController.Die();
            EndSpecialMonsterAction?.Invoke();
        }

        public void Die()
        {
            m_IsAlive = false;
            m_BoidsController.Dispose();
            m_ParticleEndSystem.TurnOffParticles();
            StartCoroutine(RotateToDieMotion(m_DieRotateTime));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (m_IsAlive) return;
            m_SP3AnimationController.DieHitGround();
        }
    }
}
