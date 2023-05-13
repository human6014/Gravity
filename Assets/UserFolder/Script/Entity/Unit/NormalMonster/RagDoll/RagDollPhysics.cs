using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollPhysics : MonoBehaviour
{
    private Rigidbody m_Rigidbody;

    private void Awake() =>  m_Rigidbody = GetComponent<Rigidbody>();
    
    [ContextMenu("TestForce")]
    public void TestForce()
    {
        m_Rigidbody.AddForce(new Vector3(0,0,-10),ForceMode.Impulse);
    }
}
