using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.AI;

namespace Entity.Unit.Special
{
    public class SpecialMonster2 : MonoBehaviour, IMonster
    {
        //SpecialMonster끼리 부모 클래스 만들기
        [Header("Data")]
        [SerializeField] private Scriptable.Monster.SpecialMonster2Scriptable m_Settings;

        private Rigidbody m_PlayerRigidbody;
        private SP2AnimationController m_SP2AnimationController;
        private SpecialMonster2AI m_SpecialMonster2AI;
        private List<Vector3> RecoveryPos;

        private Vector3 GroundDownPos;
        private Vector3 m_RecoveryPos;

        private float m_GroundTimer = 2;
        private float m_NormalAttackTimer;
        private float m_GrabAttackTimer;
        private float m_RushAttackTimer;
        private float m_TargetDist;
        private float m_GroundDownForce;

        private bool m_IsAlive;
        private bool m_DoingBehaviour;
        private bool m_DoingRecovery;
        private bool m_IsMoveRecoveryPos;
        private bool m_IsRushMove;

        private int m_RealMaxHP;
        private int m_RealDef;
        private int m_RealDamage;
        private int m_CurrentHP;

        private int m_RecoveryHPAmount;
        private int m_RecoveryTriggerHP;

        public System.Action EndSpecialMonsterAction { get; set; }

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
            GroundDownPos = groundDownTransform.position;

            RecoveryPos = new List<Vector3>();
            foreach (Transform t in groundDownTransform)
                RecoveryPos.Add(t.position);

            m_SpecialMonster2AI.MoveCompToPos += HideAndRecovery;

            Init(1);
        }

        public void Init(float statMultiplier)
        {
            SetRealStat(statMultiplier);

            

            m_SpecialMonster2AI.Init();
        }

        private void SetRealStat(float statMultiplier)
        {
            m_IsAlive = true;

            m_RealMaxHP = m_Settings.m_HP + (int)(statMultiplier * m_Settings.m_HPMultiplier);
            m_RealDef = m_Settings.m_Def + (int)(statMultiplier * m_Settings.m_DefMultiplier);
            m_RealDamage = (int)(statMultiplier * m_Settings.m_DamageMultiplier);
            m_RecoveryTriggerHP = (int)(m_RealMaxHP * m_Settings.m_RecoveryTriggerHP);
            m_RecoveryHPAmount = (int)(m_RealMaxHP * m_Settings.m_RecoveryHPAmount);

            m_CurrentHP = m_RealMaxHP;
        }

        private void Start()
        {
            m_PlayerRigidbody = AIManager.PlayerTransform.GetComponent<Rigidbody>();
        }

        
        private void Update()
        {
            if (!m_IsAlive) return;
            if (!AIManager.OnFloor)
            {
                m_GroundTimer = 0;
                GroundDown();
                return;
            }
            else
            {
                m_GroundTimer += Time.deltaTime;
                if (m_GroundTimer <= 1) GroundDown();
                else m_GroundDownForce = 0;
            }

            UpdateTimer();
            Attack();
            Move();
        }

        private void FixedUpdate()
        {
            if (!m_IsRushMove) return;

            bool isHit = Physics.Raycast(transform.position, transform.forward, 5, m_Settings.m_RushObstacleLayer);
        }

        private void UpdateTimer()
        {
            m_NormalAttackTimer += Time.deltaTime;
            m_GrabAttackTimer += Time.deltaTime;
            m_RushAttackTimer += Time.deltaTime;
        }

        public void Move()
        {
            if (m_DoingRecovery) m_SpecialMonster2AI.OperateAIBehavior(transform.position, MoveType.Self);
            else if (m_IsMoveRecoveryPos) m_SpecialMonster2AI.OperateAIBehavior(m_RecoveryPos, MoveType.RecoveryPos);
            else if (m_IsRushMove) m_SpecialMonster2AI.OperateAIBehavior(transform.position + transform.forward, MoveType.Rush);
            else m_SpecialMonster2AI.OperateAIBehavior(AIManager.PlayerGroundPosition, MoveType.ToPlayer);
        }

        public void Attack()
        {
            if (m_DoingBehaviour || m_IsMoveRecoveryPos || m_DoingRecovery || DetectObstacle()) return;

            m_TargetDist = Vector3.Distance(transform.position, AIManager.PlayerTransform.position);

            if (CanGrabAttack())
            {

            }
            else if (CanRushAttack())
            {

            }
            else if (CanNormalAttack())
            {

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
                if (m_CurrentHP <= m_RecoveryTriggerHP) FindRecoveryPos();
                else if (m_DoingRecovery)
                {

                }
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
        }

        private void GrabAttack()
        {
            m_GrabAttackTimer = 0;
            //촉수로 땡기는거
        }

        private void RushAttack()
        {
            m_RushAttackTimer = 0;
            m_IsRushMove = true;
        }

        private void GroundDown()
        {
            m_GroundDownForce = Mathf.Min(m_GroundDownForce + Time.deltaTime * 10, 35);
            Vector3 dir = transform.position - AIManager.PlayerTransform.position;
            dir.y = 0;
            dir.Normalize();
            m_PlayerRigidbody.AddForce(dir * m_GroundDownForce, ForceMode.Impulse);
            //소리 우아아앙 해서 땅으로 끌고 오기
        }

        private void FindRecoveryPos()
        {
            m_IsMoveRecoveryPos = true;

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
            //좀 이상함
        }

        private void HideAndRecovery()
        {
            m_IsMoveRecoveryPos = false;
            m_DoingRecovery = true;

            //StartCoroutine(Recovery());
        }

        private IEnumerator Recovery()
        {
            float elapsedTime = 0f;

            while (elapsedTime < m_Settings.m_RecoveryTime)
            {
                m_CurrentHP += (int)(m_RealMaxHP * m_Settings.m_RecoveryHPAmount * Time.deltaTime);
                m_CurrentHP = Mathf.Clamp(m_CurrentHP, 0, m_RealMaxHP);

                elapsedTime += Time.deltaTime;
                yield return null;
            }
            //수정 해야함


            m_DoingRecovery = false;
        }

        #endregion
    }
}
