using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using EnumType;
using Manager.AI;

namespace Entity.Unit.Special
{
    public class SpecialMonster1 : MonoBehaviour, IMonster
    {
        [Header("Stat")]
        [SerializeField] private Scriptable.Monster.SpecialMonsterScriptable m_Settings;

        [Header("Parabola")]
        [SerializeField] private float _step = 0.01f;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private LayerMask layerMask;
        //[SerializeField] float height = 10;

        [Header("Code")]
        [SerializeField] private LegController legController;
        [SerializeField] private Transform aiTransform;

        [SerializeField] private Transform m_GrabPoint;
        [SerializeField] private Transform m_ThrowingPoint;
        [SerializeField] private Transform m_LookPoint;

        [SerializeField] private LayerMask m_AttackableLayer;

        [Header("Animation")]
        [SerializeField] private SP1AnimationController m_AnimationController;

        private Transform m_Target;
        private SpecialMonsterAI m_SpecialMonsterAI;
        private PlayerData m_PlayerData;

        private Vector3 m_GroundDirection;

        private const float m_PreJumpingTime = 2f;
        private const float m_PreJumpAttackTime = 0.6f;

        private const float m_JumpHeightRatio = 2;
        private const float m_JumpAttackHeightRatio = 12;
        private const float m_DestinationDist = 2;

        private float m_JumpPercentage = 60;                //행동을 수행할 확률 
        private float m_JumpAttackPercentage = 70;          //ex) 70퍼 확률로 점프 공격 수행

        private float m_AttackBetweenTime = 2;

        //private float m_DefaultAttackCoolTime = 3;
        //private float m_DefaultAttackTimer;

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
            m_SpecialMonsterAI = aiTransform.GetComponent<SpecialMonsterAI>();

            m_SpecialMonsterAI.Init(rotation);
            m_AnimationController.Init();

            m_CurrentHP = m_Settings.m_HP;
            m_IsAlive = true;

            m_AnimationController.SetIdle(true);
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

            m_TargetDist = Vector3.Distance(AIManager.PlayerTransform.position, aiTransform.position);
            if (CanJumpBiteAttack())
            {
                if (Random.Range(0, 100) > m_JumpAttackPercentage)
                {
                    m_JumpAttackTimer = 0;
                    Debug.Log("점프 공격 실패");
                }
                else JumpBiteAttack();
            }
            else if (CanJump())
            {
                if (Random.Range(0, 100) > m_JumpPercentage)
                {
                    m_JumpTimer = 0;
                    Debug.Log("점프 실패");
                }
                else Jump();
            }
            else if (CanGrabAttack()) GrabAttack();
            else if (CanNormalAttack()) NormalAttack();
            
        }

        private async void NormalAttack()
        {
            m_DoingBehaviour = true;
            m_NormalAttackTimer = 0;
            
            m_PlayerData.PlayerHit(aiTransform, m_Settings.m_Damage, m_Settings.m_NoramlAttackType);
            await m_AnimationController.SetClawsAttack();

            m_DoingBehaviour = false;
        }

        private async void GrabAttack()
        {
            m_DoingBehaviour = true;

            m_PlayerData.PlayerHit(m_GrabPoint, 0, m_Settings.m_GrabAttack);

            await m_AnimationController.SetGrabAttack();

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
            m_AnimationController.SetDie();
        }

        private void Jump()
        {
            m_DoingBehaviour = true;
            m_JumpTimer = 0;

            float height = GetHeight(m_JumpHeightRatio, out Vector3 targetVector);

            CalculatePathWithHeight(targetVector, height, out float v0, out float angle, out float time);
            StartCoroutine(ParabolaMovement(m_GroundDirection.normalized, v0, angle, time));
        }

        private void JumpBiteAttack()
        {
            m_DoingBehaviour = true;
            m_JumpAttackTimer = 0;

            float height = GetHeight(m_JumpAttackHeightRatio, out Vector3 targetVector, m_DestinationDist);

            CalculatePathWithHeight(targetVector, height, out float v0, out float angle, out float time);
            StartCoroutine(JumpAttackMovement(m_GroundDirection.normalized, v0, angle, time));
        }

        private float GetHeight(float heightRatio, out Vector3 targetVector, float destinationDist = 0)
        {
            Vector3 m_Direction = m_Target.position - aiTransform.position;
            float dir = 0;
            switch (GravityManager.currentGravityType)
            {
                case GravityType.xUp:
                case GravityType.xDown:
                    m_GroundDirection = new(0, m_Direction.y, m_Direction.z);
                    dir = m_Direction.x;
                    break;

                case GravityType.yUp: //Init
                case GravityType.yDown:
                    m_GroundDirection = new(m_Direction.x, 0, m_Direction.z);
                    dir = m_Direction.y;
                    break;

                case GravityType.zUp:
                case GravityType.zDown:
                    m_GroundDirection = new(m_Direction.x, m_Direction.y, 0);
                    dir = m_Direction.z;
                    break;
            }
            targetVector = new(m_GroundDirection.magnitude - destinationDist, dir * -GravityManager.GravityDirectionValue, 0);
            return Mathf.Max(0.01f, targetVector.y + targetVector.magnitude / heightRatio);
        }

        #region 포물선 테스트중

#if UNITY_EDITOR
        private void DrawPath(Vector3 direction, float v0, float angle, float time, float step)
        {
            step = Mathf.Max(0.01f, step);
            lineRenderer.positionCount = (int)(time / step) + 2;
            int count = 0;
            for (float i = 0; i < time; i += step)
            {
                float x = v0 * i * Mathf.Cos(angle);
                float y = v0 * i * Mathf.Sin(angle) - 0.5f * Physics.gravity.magnitude * Mathf.Pow(i, 2);
                lineRenderer.SetPosition(count, aiTransform.position + direction * x - GravityManager.GravityVector * y);

                count++;
            }
            float xFinal = v0 * time * Mathf.Cos(angle);
            float yFinal = v0 * time * Mathf.Sin(angle) - 0.5f * Physics.gravity.magnitude * Mathf.Pow(time, 2);
            lineRenderer.SetPosition(count, aiTransform.position + direction * xFinal - GravityManager.GravityVector * yFinal);
        }
#endif

        private float QuadraticEquation(float a, float b, float c, float sign) =>
            (-b + sign * Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

        private void CalculatePathWithHeight(Vector2 targetPos, float h, out float v0, out float angle, out float time)
        {
            float g = Physics.gravity.magnitude;

            float a = (-0.5f * g);
            float b = Mathf.Sqrt(2 * g * h);
            float c = -targetPos.y;

            float tplus = QuadraticEquation(a, b, c, 1);
            float tmin = QuadraticEquation(a, b, c, -1);

            time = tplus > tmin ? tplus : tmin;
            angle = Mathf.Atan(b * time / targetPos.x);
            v0 = b / Mathf.Sin(angle);
        }

        private IEnumerator ParabolaMovement(Vector3 direction, float v0, float angle, float time)
        {
            m_SpecialMonsterAI.SetNavMeshEnable(false);
            legController.SetPreJump(true);

            float elapsedTime = 0;
            Vector3 startPos = aiTransform.position;
            while (elapsedTime < m_PreJumpingTime)
            {
                elapsedTime += Time.deltaTime;
                aiTransform.position = Vector3.Lerp(startPos, startPos - aiTransform.up, elapsedTime / m_PreJumpingTime);
                
                yield return null;
            }

            legController.SetPreJump(false);
            legController.Jump(true);

            elapsedTime = 0;
            startPos = aiTransform.position;
            Quaternion startRotation = aiTransform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(m_GroundDirection, m_Target.up);
            float xAngle = Mathf.Cos(angle);
            float yAngle = Mathf.Sin(angle);

            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime * 2f;

                float x = v0 * elapsedTime * xAngle;
                float y = v0 * elapsedTime * yAngle - 0.5f * Physics.gravity.magnitude * Mathf.Pow(elapsedTime, 2);
                aiTransform.SetPositionAndRotation(startPos + direction * x - GravityManager.GravityVector * y,
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
            Vector3 startPos = aiTransform.position;
            while (elapsedTime < m_PreJumpAttackTime)
            {
                elapsedTime += Time.deltaTime;
                aiTransform.position = Vector3.Lerp(startPos, startPos - aiTransform.up * 0.5f, elapsedTime / m_PreJumpAttackTime);

                yield return null;
            }

            legController.SetPreJump(false);
            legController.Jump(true);
            m_AnimationController.SetJumpBiteAttack();

            elapsedTime = 0;
            startPos = aiTransform.position;
            bool isPlayEndAnimation = false;
            Quaternion startRotation = aiTransform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(m_GroundDirection, m_Target.up);

            float xAngle = Mathf.Cos(angle);
            float yAngle = Mathf.Sin(angle);
            while (elapsedTime < time)
            {
                elapsedTime += Time.deltaTime * 2;

                float x = v0 * elapsedTime * xAngle;
                float y = v0 * elapsedTime * yAngle - 0.5f * Physics.gravity.magnitude * Mathf.Pow(elapsedTime, 2);
                aiTransform.SetPositionAndRotation(startPos + direction * x - GravityManager.GravityVector * y,
                                 Quaternion.Lerp(startRotation, targetRot, elapsedTime / time));
                //elapsedTime += Time.deltaTime * (time / 1.5f);
                //elapsedTime += Time.deltaTime;
                if (time - elapsedTime <= 0.3f && !isPlayEndAnimation)
                {
                    isPlayEndAnimation = true;
                    m_AnimationController.EndJumpBiteAttack();
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
            //왜 안되냐고
            Debug.Log("CheckCollider");
            if (Physics.CheckSphere(m_GrabPoint.position, 5, m_AttackableLayer, QueryTriggerInteraction.Ignore))
            {
                Debug.Log("Hit");
                m_PlayerData.PlayerHit(aiTransform, m_Settings.m_JumpAttackDamage, m_Settings.m_NoramlAttackType);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(m_GrabPoint.position,m_Settings.m_JumpAttackRange);
        }
        #endregion
    }
}
