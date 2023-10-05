using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationRotate : MonoBehaviour
{
    [SerializeField] private Color[] m_AxisEmissionColor;
    [SerializeField] private Renderer[] m_AxisRenderer;

    private int currentIndex;
    private void Awake()
    {
        FindObjectOfType<Controller.PlayerInputController>().SelectGravityAxis += HighlightAxis;
        foreach (Renderer r in m_AxisRenderer) r.material.SetColor("_EmissionColor", new Color(0, 0, 0));
        HighlightAxis(1);
    }

    private void Update()
    {
        transform.forward = Vector3.forward;
        transform.up = Vector3.up;
    }

    private void HighlightAxis(int axisIndex)
    {
        m_AxisRenderer[currentIndex].material.SetColor("_EmissionColor", new Color(0,0,0));

        m_AxisRenderer[axisIndex].material.SetColor("_EmissionColor", m_AxisEmissionColor[axisIndex]);
        currentIndex = axisIndex;
    }
}
