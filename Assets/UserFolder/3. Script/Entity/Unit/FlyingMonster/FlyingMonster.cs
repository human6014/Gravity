using Scriptable.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit.Flying
{
    [RequireComponent(typeof(FlyingMovementController), typeof(FlyingRotationController))]
    public class FlyingMonster : PoolableScript, IMonster
    {
        [SerializeField] private FlyingMonsterScriptable settings;

        private FlyingMovementController m_FlyingMovementController;
        private FlyingRotationController m_FlyingRotationController;
        private Rigidbody m_Rigidbody;
        private Manager.ObjectPoolManager.PoolingObject m_PoisonSpherePooling;

        private bool m_IsAlive;

        private int m_RealMaxHP;
        private int m_RealDef;
        private int m_RealDamage;
        private int m_CurrentHP;

        private float m_AttackTimer;

        public System.Action<FlyingMonsterType> KilledFlyingMonsterAction { get; set; }
        public System.Action EndFlyingMonsterAction { get; set; }

        private void Awake()
        {
            m_FlyingMovementController = GetComponent<FlyingMovementController>();
            m_FlyingRotationController = GetComponent<FlyingRotationController>();
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            if (!m_IsAlive) return;
            m_AttackTimer += Time.deltaTime;
            if (m_FlyingMovementController.AttackableToTarget && m_AttackTimer >= settings.m_AttackSpeed) Attack();
            //사거리 제한 아직 없는데
            Move();
            m_FlyingRotationController.LookCurrentTarget();
        }

        public void Init(Vector3 pos, float statMultiplier, 
            Manager.ObjectPoolManager.PoolingObject poolingObject, 
            Manager.ObjectPoolManager.PoolingObject poisonSpherePooling)
        {
            transform.position = pos;
            m_PoolingObject = poolingObject;
            m_PoisonSpherePooling = poisonSpherePooling;

            m_Rigidbody.useGravity = false;
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            SetRealStat(statMultiplier);

            m_FlyingMovementController.Init();
            m_FlyingRotationController.Init();
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
            m_FlyingMovementController.MoveCurrentTarget();
        }

        public void Attack()
        {
            m_AttackTimer = 0;

            ((PoisonSphere)m_PoisonSpherePooling.GetObject(true)).Init(m_PoisonSpherePooling,transform.position,transform.rotation,m_RealDamage);
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
            m_Rigidbody.useGravity = true;
            m_Rigidbody.constraints = RigidbodyConstraints.None;
            KilledFlyingMonsterAction?.Invoke(settings.m_MonsterType);

            Invoke(nameof(ReturnObject),5);
        }


        public override void ReturnObject()
        {
            EndFlyingMonsterAction?.Invoke();
            m_FlyingMovementController.Dispose();
            m_FlyingRotationController.Dispose();
            m_PoolingObject.ReturnObject(this);
        }
    }
}
