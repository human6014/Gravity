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
        private void Awake()
        {
            flyingMovementController = GetComponent<FlyingMovementController>();
            flyingRotationController = GetComponent<FlyingRotationController>();
        }

        private void Update()
        {
            flyingRotationController.LookCurrentTarget();
        }

        private void FixedUpdate()
        {
            flyingMovementController.MoveCurrentTarget();
        }

        public void Init(Vector3 pos, Manager.ObjectPoolManager.PoolingObject poolingObject)
        {
            transform.position = pos;
            this.m_PoolingObject = poolingObject;
            flyingMovementController.Init();
            flyingRotationController.Init();
        }

        public void Move()
        {
            throw new System.NotImplementedException();
        }

        public void Attack()
        {
            throw new System.NotImplementedException();
        }

        public void Hit(int damage, BulletType bulletType)
        {
            throw new System.NotImplementedException();
        }

        public void Die()
        {
            throw new System.NotImplementedException();
        }

        [ContextMenu("ReturnObject")]
        public override void ReturnObject()
        {
            Manager.SpawnManager.FlyingMonsterCount--;
            flyingMovementController.Dispose();
            flyingRotationController.Dispose();
            m_PoolingObject.ReturnObject(this);
        }
    }
}
