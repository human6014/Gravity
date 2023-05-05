using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] m_ArmsRenderer;

    public void AppearArms(bool isActive)
    {
        for(int i = 0; i < m_ArmsRenderer.Length; i++)
            m_ArmsRenderer[i].enabled = isActive;
    }
}
