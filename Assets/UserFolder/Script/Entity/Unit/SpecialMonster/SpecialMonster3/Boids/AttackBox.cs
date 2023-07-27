using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackBox : MonoBehaviour
{
    [SerializeField] private LayerMask m_CollisionLayer;
    [SerializeField] private UnityEvent CollisionEvent;

    private void OnCollisionEnter(Collision collision)
    {
        if (m_CollisionLayer == (m_CollisionLayer | (1 << collision.gameObject.layer))) 
            CollisionEvent?.Invoke();
    }
}
