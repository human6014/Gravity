using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SelectableHitBox : MonoBehaviour, IDamageable
{
    [SerializeField] private UnityEvent<int, int, AttackType> m_HitEvent;
    [SerializeField] private bool m_IsWeakPoint;
    [SerializeField] private bool m_IsEffect;
    [SerializeField] private int m_PartNumber;

    public bool Hit(int damage, AttackType bulletType, Vector3 dir)
    {
        int totalDamage = m_IsWeakPoint ? (int)(damage * 1.5f) : damage;
        m_HitEvent?.Invoke(totalDamage, m_PartNumber, bulletType);

        return m_IsEffect;
    }
}
