using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.AI;
using System.Threading.Tasks;

namespace Entity.Unit.Special
{
    public class SpecialMonster2 : MonoBehaviour, IMonster
    {
        [Header("Data")]
        [SerializeField] private Scriptable.Monster.SpecialMonster2Scriptable m_Settings;

        [SerializeField] private Transform m_GrabEndPoint;

        [SerializeField] private float m_RushCheckRadius = 3;

        private Rigidbody m_PlayerRigidbody;
        private SP2AnimationController m_SP2AnimationController;
        private SpecialMonster2AI m_SpecialMonster2AI;
        private GrabController m_GrabController;

        private PlayerData m_PlayerData;
        private List<Vector3> RecoveryPos;
        private Coroutine m_RecoveryCoroutine;

        private Vector3 m_RecoveryPos;
        private Vector3 m_RushAttackPos;

        private float m_GroundTimer = 2;
        private float m_NormalAttackTimer;
        private float m_GrabAttackTimer;
        private float m_RushAttackTimer;

        private float m_TargetDist;
        private float m_GroundDownForce;
        private float m_RecoverPerSecond;
        private float m_StoppingDistance = 8;

        private bool m_IsAlive;
        private bool m_DoingBehaviour;
        private bool m_DoingRecovery;
        private bool m_IsMoveRecoveryPos;
        private bool m_IsNonMovePos;
        private bool m_IsRushMove;
        private bool m_IsGetRushDamage;
        private bool m_IsUsingRecovery;
        private bool m_IsBehaviorWait;

        private int[] m_IsHitParts = new int[6];

        private int m_RealMaxHP;
        private int m_RealDef;
        private int m_RealDamage;

        private int m_CurrentHP;
        private int m_RecoveryTriggerHP;

        public System.Action EndSpecialMonsterAction { get; set; }

        private bool CanNormalAttack(float angle) => m_Settings.CanNormalAttack(m_TargetDist, m_NormalAttackTimer, angle);
        private bool CanGrabAttack(float angle) => m_Settings.CanGrabAttack(m_TargetDist, m_GrabAttackTimer, angle);
        private bool CanRushAttack(float angle) => m_Settings.CanRushAttack(m_TargetDist, m_RushAttackTimer, angle);

        private async void MoveWait(int time)
        {
            m_IsBehaviorWait = true;
            await Task.Delay(time);
            m_IsBehaviorWait = false;
        }

        private bool DetectObstacle()
        {
            Vector3 dir = (AIManager.PlayerTransform.position - transform.position).normalized;

            bool isHit = Physics.Raycast(transform.position, dir, out RaycastHit hit, Mathf.Infinity, m_Settings.m_ObstacleDetectLayer);
            if (!isHit) return true;
            return hit.transform.gameObject.layer != AIManager.PlayerLayerNum;
        }

        private void ForceToPlayer(float force, Vector3 position)
        {
            Vector3 dir = position - AIManager.PlayerTransform.position;
            dir.y = 0;
            dir.Normalize();
            m_PlayerRigidbody.AddForce(dir * force, ForceMode.Impulse);
        }

        private void Awake()
        {
            m_SP2AnimationController = GetComponentInChildren<SP2AnimationController>();
            m_SpecialMonster2AI = GetComponent<SpecialMonster2AI>();
            m_GrabController = GetComponent<GrabController>();

            m_SpecialMonster2AI.MoveCompToPos += HideAndRecovery;
            m_SpecialMonster2AI.RushCompToPos += RushAttackEnd;

            m_SP2AnimationController.DoDamageAction += DoDamage;
            m_GrabController.AttachedToPlayer += ()
                => m_PlayerData.PlayerHit(transform, m_Settings.m_GrabAttackDamage + m_RealDamage, m_Settings.m_NoramlAttackType);
        }

        private void Start()
        {
            m_PlayerRigidbody = AIManager.PlayerTransform.GetComponent<Rigidbody>();
            m_PlayerData = m_PlayerRigidbody.GetComponent<PlayerData>();
        }

        public void Init(Transform spawnPos, float statMultiplier)
        {
            SetRealStat(statMultiplier);

            RecoveryPos = new List<Vector3>(spawnPos.childCount);
            foreach (Transform t in spawnPos)
                RecoveryPos.Add(t.position);

            m_SpecialMonster2AI.Init(m_Settings.m_MovementSpeed);
            m_GrabController.Init(m_Settings.m_GrabAttachedTime);
        }

        private void SetRealStat(float statMultiplier)
        {
            m_IsAlive = true;

            m_RealMaxHP = m_Settings.m_HP + (int)(statMultiplier * m_Settings.m_HPMultiplier);
            m_RealDef = m_Settings.m_Def + (int)(statMultiplier * m_Settings.m_DefMultiplier);
            m_RealDamage = (int)(statMultiplier * m_Settings.m_DamageMultiplier);
            m_RecoveryTriggerHP = (int)(m_RealMaxHP * m_Settings.m_RecoveryTriggerHP);
            m_RecoverPerSecond = (m_Settings.m_RecoveryHPAmount * m_RealMaxHP) / m_Settings.m_RecoveryTime;

            m_CurrentHP = m_RealMaxHP;
        }

        private void Update()
        {
            if (!m_IsAlive) return;

            if (m_GrabController.IsAttachedPlayer)
            {
                m_GrabController.SetBezierPath();
                return;
            }

            GroundDownCheck();
            UpdateTimer();

            //if (m_GroundDownForce != 0)
            //{
            //    MoveSelf();
            //    m_SP2AnimationController.SetRoar(true);
            //    return;
            //}
            //else m_SP2AnimationController.SetRoar(false);

            if (!m_IsBehaviorWait)
            {
                m_TargetDist = Vector3.Distance(transform.position, AIManager.PlayerTransform.position);
                Attack();
                Move();
            }
            else m_SP2AnimationController.SetWalk(false);
        }

        private void FixedUpdate()
        {
            if (!m_IsAlive) return;

            if (m_GrabController.IsAttachedPlayer)
            {
                if (Vector3.Distance(m_GrabEndPoint.position, AIManager.PlayerTransform.position) <= m_Settings.m_GrabCancellationDist)
                    GrabEnd(true);
                else ForceToPlayer(m_Settings.m_GrabForce, m_GrabEndPoint.position);
            }
            else if (m_IsRushMove)
            {
                if (!Physics.CheckSphere(transform.position + transform.up * 1.5f, 3.5f, m_Settings.m_AttackableLayer))
                    return;

                Vector3 dir = (AIManager.PlayerTransform.position - transform.position).normalized;
                AIManager.PlayerTransform.GetComponent<Controller.Player.FirstPersonController>().PlayerCol(dir);
                if (!m_IsGetRushDamage)
                {
                    m_PlayerData.PlayerHit(transform, m_Settings.m_RushAttackDamage + m_RealDamage, m_Settings.m_NoramlAttackType);
                    m_IsGetRushDamage = true;
                }
            }
        }

        private void UpdateTimer()
        {
            m_NormalAttackTimer += Time.deltaTime;
            m_GrabAttackTimer += Time.deltaTime;
            m_RushAttackTimer += Time.deltaTime;
        }

        public void Attack()
        {
            if (m_DoingBehaviour || m_IsMoveRecoveryPos || m_DoingRecovery || DetectObstacle()) return;

            float toPlayerAngle = AIManager.AngleToPlayer(transform);

            if (CanGrabAttack(toPlayerAngle)) GrabAttackStart();
            else if (CanRushAttack(toPlayerAngle)) RushAttackStart();
            else if (CanNormalAttack(toPlayerAngle)) NormalAttack();
        }

        #region Move
        public void Move()
        {
            if (m_DoingRecovery || m_IsNonMovePos)
            {
                MoveSelf();
                return;
            }

            bool isWalkAnimation = true;
            if (m_IsRushMove) m_SpecialMonster2AI.OperateAIBehavior(m_RushAttackPos, MoveType.Rush);
            else if (m_IsMoveRecoveryPos) m_SpecialMonster2AI.OperateAIBehavior(m_RecoveryPos, MoveType.RecoveryPos);
            else if (m_TargetDist <= m_StoppingDistance)
            {
                m_SpecialMonster2AI.OperateAIBehavior(transform.position, MoveType.Self);
                m_SpecialMonster2AI.RotateToPlayer();
                if (AIManager.IsInsideAngleToPlayer(transform, m_Settings.m_SelfRotateAbleAngle)) isWalkAnimation = false;
            }
            else m_SpecialMonster2AI.OperateAIBehavior(AIManager.PlayerGroundPosition, MoveType.ToPlayer);
            
            m_SP2AnimationController.SetWalk(isWalkAnimation);
        }

        private void MoveSelf()
        {
            m_SpecialMonster2AI.OperateAIBehavior(transform.position, MoveType.Self);
            m_SP2AnimationController.SetWalk(false);
        }
        #endregion

        #region Hit
        public void Hit(int damage, AttackType bulletType)
        {
            if (!m_IsAlive) return;

            TypeToDamage(bulletType, damage, out int realDamage);

            m_CurrentHP -= realDamage;

            if (m_CurrentHP <= 0) Die();
            else
            {
                if (m_DoingRecovery && m_RecoveryCoroutine != null) HitAndStopHeal();
                else if (!m_IsUsingRecovery && !m_GrabController.IsGrabbing && m_CurrentHP <= m_RecoveryTriggerHP) FindRecoveryPos();
            }
        }

        public void GrabModeHit(int damage,int partsNumber, AttackType bulletType)
        {
            if (!m_IsAlive || !m_GrabController.IsAttachedPlayer) return;

            TypeToDamage(bulletType, damage, out int realDamage);

            m_IsHitParts[partsNumber] += realDamage;

            for (int i = 0; i < m_IsHitParts.Length; i++)
            {
                if (m_IsHitParts[i] < m_Settings.m_GrabCancellationDamage) return;
            }

            GrabEnd(false);
        }

        private void TypeToDamage(AttackType bulletType, int damage, out int realDamage)
        {
            if (bulletType == AttackType.Explosion) realDamage = damage / m_Settings.m_ExplosionResistance;
            else if (bulletType == AttackType.Melee) realDamage = damage / m_Settings.m_MeleeResistance;
            else realDamage = damage - m_RealDef;
        }

        private async void HitAndStopHeal()
        {
            StopCoroutine(m_RecoveryCoroutine);
            await m_SP2AnimationController.SetCriticalHit();
            m_DoingRecovery = false;
        }
        #endregion

        #region NormalAttack
        private async void NormalAttack()
        {
            m_NormalAttackTimer = 0;
            m_DoingBehaviour = true;
            m_IsNonMovePos = true;

            await m_SP2AnimationController.SetNormalAttack();

            m_DoingBehaviour = false;
            m_IsNonMovePos = false;
        }

        private void DoDamage()
        {
            if (!m_Settings.CanNormalAttack(m_TargetDist, AIManager.AngleToPlayer(transform))) return;
            m_PlayerData.PlayerHit(transform, m_Settings.m_Damage + m_RealDamage, m_Settings.m_NoramlAttackType);
        }
        #endregion

        #region GrabAttack
        private void GrabAttackStart()
        {
            m_GrabAttackTimer = 0;
            m_DoingBehaviour = true;
            m_IsNonMovePos = true;

            m_SP2AnimationController.SetWalk(false);
            m_SP2AnimationController.SetIdleSpeed(0.5f);

            m_GrabController.SetBezierPath();
        }

        private void GrabEnd(bool isSuccessGrab)
        {
            for (int i = 0; i < m_IsHitParts.Length; i++)
                m_IsHitParts[i] = 0;

            m_SP2AnimationController.SetWalk(true);
            m_SP2AnimationController.SetIdleSpeed(1);

            m_RushAttackTimer = 0;
            m_GrabAttackTimer = 0;

            if (isSuccessGrab)
            {
                m_NormalAttackTimer = m_Settings.m_AttackSpeed;
                m_GrabController.ImmediateGrabEnd();
            }
            else
            {
                m_NormalAttackTimer = 0;
                StartCoroutine(m_GrabController.GrabEndCoroutine(2));
            }

            m_DoingBehaviour = false;
            m_IsNonMovePos = false;
        }
        #endregion

        #region RushAttack
        private void RushAttackStart()
        {
            m_RushAttackTimer = 0;

            Vector3 dir = (AIManager.PlayerTransform.position - transform.position).normalized;
            dir.y = 0;
            if (!Physics.SphereCast(transform.position + transform.up * 3f, m_RushCheckRadius, dir, out RaycastHit hit, Mathf.Infinity, m_Settings.m_RushObstacleLayer) ||
                hit.distance <= 20)
                return;

            m_RushAttackPos = new Vector3(hit.point.x, transform.position.y, hit.point.z) - dir * m_RushCheckRadius;

            m_DoingBehaviour = true;
            m_IsRushMove = true;
            m_IsGetRushDamage = false;

            m_SpecialMonster2AI.MovementSpeed = m_Settings.m_RushAttackMovementSpeed;
            m_SP2AnimationController.SetMovementSpeed(4);
        }

        private void RushAttackEnd()
        {
            m_DoingBehaviour = false;
            m_IsRushMove = false;

            m_RushAttackTimer = 0;

            m_SpecialMonster2AI.MovementSpeed = m_Settings.m_MovementSpeed;
            m_SP2AnimationController.SetMovementSpeed(1);

            MoveWait(2000);
        }
        #endregion

        #region Recovery
        private void FindRecoveryPos()
        {
            m_IsUsingRecovery = true;
            m_IsMoveRecoveryPos = true;
            m_SpecialMonster2AI.MovementSpeed = m_Settings.m_RecoveryMovementSpeed;
            m_SP2AnimationController.SetMovementSpeed(4);

            float dist = 0;
            float curDist;
            Vector3 comparePos = AIManager.PlayerTransform.position;
            foreach (Vector3 pos in RecoveryPos)
            {
                curDist = (comparePos - pos).sqrMagnitude;
                if (dist < curDist)
                {
                    dist = curDist;
                    m_RecoveryPos = pos;
                }
            }
        }

        private void HideAndRecovery()
        {
            m_IsMoveRecoveryPos = false;
            m_DoingRecovery = true;

            if (m_RecoveryCoroutine != null) StopCoroutine(m_RecoveryCoroutine);
            m_RecoveryCoroutine = StartCoroutine(Recovery());
        }

        private IEnumerator Recovery()
        {
            m_SpecialMonster2AI.MovementSpeed = m_Settings.m_MovementSpeed;
            m_SP2AnimationController.SetMovementSpeed(1);

            float elapsedTime = 0f;
            while (elapsedTime < m_Settings.m_RecoveryTime)
            {
                m_CurrentHP += (int)(m_RecoverPerSecond * Time.deltaTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            m_IsUsingRecovery = false;
            //ȸ�� �߿� ���� �ȹ����� ȸ���Ϸ� ��� ��
            //���̵� ���� ������

            m_DoingRecovery = false;
        }
        #endregion

        #region GroundDown        
        private void GroundDownCheck()
        {
            if (!AIManager.OnFloor)
            {
                m_GroundTimer = 0;
                GroundDown();
    }
            else
            {
                m_GroundTimer += Time.deltaTime;
                if (m_GroundTimer <= 0.7f) GroundDown();
                else
                {
                    m_SP2AnimationController.SetRoar(false);
                    m_GroundDownForce = 0;
                }
            }
        }

        private void GroundDown()
        {
            m_RushAttackTimer = 0;
            m_GroundDownForce = Mathf.Min(m_GroundDownForce + Time.deltaTime * m_Settings.m_GroundDownForceMultiplier, m_Settings.m_MaxGroundDownForce);

            MoveSelf();
            m_SP2AnimationController.SetRoar(true);

            ForceToPlayer(m_GroundDownForce, transform.position);
            //�Ҹ� ��ƾƾ� �ؼ� ������ ���� ����
        }
        #endregion

        public void Die()
        {
            m_IsAlive = false;
            EndSpecialMonsterAction?.Invoke();

            StopAllCoroutines();
            m_SpecialMonster2AI.Dispose();
            m_GrabController.Dispose();
            m_SP2AnimationController.SetDeath(true);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + transform.up * 3f, m_RushCheckRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + transform.up * 1.5f, 3.5f);

            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(m_RushAttackPos,1);
        }
    }
}
