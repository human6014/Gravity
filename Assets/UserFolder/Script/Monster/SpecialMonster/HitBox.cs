using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Unit.Special;
using System;
using UnityEngine.Events;

public class HitBox : MonoBehaviour, IDamageable
{
    [SerializeField] private UnityEvent<int, BulletType> m_HitEvent;
    [SerializeField] private bool m_IsWeakPoint;

    public void Hit(int damage, BulletType bulletType)
    {
        int totalDamage = m_IsWeakPoint ? (int)(damage * 1.5f) : damage;
        m_HitEvent?.Invoke(totalDamage, bulletType);
    }
}
