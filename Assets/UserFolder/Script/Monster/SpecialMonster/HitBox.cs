using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Unit.Special;
using System;
using UnityEngine.Events;

public class HitBox : MonoBehaviour, IDamageable
{
    [SerializeField] private UnityEvent<int> m_HitEvent;
    [SerializeField] private bool m_IsWeakPoint;
    public void Hit(int damage)
    {
        damage = m_IsWeakPoint ? (int)(damage * 1.2f): damage;
        m_HitEvent?.Invoke(damage);
    }
    
}
