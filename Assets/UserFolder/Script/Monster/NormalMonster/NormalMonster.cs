using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumType;

namespace Entity.Unit.Normal
{
    public class NormalMonster : PoolableScript, IMonster
    {
        [SerializeField] private Scriptable.NormalMonsterScriptable settings;
        private Manager.ObjectPoolManager.PoolingObject poolingObject;

        private Animator animator;
        private NormalMonsterAI normalMonsterAI;

        public NoramlMonsterType GetMonsterType() => settings.m_MonsterType;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            normalMonsterAI = GetComponent<NormalMonsterAI>();
        }

        private void Update()
        {
            //¼º´É issue
            Move();
            if (normalMonsterAI.IsMalfunction) ReturnObject();
            
        }

        public void Init(Vector3 pos, Manager.ObjectPoolManager.PoolingObject poolingObject)
        {
            normalMonsterAI.Init(pos);
            this.poolingObject = poolingObject;
        }

        public void Move()
        {
            normalMonsterAI.Move();
            //animator.SetBool("isMove", true);
        }

        public void Attack()
        {

        }
        public void Hit(int damage)
        {
            Debug.Log(damage);
        }

        public void Die()
        {

        }

        [ContextMenu("ReturnObject")]
        public override void ReturnObject()
        {
            normalMonsterAI.Dispose();
            Manager.SpawnManager.NormalMonsterCount--;
            poolingObject.ReturnObject(this);
        }
    }
}
