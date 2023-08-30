using Controller.Player.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DistanceShakeController
{
    [Header("Shake")]
    [SerializeField] private float m_MaxShakeMagnitude = 6;
    [SerializeField] private float m_MinShakeMagnitude = 0.01f;

    private PlayerShakeController m_PlayerShakeController;

    public void Init(PlayerShakeController playerShakeController)
        => m_PlayerShakeController = playerShakeController;

    public void CheckPlayerShake(ShakeType shakeType, Vector3 pos, float range, float multiplier = 15)
    {
        float dist = Vector3.Distance(m_PlayerShakeController.transform.position, pos);
        if (dist > range) return;

        float magnitude = Mathf.Clamp(1 / Mathf.Max(dist, 0.01f) * multiplier, m_MinShakeMagnitude, m_MaxShakeMagnitude);

        m_PlayerShakeController.ShakeAllTransform(shakeType, magnitude);
    }
}
