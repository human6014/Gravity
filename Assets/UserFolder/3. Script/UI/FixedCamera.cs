using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedCamera : MonoBehaviour
{
    [SerializeField] private Transform m_UpAxis;
    private void Update()
    {
        transform.localPosition = -m_UpAxis.localPosition;
    }
}
