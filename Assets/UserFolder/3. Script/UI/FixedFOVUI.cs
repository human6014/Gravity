using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedFOVUI : MonoBehaviour
{
    [SerializeField] private Camera m_FixedCamera;
    [SerializeField] private float m_Interporate;

    private Camera m_FixingCamera;
    private float m_InitFixedFOV;
    private float m_InitFixingFOV;
    private float m_Ratio;

    private void Awake()
    {
        m_FixingCamera = GetComponent<Camera>();

        m_InitFixedFOV = m_FixedCamera.fieldOfView;
        m_InitFixingFOV = m_FixingCamera.fieldOfView;
        m_Ratio = (m_InitFixingFOV / m_InitFixedFOV) + m_Interporate;
    }

    private void Update()
    {
        m_FixingCamera.fieldOfView = m_InitFixingFOV + (m_FixedCamera.fieldOfView - m_InitFixedFOV) * m_Ratio;
    }
}
