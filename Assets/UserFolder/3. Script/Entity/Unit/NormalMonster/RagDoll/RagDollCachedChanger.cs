using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RagdollStruct
{
    public Transform m_OriginalTransform;
    public Transform m_RagdollTransform;
}

public class RagDollCachedChanger : MonoBehaviour
{
    [SerializeField] private GameObject m_OriginalObject;
    [SerializeField] private GameObject m_RagDollObject;
    [SerializeField] private RagdollStruct[] m_RagdollStructs;

    public void ChangeToOriginal()
    {
        m_RagDollObject.SetActive(false);
        m_OriginalObject.SetActive(true);
    }

    [ContextMenu("ChangeToRagDoll")]
    public void ChangeToRagDoll()
    {
        CopyToRagDoll();

        m_OriginalObject.SetActive(false);
        m_RagDollObject.SetActive(true);
    }

    public void CopyToRagDoll()
    {
        for(int i = 0; i < m_RagdollStructs.Length; i++)
        {
            m_RagdollStructs[i].m_RagdollTransform.localPosition = m_RagdollStructs[i].m_OriginalTransform.localPosition;
            m_RagdollStructs[i].m_RagdollTransform.localRotation = m_RagdollStructs[i].m_OriginalTransform.localRotation;

            if (m_RagdollStructs[i].m_RagdollTransform.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}
