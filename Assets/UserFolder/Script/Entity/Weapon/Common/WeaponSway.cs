using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Contoller.Util
{
    [Serializable]
    public class WeaponSway
    {
        [Header("Sway")]
        [SerializeField] private Transform m_Sway;
        [SerializeField] private float m_Smooth;
        [SerializeField] private float m_SwayMultiplier;

        public void Sway(float xMovement, float yMovement)
        {
            Quaternion rotationX = Quaternion.AngleAxis(-yMovement * m_SwayMultiplier, Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(xMovement * m_SwayMultiplier, Vector3.up);

            Quaternion targetRotation = rotationX * rotationY;

            m_Sway.localRotation = Quaternion.Slerp(m_Sway.localRotation, targetRotation, m_Smooth * Time.deltaTime);
        }
    }
}
