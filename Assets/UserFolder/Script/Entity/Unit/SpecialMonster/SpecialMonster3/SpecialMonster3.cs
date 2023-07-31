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
        
        private ObjectPoolManager.PoolingObject m_PollingObject;
        private SP3AnimationController m_SP3AnimationController;
        private BoidsController m_BoidsController;
        private PathFollower m_PathFollower;
        private Rigidbody m_Rigidbody;

        private bool m_IsAlive;
        private bool m_IsRespawned;
        private bool m_CanMove = true;

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

        
        private bool CanNormalAttack() => m_Setting.m_AttackSpeed <= m_AttackTimer;
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
            m_BoidsController.Init();
            m_SP3AnimationController.Init();
            m_PathFollower.Init(pathCreator);

            m_SP3AnimationController.EndDieHitGroundAnimation += () => m_Rigidbody.isKinematic = true;

            SetRealStat(statMultiplier);

            m_PollingObject = ObjectPoolManager.Register(m_PoisonSphere, GameObject.Find("ActiveObjectPool").transform);
            m_PollingObject.GenerateObj(3);

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
            Attack();
            if(m_CanMove) Move();
            m_BoidsController.Dispatch();
        }
        #region

        #endregion
        private void UpdateTimer()
        {
            m_AttackTimer += Time.deltaTime;
            m_TraceAttackTimer += Time.deltaTime;
            m_TraceBoidsTimer += Time.deltaTime;
            m_PatrolBoidsTimer += Time.deltaTime;
        }

        public void Attack()
        {
            if (!DetectObstacle())
            {
                //if (CanNormalAttack())
                    //NormalAttack();
                if (Input.GetKeyDown(KeyCode.U))
                    NormalAttack();
                if (Input.GetKeyDown(KeyCode.O))
                    StartCoroutine(m_BoidsController.TracePlayer());
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.P))
                    StartCoroutine(m_BoidsController.PatrolBoids());
                else if (Input.GetKeyDown(KeyCode.I))
                    m_BoidsController.TraceAttack(true, 5);
            }
        }

        private void NormalAttack()
        {
            m_AttackTimer = 0;
            m_CanMove = false;
            StartCoroutine(NormalAttackCoroutine(1.5f));
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

            yield return new WaitForSeconds(1);
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
            StartCoroutine(RotateToDieMotion(1));
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (m_IsAlive) return;
            m_SP3AnimationController.DieHitGround();
        }
    }
}
