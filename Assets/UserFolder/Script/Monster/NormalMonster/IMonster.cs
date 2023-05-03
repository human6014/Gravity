using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonster
{
    public void Move();
    public void Attack();
    public void Hit(int damage, BulletType bulletType);
    public void Die();
}
