using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    private SphereCollider m_SphereCollider;

    private void Awake()
    {
        m_SphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            Debug.Log(other.name);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
    }
}
