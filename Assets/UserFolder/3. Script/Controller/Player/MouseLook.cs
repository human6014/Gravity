using System;
using UnityEngine;

namespace Controller.Player.Utility
{
    [Serializable]
    public class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90f;
        public float MaximumX = 90f;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;

        private bool m_HasData;

        private float m_RightAxisRecoil;
        private float m_UpAxisRecoil;
        private float m_RecoilSpeed = 5f;

        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;

        private Transform m_TargetCharacter;    //y
        private Transform m_TargetCamera;       //x

        private PlayerState m_PlayerState;
        private GameControlSetting m_GameControlSetting;

        public void Setup(Transform character, Transform camera, PlayerState playerState)
        {
            m_TargetCharacter = character;
            m_TargetCamera = camera;
            m_PlayerState = playerState;

            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;

            if (DataManager.Instance == null) m_HasData = false;
            else
            {
                m_HasData = true;
                m_GameControlSetting = (GameControlSetting)DataManager.Instance.Settings[1];
            }
        }

        public void LookRotation(float mouseHorizontal, float mouseVertical)
        {
            float xSensitivity = XSensitivity;
            float ySensitivity = YSensitivity;
            if (m_HasData)
            {
                if(m_PlayerState.PlayerWeaponState == PlayerWeaponState.Aiming)
                {
                    xSensitivity = m_GameControlSetting.m_AimSensitivity;
                    ySensitivity = m_GameControlSetting.m_AimSensitivity;
                }
                else
                {
                    xSensitivity = m_GameControlSetting.m_LookSensitivity;
                    ySensitivity = m_GameControlSetting.m_LookSensitivity;
                }
            }

            float yRot = m_RightAxisRecoil + mouseHorizontal * xSensitivity;
            float xRot = m_UpAxisRecoil + mouseVertical * ySensitivity;

            m_RightAxisRecoil -= m_RecoilSpeed * Time.deltaTime;
            m_UpAxisRecoil -= m_RecoilSpeed * Time.deltaTime;

            if (m_RightAxisRecoil < 0) m_RightAxisRecoil = 0;
            if (m_UpAxisRecoil < 0) m_UpAxisRecoil = 0;

            m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

            if (clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

            if (smooth)
            {
                m_TargetCharacter.localRotation = Quaternion.Lerp(m_TargetCharacter.localRotation, m_CharacterTargetRot, smoothTime * Time.deltaTime);
                m_TargetCamera.localRotation = Quaternion.Lerp(m_TargetCamera.localRotation, m_CameraTargetRot, smoothTime * Time.deltaTime);
            }
            else
            {
                m_TargetCharacter.localRotation = m_CharacterTargetRot;
                m_TargetCamera.localRotation = m_CameraTargetRot;
            }
        }

        public void LookRotation(Transform lookPoint, Transform player)
        {
            m_TargetCharacter.localRotation = Quaternion.identity;
            m_TargetCamera.localRotation = Quaternion.identity;

            Vector3 dir = lookPoint.position - player.position;

            player.rotation = Quaternion.LookRotation(dir);
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
            m_UpAxisRecoil += up;
            m_RightAxisRecoil += right;
        }
    }
}
