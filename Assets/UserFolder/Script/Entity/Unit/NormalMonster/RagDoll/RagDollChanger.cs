using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollChanger : MonoBehaviour
{
    [SerializeField] private GameObject m_OriginalObject;
    [SerializeField] private GameObject m_RagDollObject;

    private Transform m_OriginalRoot;
    private Transform m_RagDollRoot;
    private Transform m_RagChild;
    private Transform m_OriginalChild;

    private const int m_MaxDepth = 8;

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
    
    public void CopyToRagDoll(Transform original, Transform ragDoll, int depth)
    {
        for(int i = 0; i < original.childCount; i++)
        {
            if (depth <= m_MaxDepth && original.childCount != 0) CopyToRagDoll(original.GetChild(i), ragDoll.GetChild(i), depth + 1);
            
            m_RagChild = ragDoll.GetChild(i);
            m_OriginalChild = original.GetChild(i);

            m_RagChild.localPosition = m_OriginalChild.localPosition;
            m_RagChild.localRotation = m_OriginalChild.localRotation;
        }
    }
}
