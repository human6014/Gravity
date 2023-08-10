using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.AI;
using System.Threading.Tasks;

namespace Entity.Unit.Special
{
    public class SpecialMonster2 : MonoBehaviour, IMonster
    {
        //SpecialMonster끼리 부모 클래스 만들기
        [Header("Data")]
        [SerializeField] private Scriptable.Monster.SpecialMonster2Scriptable m_Settings;

        [SerializeField] private float m_RushCheckRadius = 3;

        private Rigidbody m_PlayerRigidbody;
        private SP2AnimationController m_SP2AnimationController;
        private SpecialMonster2AI m_SpecialMonster2AI;
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

        private bool m_IsAlive;
        private bool m_DoingBehaviour;
        private bool m_DoingRecovery;
        private bool m_IsMoveRecoveryPos;
        private bool m_IsRushMove;
        private bool m_IsUsingRecovery;
        private bool m_IsBehaviorWait;
        private bool m_IsNonMovePos;
        private bool m_IsRushForce;

        private int m_RealMaxHP;
        private int m_RealDef;
        private int m_RealDamage;

        private int m_CurrentHP;
        private int m_RecoveryTriggerHP;

        public System.Action EndSpecialMonsterAction { get; set; }

        private async void MoveWait(int time)
        {
            m_IsBehaviorWait = true;
            await Task.Delay(time);
            m_IsBehaviorWait = false;
        }
        private bool CanNormalAttack() => m_Settings.CanNormalAttack(m_TargetDist, m_NormalAttackTimer);
        private bool CanGrabAttack() => m_Settings.CanGrabAttack(m_TargetDist, m_GrabAttackTimer);
        private bool CanRushAttack() => m_Settings.CanRushAttack(m_TargetDist, m_RushAttackTimer);

        private bool DetectObstacle()
        {
            Vector3 dir = (AIManager.PlayerTransform.position - transform.position).normalized;

            bool isHit = Physics.Raycast(transform.position, dir, out RaycastHit hit, Mathf.Infinity, m_Settings.m_ObstacleDetectLayer);
            if (!isHit) return true;
            return hit.transform.gameObject.layer != AIManager.PlayerLayerNum;
        }

        private void Awake()
        {
            m_SP2AnimationController = GetComponentInChildren<SP2AnimationController>();
            m_SpecialMonster2AI = GetComponent<SpecialMonster2AI>();

            Transform groundDownTransform = GameObject.Find("SP2RecoveryPos").transform;

            RecoveryPos = new List<Vector3>();
            foreach (Transform t in groundDownTransform)
                RecoveryPos.Add(t.position);

            m_SpecialMonster2AI.MoveCompToPos += HideAndRecovery;
            m_SpecialMonster2AI.RushCompToPos += RushAttackEnd;

            Init(1);
        }

        public void Init(float statMultiplier)
        {
            SetRealStat(statMultiplier);

            m_SpecialMonster2AI.Init();
            m_SpecialMonster2AI.MovementSpeed = m_Settings.m_MovementSpeed;
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

        private void Start()
        {
            m_PlayerRigidbody = AIManager.PlayerTransform.GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (!m_IsRushMove) return;
            bool isHit = Physics.CheckSphere(transform.position + transform.up * 1.5f, 3.5f, m_Settings.m_AttackableLayer);
            if (isHit)
            {
                Debug.Log("Hit");
                Vector3 dir = (AIManager.PlayerTransform.position - transform.position).normalized;
                AIManager.PlayerTransform.GetComponent<Controller.Player.FirstPersonController>().PlayerCol(dir);
                //m_PlayerRigidbody.AddForce(dir * 200 + transform.up * 100, ForceMode.Impulse);
            }
            //한번만 맞도록 변경
        }

        private void Update()
        {
            if (!m_IsAlive) return;

            GroundDownCheck();
            UpdateTimer();

            if (m_GroundDownForce != 0)
            {
                m_SpecialMonster2AI.OperateAIBehavior(transform.position, MoveType.Self);
                return;
            }


            if (!m_IsBehaviorWait)
            {
                Attack();
                Move();
            }
            else m_SP2AnimationController.SetWalk(false);
        }

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
                else m_GroundDownForce = 0;
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

            m_TargetDist = Vector3.Distance(transform.position, AIManager.PlayerTransform.position);

            if (CanGrabAttack())
            {
                GrabAttack();
            }
            else if (CanRushAttack())
                RushAttack();
            else if (CanNormalAttack())
                NormalAttack();
        }

        public void Move()
        {
            if (m_DoingRecovery)
            {
                m_SpecialMonster2AI.OperateAIBehavior(transform.position, MoveType.Self);
                m_SP2AnimationController.SetWalk(false);
            }
            else
            {
                if (m_IsMoveRecoveryPos) m_SpecialMonster2AI.OperateAIBehavior(m_RecoveryPos, MoveType.RecoveryPos);
                else if (m_IsRushMove) m_SpecialMonster2AI.OperateAIBehavior(m_RushAttackPos, MoveType.Rush);
                else m_SpecialMonster2AI.OperateAIBehavior(AIManager.PlayerGroundPosition, MoveType.ToPlayer);
                m_SP2AnimationController.SetWalk(true);
            }
        }

        public void Hit(int damage, AttackType bulletType)
        {
            if (!m_IsAlive) return;

            int realDamage;
            if (bulletType == AttackType.Explosion) realDamage = damage / m_Settings.m_ExplosionResistance;
            else if (bulletType == AttackType.Melee) realDamage = damage / m_Settings.m_MeleeResistance;
            else realDamage = damage - m_RealDef;

            m_CurrentHP -= realDamage;

            if (m_CurrentHP <= 0) Die();
            else
            {
                if (m_DoingRecovery && m_RecoveryCoroutine != null)
                {
                    StopCoroutine(m_RecoveryCoroutine);
                    m_DoingRecovery = false;
                }
                else if (!m_IsUsingRecovery && m_CurrentHP <= m_RecoveryTriggerHP) FindRecoveryPos();
            }
        }

        public void Die()
        {
            m_IsAlive = false;
        }

        #region Patterns

        private void NormalAttack()
        {
            m_NormalAttackTimer = 0;
            //m_IsNonMovePos = true;

            //m_SP2AnimationController.SetNormalAttack();
        }

        private void GrabAttack()
        {
            m_GrabAttackTimer = 0;
            //촉수로 땡기는거
        }

        private void RushAttack()
        {
            m_RushAttackTimer = 0;

            Vector3 dir = (AIManager.PlayerTransform.position - transform.position).normalized;
            dir.y = 0;
            if (!Physics.SphereCast(transform.position + transform.up * 2.5f, m_RushCheckRadius, dir, out RaycastHit hit, Mathf.Infinity, m_Settings.m_RushObstacleLayer) ||
                hit.distance <= 20)
                return;

            m_RushAttackPos = hit.point - (transform.up * 2.5f) - (dir * (m_RushCheckRadius + 1)); //혹시 모르니 체크
            m_DoingBehaviour = true;
            m_IsRushMove = true;
            m_SpecialMonster2AI.MovementSpeed = m_Settings.m_RushAttackMovementSpeed;
            m_SP2AnimationController.SetMovementSpeed(4);


        }

        private void RushAttackEnd()
        {
            m_DoingBehaviour = false;
            m_IsRushMove = false;
            m_SpecialMonster2AI.MovementSpeed = m_Settings.m_MovementSpeed;
            m_SP2AnimationController.SetMovementSpeed(1);
            MoveWait(2000);
        }

        private void GroundDown()
        {
            m_RushAttackTimer = 0;
            m_GroundDownForce = Mathf.Min(m_GroundDownForce + Time.deltaTime * 10, 35);
            Vector3 dir = transform.position - AIManager.PlayerTransform.position;
            dir.y = 0;
            dir.Normalize();
            m_PlayerRigidbody.AddForce(dir * m_GroundDownForce, ForceMode.Impulse);
            //소리 우아아앙 해서 땅으로 끌고 오기
        }

        private void FindRecoveryPos()
        {
            m_IsUsingRecovery = true;
            m_IsMoveRecoveryPos = true;
            m_SpecialMonster2AI.MovementSpeed = m_Settings.m_RecoveryMovementSpeed;

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

            float elapsedTime = 0f;
            while (elapsedTime < m_Settings.m_RecoveryTime)
            {
                m_CurrentHP += (int)(m_RecoverPerSecond * Time.deltaTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            m_DoingRecovery = false;
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + transform.up * 2.5f, m_RushCheckRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position + transform.up * 1.5f, 3.5f);
        }
    }
}
