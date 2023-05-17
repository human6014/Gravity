using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using EnumType;
using Manager.AI;
using System.Threading.Tasks;

namespace Entity.Unit.Special
{
    public class SpecialMonster1 : MonoBehaviour, IMonster
    {
        [Header("Stat")]
        [SerializeField] private Scriptable.Monster.SpecialMonsterScriptable m_Settings;

        [Header("Parabola")]
        [SerializeField] private LayerMask layerMask;


        [Header("Code")]
        [SerializeField] private LegController legController;

        [SerializeField] private Transform m_GrabPoint;
        [SerializeField] private Transform m_ThrowingPoint;
        [SerializeField] private Transform m_LookPoint;

        [SerializeField] private LayerMask m_AttackableLayer;

        private Transform m_NavMeshTransform;
        private SpecialMonsterAI m_SpecialMonsterAI;
        private SP1AnimationController m_SP1AnimationController;

        private Transform m_Target;
        private PlayerData m_PlayerData;
        private Parabola m_Parabola;

        private Vector3 m_GroundDirection;

        private const float m_JumpHeightRatio = 2;
        private const float m_JumpAttackHeightRatio = 12;

        private float m_AttackBetweenTime = 2;

        private float m_TargetDist;
        private float m_CurrentHP;

        private float m_NormalAttackTimer;
        private float m_GrabAttackTimer;
        private float m_JumpAttackTimer;
        private float m_RangeAttackTimer;
        private float m_JumpTimer;

        private bool m_IsAlive;
        private bool m_DoingBehaviour;

        private bool CanGrabAttack() => m_GrabAttackTimer >= m_Settings.m_GrabAttackSpeed &&
            m_TargetDist <= m_Settings.m_GrabAttackRange;

        private bool CanNormalAttack() => m_NormalAttackTimer >= m_Settings.m_AttackSpeed &&
            m_TargetDist <= m_Settings.m_AttackRange;

        //private bool CanSpitVenom() => !IsDetectObstacle(int.MaxValue);
        
        private bool CanJumpBiteAttack() => m_Settings.CanJumpBiteAttack(m_TargetDist, m_JumpAttackTimer) && 
            !IsDetectObstacle(m_Settings.m_JumpAttackMaxRange) && AIManager.PlayerIsGround;
        
        private bool CanJump() => m_Settings.CanJump(m_TargetDist, m_JumpTimer) &&
                 !IsDetectObstacle(m_Settings.m_JumpMaxRange) && AIManager.PlayerIsGround;
        
        private bool IsDetectObstacle(float maxRange)
        {
            Vector3 currentPos = m_SpecialMonsterAI.transform.position;
            Vector3 dir = (AIManager.PlayerTransform.position - currentPos).normalized;
            return Physics.Raycast(currentPos, dir, maxRange, layerMask);
        }

        public void Init(Quaternion rotation)
        {
            m_Target = AIManager.PlayerTransform;
            m_PlayerData = m_Target.GetComponent<PlayerData>();

            m_NavMeshTransform = transform.GetChild(0);
            m_SpecialMonsterAI = m_NavMeshTransform.GetComponent<SpecialMonsterAI>();
            m_SP1AnimationController = m_NavMeshTransform.GetComponent<SP1AnimationController>();

            m_Parabola = GetComponent<Parabola>();

            m_SpecialMonsterAI.Init(rotation);
            m_SP1AnimationController.Init();

            m_CurrentHP = m_Settings.m_HP;
            m_IsAlive = true;

            m_SP1AnimationController.SetWalk(true);
            m_PlayerData.GrabPoint(m_GrabPoint, m_LookPoint, m_ThrowingPoint);
        }

        private void FixedUpdate()
        {
            if (!m_IsAlive) return;

            Attack();
            if (!m_DoingBehaviour) Move(); 
        }

        private void Update()
        {
            if (!m_IsAlive) return;
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            m_NormalAttackTimer += Time.deltaTime;
            m_GrabAttackTimer += Time.deltaTime; 
            //m_RangeAttackTimer += Time.deltaTime;
            m_JumpAttackTimer += Time.deltaTime;
            m_JumpTimer += Time.deltaTime;
        }

        public void Move()
        {
            m_SpecialMonsterAI.OperateAIBehavior();
        }

        public void Attack()
        {
            if (m_DoingBehaviour || m_SpecialMonsterAI.GetIsOnOffMeshLink()) return;

            m_TargetDist = Vector3.Distance(AIManager.PlayerTransform.position, m_NavMeshTransform.position);
            if (CanJumpBiteAttack())
            {
                if (Random.Range(0, 100) > m_Settings.m_JumpAttackPercentage) m_JumpAttackTimer = 0;
                else JumpAttack();
            }
            else if (CanJump())
            {
                if (Random.Range(0, 100) > m_Settings.m_JumpPercentage) m_JumpTimer = 0;
                else Jump();
            }
            else if (CanGrabAttack()) GrabAttack();
            else if (CanNormalAttack()) NormalAttack();
        }

        private async void NormalAttack()
        {
            m_DoingBehaviour = true;
            m_NormalAttackTimer = 0;
            
            m_PlayerData.PlayerHit(m_NavMeshTransform, m_Settings.m_Damage, m_Settings.m_NoramlAttackType);
            await m_SP1AnimationController.SetClawsAttack();

            m_DoingBehaviour = false;
        }

        private async void GrabAttack()
        {
            m_DoingBehaviour = true;

            m_PlayerData.PlayerHit(m_GrabPoint, 0, m_Settings.m_GrabAttack);

            await m_SP1AnimationController.SetGrabAttack();

            m_PlayerData.PlayerHit(m_GrabPoint, m_Settings.m_GrabAttackDamage, m_Settings.m_NoramlAttackType);

            m_GrabAttackTimer = 0;
            m_NormalAttackTimer = m_Settings.m_AttackSpeed - m_AttackBetweenTime;
            m_PlayerData.EndGrab();
            m_DoingBehaviour = false;
        }


        public void Hit(int damage, AttackType bulletType)
        {
            if (!m_IsAlive) return;

            if (bulletType == AttackType.Explosion) m_CurrentHP -= (damage / m_Settings.m_ExplosionResistance);
            else if (bulletType == AttackType.Melee) m_CurrentHP -= (damage / m_Settings.m_MeleeResistance);
            else m_CurrentHP -= (damage - m_Settings.m_Def);

            if (m_CurrentHP <= 0) Die();
        }

        public void Die()
        {
            m_IsAlive = false;
            //StopAllCoroutines();
            m_SpecialMonsterAI.Dispose();
            m_PlayerData.EndGrab();
            m_SP1AnimationController.SetDie();
        }

        #region Parabola Related
        private void Jump()
        {
            Debug.Log("Jump");
            m_DoingBehaviour = true;
            m_JumpTimer = 0;

            float height = m_Parabola.GetHeight(m_Settings.m_JumpHeightRatio, m_Target, out Vector3 targetVector, ref m_GroundDirection);

            m_Parabola.CalculatePathWithHeight(targetVector, height, out float v0, out float angle, out float time);
            //StartCoroutine(JumpMovement(m_GroundDirection.normalized, v0, angle, time));

            TestJump(m_GroundDirection.normalized, v0, angle, m_Settings.m_PreJumpTime, time, false);
        }

        private void JumpAttack()
        {
            Debug.Log("JumpAttack");
            m_DoingBehaviour = true;
            m_JumpAttackTimer = 0;

            float height = m_Parabola.GetHeight(m_Settings.m_JumpAttackHeightRatio, m_Target, out Vector3 targetVector, ref m_GroundDirection, m_Settings.m_DestinationDist);

            m_Parabola.CalculatePathWithHeight(targetVector, height, out float v0, out float angle, out float time);
            //StartCoroutine(JumpAttackMovement(m_GroundDirection.normalized, v0, angle, time));
            TestJump(m_GroundDirection.normalized, v0, angle, m_Settings.m_PreJumpAttackTime, time, true);
        }

        #region 통합 테스트
        //이상함
        private async void TestJump(Vector3 direction, float v0,float angle, float preJumpTime, float jumpTime, bool hasAnimation)
        {
            m_DoingBehaviour = true;
            float upDist = hasAnimation ? 0.5f : 1;

            await PreJump(preJumpTime, upDist);
            if(hasAnimation) m_SP1AnimationController.SetJumpBiteAttack();
            await DoJump(direction, v0, angle, jumpTime, hasAnimation);

            
            if (hasAnimation)
            {
                m_GrabAttackTimer = 8;
                m_NormalAttackTimer = 3;

                CheckJumpAttackToPlayer();
            }
            m_DoingBehaviour = false;
        }

        private async Task PreJump(float preJumpTime, float upDist)
        {
            m_SpecialMonsterAI.SetNavMeshEnable(false);
            legController.SetPreJump(true);

            float elapsedTime = 0;
            Vector3 startPos = m_NavMeshTransform.position;

            while (elapsedTime < preJumpTime)
            {
                elapsedTime += Time.deltaTime;
                m_NavMeshTransform.position = Vector3.Lerp(startPos, startPos - m_NavMeshTransform.up * upDist, elapsedTime / preJumpTime);

                await Task.Yield();
            }
            legController.SetPreJump(false);
            legController.Jump(true);
        }

        private async Task DoJump(Vector3 direction, float v0, float angle, float time, bool hasAnimation)
        {
            float elapsedTime = 0;
            Vector3 startPos = m_NavMeshTransform.position;
            Quaternion startRotation = m_NavMeshTransform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(m_GroundDirection, m_Target.up);
            float xAngle = Mathf.Cos(angle);
            float yAngle = Mathf.Sin(angle);
            bool isPlayEndAnimation = false;

            await Task.Yield();

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime * 2f;

                float x = v0 * elapsedTime * xAngle;
                float y = v0 * elapsedTime * yAngle - 0.5f * Physics.gravity.magnitude * Mathf.Pow(elapsedTime, 2);
                m_NavMeshTransform.SetPositionAndRotation(startPos + direction * x - GravityManager.GravityVector * y,
                                 Quaternion.Lerp(startRotation, targetRot, elapsedTime / time));

                if (hasAnimation && time - elapsedTime <= 0.3f && !isPlayEndAnimation)
                {
                    isPlayEndAnimation = true;
                    m_SP1AnimationController.EndJumpBiteAttack();
                }

                await Task.Yield();
            }

            legController.Jump(false);
            m_DoingBehaviour = false;
            m_SpecialMonsterAI.SetNavMeshEnable(true);
        }

        #endregion

        private IEnumerator JumpMovement(Vector3 direction, float v0, float angle, float time)
        {
            //m_SP1AnimationController.SetWalk(false);
            m_SpecialMonsterAI.SetNavMeshEnable(false);
            legController.SetPreJump(true);

            float elapsedTime = 0;
            Vector3 startPos = m_NavMeshTransform.position;
            while (elapsedTime < m_Settings.m_PreJumpTime)
            {
                elapsedTime += Time.deltaTime;
                m_NavMeshTransform.position = Vector3.Lerp(startPos, startPos - m_NavMeshTransform.up, elapsedTime / m_Settings.m_PreJumpTime);
                
                yield return null;
            }

            legController.SetPreJump(false);
            legController.Jump(true);

            elapsedTime = 0;
            startPos = m_NavMeshTransform.position;
            Quaternion startRotation = m_NavMeshTransform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(m_GroundDirection, m_Target.up);
            float xAngle = Mathf.Cos(angle);
            float yAngle = Mathf.Sin(angle);

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime * 2f;

                float x = v0 * elapsedTime * xAngle;
                float y = v0 * elapsedTime * yAngle - 0.5f * Physics.gravity.magnitude * Mathf.Pow(elapsedTime, 2);
                m_NavMeshTransform.SetPositionAndRotation(startPos + direction * x - GravityManager.GravityVector * y,
                                 Quaternion.Lerp(startRotation, targetRot, elapsedTime / time));
                //elapsedTime += Time.deltaTime * (time / 1.5f);
                //elapsedTime += Time.deltaTime;
                
                yield return null;
            }
            legController.Jump(false);
            m_DoingBehaviour = false;
            m_SpecialMonsterAI.SetNavMeshEnable(true);
        }

        private IEnumerator JumpAttackMovement(Vector3 direction, float v0, float angle, float time)
        {
            m_SpecialMonsterAI.SetNavMeshEnable(false);
            legController.SetPreJump(true);

            float elapsedTime = 0;
            Vector3 startPos = m_NavMeshTransform.position;
            while (elapsedTime < m_Settings.m_PreJumpAttackTime)
            {
                elapsedTime += Time.deltaTime;
                m_NavMeshTransform.position = Vector3.Lerp(startPos, startPos - m_NavMeshTransform.up * 0.5f, elapsedTime / m_Settings.m_PreJumpAttackTime);

                yield return null;
            }

            legController.SetPreJump(false);
            legController.Jump(true);
            m_SP1AnimationController.SetJumpBiteAttack();

            elapsedTime = 0;
            startPos = m_NavMeshTransform.position;
            Quaternion startRotation = m_NavMeshTransform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(m_GroundDirection, m_Target.up);
            float xAngle = Mathf.Cos(angle);
            float yAngle = Mathf.Sin(angle);
            bool isPlayEndAnimation = false;

            Debug.Log(time);
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime * 2;

                float x = v0 * elapsedTime * xAngle;
                float y = v0 * elapsedTime * yAngle - 0.5f * Physics.gravity.magnitude * Mathf.Pow(elapsedTime, 2);
                m_NavMeshTransform.SetPositionAndRotation(startPos + direction * x - GravityManager.GravityVector * y,
                                 Quaternion.Lerp(startRotation, targetRot, elapsedTime / time));
                //elapsedTime += Time.deltaTime * (time / 1.5f);
                //elapsedTime += Time.deltaTime;
                if (time - elapsedTime <= 0.4f && !isPlayEndAnimation)
                {
                    isPlayEndAnimation = true;
                    m_SP1AnimationController.EndJumpBiteAttack();
                }

                yield return null;
            }
            legController.Jump(false);
            m_DoingBehaviour = false;
            m_SpecialMonsterAI.SetNavMeshEnable(true);

            m_GrabAttackTimer = 8;
            m_NormalAttackTimer = 3;

            CheckJumpAttackToPlayer();
        }

        private void CheckJumpAttackToPlayer()
        {
            if (Physics.CheckSphere(m_GrabPoint.position, m_Settings.m_JumpAttackRange, m_AttackableLayer, QueryTriggerInteraction.Ignore))
                m_PlayerData.PlayerHit(m_NavMeshTransform, m_Settings.m_JumpAttackDamage, m_Settings.m_NoramlAttackType);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_GrabPoint.position,m_Settings.m_JumpAttackRange);
        }
        #endregion
    }
}
