using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scriptable;

namespace Controller.Player.Utility
{
    [System.Serializable]
    public class TransformShake
    {
        public EZCameraShake.CameraShaker m_CameraShaker;

        public void ShakeOnce(TransformShakeData csd, float magnitudeMultiplier = 1, float roughnessMultiplier = 1)
        {
            m_CameraShaker.ShakeOnce(
                csd.m_Magnitude * magnitudeMultiplier,
                csd.m_Roughness * roughnessMultiplier,
                csd.m_FadeInTime,
                csd.m_FadeOutTime,
                csd.m_PosShake,
                csd.m_RotShake
            );
        }
    }

    public class PlayerShakeController : MonoBehaviour
    {
        [SerializeField] private TransformShakeData[] m_CameraShakeData;
        [SerializeField] private TransformShake[] m_CameraShakes;

        private readonly int ShakeTypeLength = System.Enum.GetValues(typeof(ShakeType)).Length;

        public void ShakeAllTransform(ShakeType shakeType, float magnitudeMultiplier = 1, float roughnessMultiplier = 1)
        {
            if ((int)shakeType >= ShakeTypeLength)
            {
                shakeType = ShakeType.Landing;
                Debug.LogWarning("Length diffrent");
            }

            foreach (TransformShake cs in m_CameraShakes)
                cs.ShakeOnce(m_CameraShakeData[(int)shakeType], magnitudeMultiplier, roughnessMultiplier);
        }

        public void ShakeCameraTransform(ShakeType shakeType)
            => m_CameraShakes[0].ShakeOnce(m_CameraShakeData[(int)shakeType]);
        
        public void ShakeBodyTransform(ShakeType shakeType)
            => m_CameraShakes[1].ShakeOnce(m_CameraShakeData[(int)shakeType]);
    }
}
