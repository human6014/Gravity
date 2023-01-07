using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMonster
{
    public void Init();
    public void Hit();
    public void Attack();
    public void Die();
}
