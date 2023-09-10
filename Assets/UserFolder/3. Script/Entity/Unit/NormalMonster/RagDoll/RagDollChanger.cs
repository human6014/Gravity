using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollChanger : MonoBehaviour
{
    [SerializeField] private GameObject m_OriginalObject;
    [SerializeField] private GameObject m_RagDollObject;
    [SerializeField] private int m_MaxDepth = 8;

    private Transform m_OriginalRoot; 
    private Transform m_OriginalChild;
    private Transform m_RagDollRoot;
    private Transform m_RagChild;

    private void Awake()
    {
        m_OriginalRoot = m_OriginalObject.transform.GetChild(1);
        m_RagDollRoot = m_RagDollObject.transform.GetChild(1);
    }

    public void ChangeToOriginal()
    {
        m_RagDollObject.SetActive(false);
        m_OriginalObject.SetActive(true);
    }

    [ContextMenu("ChangeToRagDoll")]
    public void ChangeToRagDoll()
    {
        CopyToRagDoll(m_OriginalRoot, m_RagDollRoot, 0);

        m_OriginalObject.SetActive(false);
        m_RagDollObject.SetActive(true);
    }
    
    // 성능 이슈 심할 경우 케싱 해서 사용
    // 그래도 심할 경우 Bone만 위치 변경
    public void CopyToRagDoll(Transform original, Transform ragDoll, int depth)
    {
        for(int i = 0; i < original.childCount; i++)
        {
            if (depth <= m_MaxDepth && original.childCount != 0) CopyToRagDoll(original.GetChild(i), ragDoll.GetChild(i), depth + 1);
            
            m_RagChild = ragDoll.GetChild(i);
            m_OriginalChild = original.GetChild(i);

            m_RagChild.localPosition = m_OriginalChild.localPosition;
            m_RagChild.localRotation = m_OriginalChild.localRotation;
            if (m_RagChild.TryGetComponent(out Rigidbody rigidbody))
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }
}
