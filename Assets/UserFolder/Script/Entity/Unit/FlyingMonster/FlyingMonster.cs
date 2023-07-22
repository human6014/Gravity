using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit.Flying
{
    [RequireComponent(typeof(FlyingMovementController), typeof(FlyingRotationController))]
    public class FlyingMonster : PoolableScript, IMonster
    {
        [SerializeField] private Scriptable.Monster.FlyingMonsterScriptable settings;

        private FlyingMovementController flyingMovementController;
        private FlyingRotationController flyingRotationController;
        private PlayerData m_PlayerData;

        private bool m_IsAlive;

        private int m_RealMaxHP;
        private int m_RealDef;
        private int m_RealDamage;
        private int m_CurrentHP;

        private float m_AttackTimer;

        private void Awake()
        {
            flyingMovementController = GetComponent<FlyingMovementController>();
            flyingRotationController = GetComponent<FlyingRotationController>();
        }

        private void Update()
        {
            if (!m_IsAlive) return;
            m_AttackTimer += Time.deltaTime;
            flyingRotationController.LookCurrentTarget();
        }

        private void FixedUpdate()
        {
            flyingMovementController.MoveCurrentTarget();
        }

        public void Init(Vector3 pos, Manager.ObjectPoolManager.PoolingObject poolingObject, float statMultiplier)
        {
            transform.position = pos;
            m_PoolingObject = poolingObject;

            SetRealStat(statMultiplier);

            flyingMovementController.Init();
            flyingRotationController.Init();
        }

        private void SetRealStat(float statMultiplier)
        {
            m_IsAlive = true;

            m_RealMaxHP = settings.m_HP + (int)(statMultiplier * settings.m_HPMultiplier);
            m_RealDef = settings.m_Def + (int)(statMultiplier * settings.m_HPMultiplier);
            m_RealDamage = settings.m_Damage + (int)(statMultiplier * settings.m_Damage);

            m_CurrentHP = m_RealMaxHP;
        }

        public void Move()
        {
            
        }

        public void Attack()
        {
            m_AttackTimer = 0;
            
            m_PlayerData.PlayerHit(transform, m_RealDamage, settings.m_NoramlAttackType);
        }

        public void Hit(int damage, AttackType bulletType)
        {
            if (!m_IsAlive) return;
            TypeToDamage(damage, bulletType);

            if (m_CurrentHP <= 0) Die();
        }

        private void TypeToDamage(int damage, AttackType bulletType)
        {
            if (bulletType == AttackType.Explosion) m_CurrentHP -= (damage / settings.m_ExplosionResistance);
            else if (bulletType == AttackType.Melee) m_CurrentHP -= (damage / settings.m_MeleeResistance);
            else m_CurrentHP -= (damage - m_RealDef);
        }

        public void Die()
        {
            m_CurrentHP = 0;
            m_IsAlive = false;

            //ReturnObject();
        }


        public override void ReturnObject()
        {
            Manager.SpawnManager.FlyingMonsterCount--;
            flyingMovementController.Dispose();
            flyingRotationController.Dispose();
            m_PoolingObject.ReturnObject(this);
        }
    }
}
