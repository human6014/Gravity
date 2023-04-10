using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [SerializeField] private Explosible m_Explosible;

    private void OnTriggerEnter(Collider other) => m_Explosible.m_TriggerStay?.Invoke(true, other);

    private void OnTriggerExit(Collider other) => m_Explosible.m_TriggerStay?.Invoke(false, other);
    
}
