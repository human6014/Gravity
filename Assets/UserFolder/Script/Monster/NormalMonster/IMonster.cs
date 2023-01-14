using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonster
{
    public void Init(Vector3 pos, Manager.ObjectPoolManager.PoolingObject poolingObject);
    public void Move();
    public void Hit(int damage);
    public void Attack();
    public void Die();
}
