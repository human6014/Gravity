using System;
using UnityEngine;

namespace Contoller.Player.Utility
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;

        private float rightAxisRecoil;
        private float upAxisRecoil;
        private float recoilSpeed = 5f;

        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;

        public void Setup(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }

        public void LookRotation(Transform character, Transform camera, float mouseHorizontal, float mouseVertical)
        {
            float yRot = rightAxisRecoil + mouseHorizontal * XSensitivity;
            float xRot = upAxisRecoil + mouseVertical * YSensitivity;

            rightAxisRecoil -= recoilSpeed * Time.deltaTime;
            upAxisRecoil -= recoilSpeed * Time.deltaTime;

            if (rightAxisRecoil < 0) rightAxisRecoil = 0;
            if (upAxisRecoil < 0) upAxisRecoil = 0;

            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                character.localRotation = Quaternion.Lerp
                    (character.localRotation, m_CharacterTargetRot, smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Lerp
                    (camera.localRotation, m_CameraTargetRot, smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }
        }

        private Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        public void AddRecoil(float up, float right)
        {
            upAxisRecoil += up;
            rightAxisRecoil += right;
        }
    }
}
