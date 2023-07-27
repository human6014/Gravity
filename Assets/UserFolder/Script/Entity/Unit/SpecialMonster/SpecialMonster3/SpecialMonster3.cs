using Scriptable.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.AI;

namespace Entity.Unit.Special
{
    public class SpecialMonster3 : MonoBehaviour, IMonster
    {
        [SerializeField] [Range(0, 2000)] private int m_BoidsInstacingCount = 500;
        [SerializeField] private SpecialMonster3Scriptable m_Setting;
        private Transform[] m_MovePoints;

        private SP3AnimationController m_SP3AnimationController;
        private BoidsController m_BoidsController;
        private PlayerData m_PlayerData;

        private bool m_IsAlive;

        private int m_PlayerLayerNum;

        private int m_RealMaxHP;
        private int m_RespawnBoidsHP;
        private int m_RealDef;
        private int m_RealDamage;
        private int m_CurrentHP;

        private float m_AttackTimer;
        private float m_TraceBoidsTimer;
        private float m_PatrolBoidsTimer;

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

            m_MovePoints = new Transform[6];
            //Transform points = transform.Find("MovePoints");
            //int count = 0;
            //foreach(Transform child in points)
            //    m_MovePoints[count++] = child;

            m_PlayerLayerNum = LayerMask.NameToLayer("Player");
        }

        public void Init(Vector3 pos, float statMultiplier)
        {
            m_PlayerData = AIManager.PlayerTransform.GetComponent<PlayerData>();
            m_SP3AnimationController.Init();
            SetRealStat(statMultiplier);
        }

        private void SetRealStat(float statMultiplier)
        {
            m_IsAlive = true;

            m_RealMaxHP = m_Setting.m_HP + (int)(statMultiplier * m_Setting.m_HPMultiplier);
            m_RealDef = m_Setting.m_Def + (int)(statMultiplier * m_Setting.m_HPMultiplier);
            m_RealDamage = m_Setting.m_Damage + (int)(statMultiplier * m_Setting.m_Damage);

            m_RespawnBoidsHP = (int)(m_RealMaxHP * 0.5f);
            m_CurrentHP = m_RealMaxHP;
        }

        private void Start()
        {
            m_BoidsController.GenerateBoidMonster(m_BoidsInstacingCount);
        }

        private void Update()
        {
            UpdateTimer();
        }

        private void UpdateTimer()
        {
            m_AttackTimer += Time.deltaTime;
            m_TraceBoidsTimer += Time.deltaTime;
            m_PatrolBoidsTimer += Time.deltaTime;
        }

        public void Attack()
        {
            
        }

        public void Move()
        {

        }

        public void Hit(int damage, AttackType bulletType)
        {
            if (!m_IsAlive) return;

            int realDamage;
            if (bulletType == AttackType.Explosion) realDamage = damage / m_Setting.m_ExplosionResistance;
            else if (bulletType == AttackType.Melee) realDamage = damage / m_Setting.m_MeleeResistance;
            else realDamage = damage - m_RealDef;

            m_CurrentHP -= realDamage;

            if (m_CurrentHP <= 0) Die();
            else if (m_CurrentHP <= m_RealMaxHP) RespawnBoids();
        }

        private void RespawnBoids()
        {

        }

        public void Die()
        {
            m_IsAlive = false;
            EndSpecialMonsterAction?.Invoke();
        }
    }
}
