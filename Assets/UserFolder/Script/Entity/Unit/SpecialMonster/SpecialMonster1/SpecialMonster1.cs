using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using EnumType;

namespace Entity.Unit.Special
{
    public class SpecialMonster1 : MonoBehaviour, IMonster
    {
        [Header("Stat")]
        [SerializeField] private Scriptable.Monster.SpecialMonsterScriptable m_settings;

        [Header("Parabola")]
        [SerializeField] private float _step = 0.01f;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private LayerMask layerMask;
        //[SerializeField] float height = 10;

        [Header("Code")]
        [SerializeField] private Transform aiTransform;
        [SerializeField] private LegController legController;

        [SerializeField] private Transform m_GrabPoint;
        [SerializeField] private Transform m_ThrowingPoint;
        [SerializeField] private Transform m_LookPoint;

        [Header("Animation")]
        [SerializeField] private SP1AnimationController m_AnimationController;

        private Transform m_Target;
        private SpecialMonsterAI m_SpecialMonsterAI;
        private PlayerData m_PlayerData;

        private Vector3 m_GroundDirection;
        private Vector3 m_TargetPos;
        private Vector3 m_Direction;

        private float m_TargetDist;
        private float m_CurrentHP;
        private float m_AttackTimer;

        private bool m_IsAlive;
        private bool m_IsPreJump;

        private bool CanAttack() => CanBiteAttack() || CanCrawsAttack();

        private bool CanBiteAttack() => m_AttackTimer >= m_settings.m_AttackSpeed &&
            m_TargetDist <= m_settings.m_AttackRange;

        private bool CanCrawsAttack() => m_AttackTimer >= m_settings.m_AttackSpeed &&
            m_TargetDist <= m_settings.HeavyAttackRange;

        private void Awake()
        {
            m_SpecialMonsterAI = aiTransform.GetComponent<SpecialMonsterAI>();
        }

        public void Init(Quaternion rotation)
        {
            m_Target = Manager.AI.AIManager.PlayerTransform;
            m_PlayerData = m_Target.GetComponent<PlayerData>();
            m_SpecialMonsterAI.Init(rotation);
            m_CurrentHP = m_settings.m_HP;
            m_IsAlive = true;

            m_PlayerData.GrabPoint(m_GrabPoint, m_LookPoint, m_ThrowingPoint);
        }

        private void FixedUpdate()
        {
            if (!m_IsAlive) return;
            m_AttackTimer += Time.deltaTime;
            m_TargetDist = Vector3.Distance(Manager.AI.AIManager.PlayerTransform.position, aiTransform.position);
            if (CanAttack()) Attack();
            else if(m_AnimationController.CanMoving()) m_SpecialMonsterAI.OperateAIBehavior();
        }

        public void Move()
        {
            
        }

        public void Attack()
        {
            if (CanCrawsAttack()) BiteAttack();
            else if (CanBiteAttack()) CrawsAttack();
        }

        public async void BiteAttack()
        {
            m_AttackTimer = 0;

            m_PlayerData.PlayerHit(m_GrabPoint, m_settings.m_Damage, m_settings.m_GrabAttack);
            
            await m_AnimationController.SetBiteAttack();

            m_PlayerData.EndGrab();
        }

        public void CrawsAttack()
        {
            m_AttackTimer = 0;

            m_PlayerData.PlayerHit(aiTransform, m_settings.m_Damage, m_settings.m_NoramlAttackType);
            m_AnimationController.SetClawsAttack();
        }


        public void Hit(int damage, AttackType bulletType)
        {
            if (!m_IsAlive) return;

            if (bulletType == AttackType.Explosion) m_CurrentHP -= (damage / m_settings.m_ExplosionResistance);
            else m_CurrentHP -= (damage - m_settings.m_Def);

            if (m_CurrentHP <= 0) Die();
        }

        public void Die()
        {
            m_IsAlive = false;
            m_PlayerData.EndGrab();
            m_AnimationController.SetDie();
        }

        private void Update()
        {
            if (!m_Target || !m_IsAlive) return;

            m_Direction = m_Target.position - aiTransform.position;
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
            m_TargetPos = new(m_GroundDirection.magnitude, dir * -GravityManager.GravityDirectionValue, 0);
            float height = Mathf.Max(0.01f, m_TargetPos.y + m_TargetPos.magnitude / 2f);

            CalculatePathWithHeight(m_TargetPos, height, out float v0, out float angle, out float time);
            t_v0 = v0;
            t_angle = angle;
            t_time = time;
            //DrawPath(groundDirection.normalized, v0, angle, time, _step); //경로 그리기

            if (Input.GetKeyDown(KeyCode.Backspace) && !m_IsPreJump && !m_SpecialMonsterAI.GetIsOnOffMeshLink())
                StartCoroutine(Coroutine_Movement(m_GroundDirection.normalized, v0, angle, time));
        }


        float t_v0;
        float t_angle;
        float t_time;
        [ContextMenu("ExcuteJump")]
        public void ExcuteJump()
        {
            if (!m_IsPreJump && !m_SpecialMonsterAI.GetIsOnOffMeshLink())
            {
                StartCoroutine(Coroutine_Movement(m_GroundDirection.normalized, t_v0, t_angle, t_time));
            }
        }

        public void StartJumpCoroutine()
        {
            //StartCoroutine(Jump(0.9f));
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

        IEnumerator Coroutine_Movement(Vector3 direction, float v0, float angle, float time)
        {
            m_SpecialMonsterAI.SetNavMeshEnable(false);
            m_IsPreJump = true;
            legController.SetPreJump(true);

            float beforeJumpingTime = 1f;
            float elapsedTime = 0;
            Vector3 startPos = aiTransform.position;
            while (elapsedTime < beforeJumpingTime)
            {
                aiTransform.position = Vector3.Lerp(startPos, startPos - aiTransform.up, elapsedTime / beforeJumpingTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            legController.SetPreJump(false);
            legController.Jump(true);
            Debug.Log("Coroutine target : " + m_Target);
            elapsedTime = 0;
            startPos = aiTransform.position;
            Quaternion startRotation = aiTransform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(m_GroundDirection, m_Target.up);
            Debug.Log("time : " + time);
            while (elapsedTime < time)
            {
                float x = v0 * elapsedTime * Mathf.Cos(angle);
                float y = v0 * elapsedTime * Mathf.Sin(angle) - 0.5f * Physics.gravity.magnitude * Mathf.Pow(elapsedTime, 2);
                aiTransform.SetPositionAndRotation(startPos + direction * x - GravityManager.GravityVector * y,
                                 Quaternion.Lerp(startRotation, targetRot, elapsedTime / time));
                //elapsedTime += Time.deltaTime * (time / 1.5f);
                //elapsedTime += Time.deltaTime;
                elapsedTime += Time.deltaTime * 2.5f;
                yield return null;
            }
            legController.Jump(false);
            m_IsPreJump = false;
            m_SpecialMonsterAI.SetNavMeshEnable(true);
            //specialMonsterAI.SetNavMeshPos(transform.position);
        }
        #endregion
    }
}
