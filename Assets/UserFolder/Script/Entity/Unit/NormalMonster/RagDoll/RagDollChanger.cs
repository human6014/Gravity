using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollChanger : MonoBehaviour
{
    [SerializeField] private GameObject m_OriginalObject;
    [SerializeField] private GameObject m_RagDollObject;

    private Transform m_OriginalRoot;
    private Transform m_RagDollRoot;

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
        CopyToRagDoll(m_OriginalRoot, m_RagDollRoot);
        Debug.Log("Comp Copy");
        m_OriginalObject.SetActive(false);
        m_RagDollObject.SetActive(true);
    }


    
    public void CopyToRagDoll(Transform original, Transform ragDoll)
    {
        for(int i = 0; i < original.childCount; i++)
        {
            if (original.childCount != 0) CopyToRagDoll(original.GetChild(i), ragDoll.GetChild(i));

            ragDoll.GetChild(i).localPosition = original.GetChild(i).localPosition;
            ragDoll.GetChild(i).localRotation = original.GetChild(i).localRotation;
        }
    }
}
