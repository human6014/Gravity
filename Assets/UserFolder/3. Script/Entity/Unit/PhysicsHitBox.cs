using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHitBox : MonoBehaviour, IDamageable
{
    [SerializeField] private Rigidbody m_CorrespondingParts;
    [SerializeField] private bool m_IsWeakPoint;
    [SerializeField] private bool m_IsEffect;

    private IPhysicsable m_Physicsable;

    private void Awake() => m_Physicsable = transform.root.GetComponent<IPhysicsable>();
    
    public bool Hit(int damage, AttackType bulletType, Vector3 dir)
    {
        int totalDamage = m_IsWeakPoint ? (int)(damage * 1.5f) : damage;

        if (m_Physicsable.PhysicsableHit(totalDamage, bulletType)) 
            m_CorrespondingParts.AddForce(dir, ForceMode.Impulse);

        return m_IsEffect;
    }
}
