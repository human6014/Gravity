using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Controller.Util
{
    [Serializable]
    public class WeaponSway
    {
        [Header("Sway")]
        [SerializeField] private Transform m_Sway;
        [SerializeField] private float m_Smooth = 12;
        [SerializeField] private float m_SwayMultiplier = 3;

        [SerializeField] private float m_MaxX = 5;
        [SerializeField] private float m_MaxY = 5;
        public void Sway(float xMovement, float yMovement)
        {
            //각도 기준 X
            float clampX = Mathf.Clamp(yMovement * m_SwayMultiplier, -m_MaxX, m_MaxX);
            float clampY = Mathf.Clamp(xMovement * m_SwayMultiplier, -m_MaxY, m_MaxY);

            Quaternion rotationX = Quaternion.AngleAxis(clampX, -Vector3.right);
            Quaternion rotationY = Quaternion.AngleAxis(clampY, Vector3.up);

            Quaternion targetRotation = rotationX * rotationY;

            m_Sway.localRotation = Quaternion.Slerp(m_Sway.localRotation, targetRotation, m_Smooth * Time.deltaTime);
        }
    }
}
