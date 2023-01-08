using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
public class NormalMonster : PoolableScript, IMonster
{
    NormalMonsterAI normalMonsterAI;

    private void Awake()
    {
        normalMonsterAI = GetComponent<NormalMonsterAI>();
    }

    public void Init(Vector3 pos)
    {
        normalMonsterAI.Init(pos);
    }
    public void Attack()
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }

    public void Hit()
    {
        throw new System.NotImplementedException();
    }

    public void Init()
    {
        throw new System.NotImplementedException();
    }

    public override void ReturnObject()
    {
        throw new System.NotImplementedException();
    }
}
