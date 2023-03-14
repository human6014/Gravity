using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlyingMovementController),typeof(FlyingRotationController))]
public class FlyingMonster : PoolableScript, IMonster
{
    [SerializeField] private Scriptable.FlyingMonsterScriptable settings;
    private ObjectPoolManager.PoolingObject poolingObject;

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


    public void Init(Vector3 pos, ObjectPoolManager.PoolingObject poolingObject)
    {
        transform.position = pos;
        this.poolingObject = poolingObject;
    }

    public void Move()
    {
        throw new System.NotImplementedException();
    }
    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void Hit(int damage)
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
        poolingObject.ReturnObject(this);
    }
}
