using Controller.Player.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DistanceShakeController
{
    [Header("Shake")]
    [SerializeField] private LayerMask m_ShakeLayer;
    [SerializeField] private float m_ShakeRadiusAdd = 20;
    [SerializeField] private float m_MaxShakeMagnitude = 6;
    [SerializeField] private float m_MinShakeMagnitude = 0.01f;

    public void CheckPlayerShake(Vector3 pos, float radius)
    {
        Collider[] col = Physics.OverlapSphere(pos, radius, m_ShakeLayer);

        if (col.Length == 0) return;

        //Only Player
        Transform playerTransform = col[0].transform;

        float dist = Vector3.Distance(playerTransform.position, pos);
        float magnitude = Mathf.Clamp(1 / Mathf.Max(dist, 0.01f) * 15, m_MinShakeMagnitude, m_MaxShakeMagnitude);

        playerTransform.TryGetComponent(out PlayerShakeController psc);
        psc.ShakeAllTransform(ShakeType.Explosion, magnitude);
    }
}
