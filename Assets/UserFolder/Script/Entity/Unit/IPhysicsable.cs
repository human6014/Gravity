using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPhysicsable
{
    public bool PhysicsableHit(int damage, AttackType bulletType);
}
