using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShakeType
{
    Landing,
    Changing,
    Reloading,
    Attacking,
    Explosion,
    SP1Grab,
    SP2Roar,
    SP2RushEnd,
    SP2Walk,
    SP2NormalAttack
}

namespace Scriptable 
{
    [CreateAssetMenu(fileName = "CameraShakeData", menuName = "Scriptable Object/CameraShakeDatas", order = int.MaxValue)]
    public class TransformShakeData : ScriptableObject
    {
        public ShakeType m_ShakeType;

        public Vector3 m_PosShake = new Vector3(0.15f, 0.15f, 0.15f);
        public Vector3 m_RotShake = new Vector3(1,1,1);

        public float m_Magnitude = 1;
        public float m_Roughness = 1;

        public float m_FadeInTime = 0.1f;
        public float m_FadeOutTime = 0.1f;
    }
}
/*
 * Camera Shake 요소들
 * 
 * SP1 Grab
 * 수류탄
 * SP2 Roar
 * Falling
 * 등등....
 */
