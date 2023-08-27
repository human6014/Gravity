using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void Hit(int damage, AttackType bulletType, Vector3 dir);
}
