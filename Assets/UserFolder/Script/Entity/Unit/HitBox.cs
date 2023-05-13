using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Unit.Special;
using System;
using UnityEngine.Events;

public class HitBox : MonoBehaviour, IDamageable
{
    [SerializeField] private UnityEvent<int, AttackType> m_HitEvent;
    [SerializeField] private bool m_IsWeakPoint;

    public void Hit(int damage, AttackType bulletType, Vector3 dir)
    {
        int totalDamage = m_IsWeakPoint ? (int)(damage * 1.5f) : damage;
        m_HitEvent?.Invoke(totalDamage, bulletType);
    }
}
