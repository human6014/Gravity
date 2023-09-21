using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public bool Hit(int damage, AttackType bulletType, Vector3 dir);
}
