using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using Manager.AI;
using System.Threading.Tasks;

namespace Entity.Unit.Special
{
    public class SpecialMonster1 : MonoBehaviour, IMonster
    {
        [Header("Data")]
        [SerializeField] private Scriptable.Monster.SpecialMonsterScriptable m_Settings;

        [Header("Model")]
        [SerializeField] private SkinnedMeshRenderer m_SkinnedMeshRenderer;

        [Header("GrabPoint")]
        [SerializeField] private Transform m_GrabCameraPoint;
        [SerializeField] private Transform m_GrabBodyPoint;
        [SerializeField] private Transform m_ThrowingPoint;
        [SerializeField] private Transform m_LookPoint;

        private Transform m_NavMeshTransform;   //transform말고 이거 써야대
        private SpecialMonsterAI m_SpecialMonsterAI;
        private SP1AnimationController m_SP1AnimationController;
        private MaterialPropertyBlock m_MaterialPropertyBlock;
        private LegController m_LegController;
        private PlayerData m_PlayerData;
        private Controller.Player.Utility.PlayerShakeController m_PlayerShakeController;
        private Parabola m_Parabola;

        private Vector3 m_GroundDirection;

        private readonly float m_AttackBetweenTime = 3;
        private float m_TargetDist;
        private float m_CurrentHP;

        private float m_NormalAttackTimer;
        private float m_GrabAttackTimer;
        private float m_JumpAttackTimer;
        private float m_JumpTimer;

        private bool m_IsAlive;
        private bool m_DoingBehaviour;
        private bool m_IsGrabbing;

        private int m_RealMaxHP;
        private int m_RealDef;
        private int m_RealCriticalHP;
        private int m_RealDamage;

        public System.Action EndSpecialMonsterAction { get; set; }

        private bool CanGrabAttack(float angle) 
            => m_Settings.CanGrabAttack(m_TargetDist, m_GrabAttackTimer, angle);

        private bool CanNormalAttack(float angle)
            => m_Settings.CanNormalAttack(m_TargetDist, m_NormalAttackTimer, angle);

        private bool CanJumpAttack() => m_Settings.CanJumpAttack(m_TargetDist, m_JumpAttackTimer) && 
            AIManager.PlayerIsGround;
        
        private bool CanJump() => m_Settings.CanJump(m_TargetDist, m_JumpTimer) &&
            AIManager.PlayerIsGround;

        private bool CanCriticalHit(int realDamage) => realDamage >= m_Settings.m_HitDamage && !m_DoingBehaviour &&
            m_CurrentHP <= m_RealCriticalHP && Random.Range(0, 100) <= m_Settings.m_HitPercentage;

        private bool DetectObstacle()
        {
            Vector3 currentPos = m_NavMeshTransform.position;
            Vector3 dir = (AIManager.PlayerTransform.position - currentPos).normalized;

            bool isHit = Physics.Raycast(currentPos, dir, out RaycastHit hit, m_TargetDist, m_Settings.m_ObstacleDetectLayer);

            if (!isHit) return true;
            return hit.transform.gameObject.layer != AIManager.PlayerLayerNum;
        }

        private void Awake()
        {
            m_LegController = GetComponentInChildren<LegController>();
            m_Parabola = GetComponent<Parabola>();

            m_NavMeshTransform = transform.GetChild(0);
            m_SpecialMonsterAI = m_NavMeshTransform.GetComponent<SpecialMonsterAI>();
            m_SP1AnimationController = m_NavMeshTransform.GetComponent<SP1AnimationController>();
            m_MaterialPropertyBlock = new MaterialPropertyBlock();

            m_SP1AnimationController.DoDamageAction += DoDamage;
        }

        private void Start()
        {

        }

        public void Init(Quaternion rotation, float statMultiplier)
        {
            m_PlayerData = AIManager.PlayerTransform.GetComponent<PlayerData>();
            m_PlayerShakeController = m_PlayerData.GetComponent<Controller.Player.Utility.PlayerShakeController>();

            SetRealStat(statMultiplier);

            m_SpecialMonsterAI.Init(rotation);
            m_SP1AnimationController.SetWalk(true);
            m_PlayerData.GrabPointAssign(m_GrabCameraPoint, m_GrabBodyPoint, m_LookPoint);
        }

        private void SetRealStat(float statMultiplier)
        {
            m_IsAlive = true;

            m_RealMaxHP = m_Settings.m_HP + (int)(statMultiplier * m_Settings.m_HPMultiplier);
            m_RealDef = m_Settings.m_Def + (int)(statMultiplier * m_Settings.m_DefMultiplier);
            m_RealDamage = (int)(statMultiplier * m_Settings.m_DamageMultiplier);
            m_RealCriticalHP = (int)(m_RealMaxHP * m_Settings.m_HitHP);
            
            m_CurrentHP = m_RealMaxHP;
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
            m_JumpAttackTimer += Time.deltaTime;
            m_JumpTimer += Time.deltaTime;
        }

        public void Move()
        {
            bool changeFlag = false;
            bool isWalk = m_SpecialMonsterAI.OperateAIBehavior(ref changeFlag);
            if (changeFlag) m_SP1AnimationController.SetWalk(isWalk);
        }

        #region Attack
        public void Attack()
        {
            if (m_DoingBehaviour || m_SpecialMonsterAI.GetIsOnOffMeshLink) return;

            m_TargetDist = Vector3.Distance(AIManager.PlayerTransform.position, m_NavMeshTransform.position);

            if (DetectObstacle()) return;
            float toPlayerAngle = AIManager.AngleToPlayer(m_NavMeshTransform);

            if (CanJumpAttack())
            {
                if (!m_Settings.CanJumpAttackPercentage()) m_JumpAttackTimer = 0;
                else JumpBehavior(ref m_JumpAttackTimer, m_Settings.m_JumpAttackHeightRatio, 
                    m_Settings.m_DestinationDist, m_Settings.m_PreJumpAttackTime, true);
            }
            else if (CanJump())
            {
                if (!m_Settings.CanJumpPercentage()) m_JumpTimer = 0;
                else JumpBehavior(ref m_JumpTimer, m_Settings.m_JumpHeightRatio, 0, m_Settings.m_PreJumpTime, false);
            }
            else if (CanGrabAttack(toPlayerAngle)) GrabAttack();
            else if (CanNormalAttack(toPlayerAngle)) NormalAttack();
        }

        private async void NormalAttack()
        {
            m_DoingBehaviour = true;
            m_NormalAttackTimer = 0;
            
            await m_SP1AnimationController.SetClawsAttack();
            m_GrabAttackTimer = Mathf.Min(m_GrabAttackTimer, m_Settings.m_GrabAttackSpeed - m_AttackBetweenTime);

            m_DoingBehaviour = false;
        }

        private void DoDamage()
        {
            if (!m_Settings.CanNormalAttack(m_TargetDist, AIManager.AngleToPlayer(m_NavMeshTransform))) return;
            m_PlayerData.PlayerHit(m_NavMeshTransform, m_Settings.m_Damage + m_RealDamage, m_Settings.m_NoramlAttackType);
        }

        private async void GrabAttack()
        {
            m_DoingBehaviour = true;
            m_IsGrabbing = true;

            m_PlayerData.PlayerHit(m_GrabCameraPoint, 0, m_Settings.m_GrabAttackType);

            await m_SP1AnimationController.SetGrabAttack();

            m_PlayerData.PlayerHit(m_GrabCameraPoint, m_Settings.m_GrabAttackDamage + m_RealDamage, m_Settings.m_NoramlAttackType);
            m_PlayerData.GrabAction(false, (m_GrabCameraPoint.position - m_ThrowingPoint.position).normalized * m_Settings.m_GrabThrowingForce);

            m_GrabAttackTimer = 0;
            m_NormalAttackTimer = m_Settings.m_AttackSpeed - m_AttackBetweenTime;
            m_IsGrabbing = false;
            
            await Task.Delay(1500);
            m_DoingBehaviour = false;
        }

        #region Parabola Related

        private void JumpBehavior(ref float controllingTimer, float heightRatio, float destinationDist, float preJumpTime, bool hasAnimation)
        {
            m_DoingBehaviour = true;
            controllingTimer = 0;

            float height = m_Parabola.GetHeight(heightRatio, AIManager.PlayerTransform, out Vector3 targetVector, ref m_GroundDirection, destinationDist);
            m_Parabola.CalculatePathWithHeight(targetVector, height, out float v0, out float angle, out float time);

            Jump(m_GroundDirection.normalized, v0, angle, preJumpTime, time, hasAnimation);
        }

        private async void Jump(Vector3 direction, float v0, float angle, float preJumpTime, float jumpTime, bool hasAnimation)
        {
            float upDist = hasAnimation ? 0.5f : 1;

            await PreJump(preJumpTime, upDist);
            if (hasAnimation) m_SP1AnimationController.SetJumpBiteAttack();
            await DoJump(direction, v0, angle, jumpTime, hasAnimation);

            m_GrabAttackTimer = m_Settings.m_GrabAttackSpeed - 2f;
            if (hasAnimation)
            {
                m_NormalAttackTimer = 3;

                CheckJumpAttackToPlayer();
            }

            m_DoingBehaviour = false;
        }

        private async Task PreJump(float preJumpTime, float upDist)
        {
            m_SpecialMonsterAI.SetNavMeshEnable = false;
            m_LegController.SetPreJump(true);

            float elapsedTime = 0;
            Vector3 startPos = m_NavMeshTransform.position;

            while (elapsedTime < preJumpTime)
            {
                elapsedTime += Time.deltaTime;
                m_NavMeshTransform.position = Vector3.Lerp(startPos, startPos - m_NavMeshTransform.up * upDist, elapsedTime / preJumpTime);

                await Task.Yield();
            }
            m_LegController.SetPreJump(false);
            m_LegController.Jump(true);
        }

        private async Task DoJump(Vector3 direction, float v0, float angle, float time, bool hasAnimation)
        {
            float elapsedTime = 0;
            Vector3 startPos = m_NavMeshTransform.position;
            Quaternion startRotation = m_NavMeshTransform.rotation;
            Quaternion targetRot = Quaternion.LookRotation(m_GroundDirection, AIManager.PlayerTransform.up);
            float xAngle = Mathf.Cos(angle);
            float yAngle = Mathf.Sin(angle);
            bool isPlayEndAnimation = false;

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

            m_LegController.Jump(false);
            m_SpecialMonsterAI.SetNavMeshEnable = true;
            m_SpecialMonsterAI.SetNavMeshPos(m_NavMeshTransform.position);
        }

        private void CheckJumpAttackToPlayer()
        {
            if (Physics.CheckSphere(m_GrabCameraPoint.position, m_Settings.m_JumpAttackRange, m_Settings.m_AttackableLayer, QueryTriggerInteraction.Ignore))
                m_PlayerData.PlayerHit(m_NavMeshTransform, m_Settings.m_JumpAttackDamage + m_RealDamage, m_Settings.m_NoramlAttackType);
        }

        private void OnDrawGizmosSelected()
        {
            if (!m_IsAlive) return;
            Vector3 currentPos = m_NavMeshTransform.position + m_NavMeshTransform.up * 4;
            Vector3 dir = (AIManager.PlayerTransform.position - currentPos).normalized;

            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(currentPos, dir * m_Settings.m_JumpAttackMaxRange);

            currentPos = m_LookPoint.position;
            dir = (AIManager.PlayerTransform.position - currentPos).normalized;

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(currentPos, dir * m_Settings.m_AttackRange);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(m_ThrowingPoint.position, m_GrabCameraPoint.position - m_ThrowingPoint.position);
        }
        #endregion

        #endregion

        #region Hit
        public void Hit(int damage, AttackType bulletType)
        {
            if (!m_IsAlive) return;

            int realDamage;
            if (bulletType == AttackType.Explosion) realDamage = damage / m_Settings.m_ExplosionResistance;
            else if (bulletType == AttackType.Melee) realDamage = damage / m_Settings.m_MeleeResistance;
            else realDamage = damage - m_RealDef;

            m_CurrentHP -= realDamage;

            ChangeBaseColor();
            if (m_CurrentHP <= 0) Die();
            else if (CanCriticalHit(realDamage)) CriticalHit();
        }
        private async void CriticalHit()
        {
            m_DoingBehaviour = true;

            await m_SP1AnimationController.SetHit();

            m_DoingBehaviour = false;
        }

        private void ChangeBaseColor()
        {
            float ratio = Mathf.Clamp01(m_CurrentHP / m_RealMaxHP);
            Color newColor = Color.Lerp(m_Settings.m_MaxInjuryColor, Color.white, ratio);

            m_SkinnedMeshRenderer.GetPropertyBlock(m_MaterialPropertyBlock);
            m_MaterialPropertyBlock.SetColor("_BaseColor", newColor);
            m_SkinnedMeshRenderer.SetPropertyBlock(m_MaterialPropertyBlock);
        }
        #endregion

        public void Die()
        {
            m_IsAlive = false;
            EndSpecialMonsterAction?.Invoke();

            m_SpecialMonsterAI.Dispose();
            m_LegController.Dispose();
            if (m_IsGrabbing) m_PlayerData.GrabAction(false, Vector3.zero);
            m_SP1AnimationController.SetDie();
        }
    }
}
