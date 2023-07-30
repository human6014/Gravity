using UnityEngine;
using System.Collections.Generic;
using System;
using EnumType;
using System.Collections;

namespace Manager
{
    public class GravityManager : MonoBehaviour
    {
        [SerializeField] private Contoller.PlayerInputController m_PlayerInputController;

        public static GravityType m_CurrentGravityType = GravityType.yDown;
        public static GravityType m_BeforeGravityType = GravityType.yDown;
        public static GravityDirection m_CurrentGravityAxis = GravityDirection.Y;

        private const float m_RotateTime = 1;

        public static Action <GravityType> GravityChangeAction { get; set; }

        /// <summary>
        /// 중력 방향 마우스 스크롤 아래  : -1 , 위 : 1
        /// </summary>
        public static float GravityDirectionValue { get; private set; } = -1;

        /// <summary>
        /// 중력 값 변경시 플레이어 회전 중일 때 true, 플레이어 회전 끝나면 false
        /// </summary>
        public static bool IsGravityChanging { get; private set; } = false;

        /// <summary>
        /// 중력 값 변경시 곂칠경우 true
        /// </summary>
        public static bool IsGravityDupleicated { get; private set; } = false;

        /// <summary>
        /// 중력 백터 일반화
        /// </summary>
        public static Vector3 GravityVector { get; private set; } = Vector3.down;

        /// <summary>
        /// 현재 중력에 해당하는 Area의 normal 각도
        /// </summary>
        private static readonly Vector3[] m_GravityNormalRotation =
        {
            new Vector3(0, 0, -90)   , new Vector3(0, 0, 90),
            new Vector3(0, 0, 0)     , new Vector3(180, 0, 0),
            new Vector3(90, 0, 0)    , new Vector3(-90, 0, 0),
        };

        private void Awake()
        {
            m_PlayerInputController.DoGravityChange += GravityChange;
        }

        public void GravityChange(int gravityKeyInput, float mouseScroll)
        {
            if (IsGravityChanging) return;

            m_CurrentGravityAxis = (GravityDirection)gravityKeyInput;
            GravityChange(Mathf.FloorToInt(mouseScroll * 10));
            if(!IsGravityDupleicated) StartCoroutine(GravityRotate());
        }

        /// <summary>
        /// 중력 변경, 중력이 향하는 방향을 기록함
        /// </summary>
        /// <param name="direct"></param>
        private void GravityChange(int direct)
        {
            m_BeforeGravityType = m_CurrentGravityType;
            GravityDirectionValue = direct;
            switch (m_CurrentGravityAxis)
            {
                case GravityDirection.X:
                    m_CurrentGravityType = direct < 0 ? GravityType.xDown : GravityType.xUp;
                    GravityVector = new Vector3(direct, 0, 0);
                    break;
                case GravityDirection.Y:
                    m_CurrentGravityType = direct < 0 ? GravityType.yDown : GravityType.yUp;
                    GravityVector = new Vector3(0, direct, 0);
                    break;
                case GravityDirection.Z:
                    m_CurrentGravityType = direct < 0 ? GravityType.zDown : GravityType.zUp;
                    GravityVector = new Vector3(0, 0, direct);
                    break;
            }

            if (m_BeforeGravityType == m_CurrentGravityType) IsGravityDupleicated = true;
            else
            {
                GravityChangeAction?.Invoke(m_CurrentGravityType);
                Physics.gravity = GravityVector * Physics.gravity.magnitude;

                IsGravityChanging = true;
                IsGravityDupleicated = false;
            }
        }

        private IEnumerator GravityRotate()
        {
            Quaternion currentRotation = transform.rotation;
            float t = 0;
            while (t <= m_RotateTime)
            {
                t += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(currentRotation, GetCurrentGravityNormalRotation(), t / m_RotateTime);
                yield return null;
            }
            transform.rotation = GetCurrentGravityNormalRotation();
            IsGravityChanging = false;
        }

        public static Vector3 GetCurrentGravityNoramlDirection() => m_GravityNormalRotation[(int)m_CurrentGravityType];

        public static Quaternion GetSpecificGravityNormalRotation(int index) => Quaternion.Euler(m_GravityNormalRotation[index]);
        
        private Quaternion GetCurrentGravityNormalRotation() => Quaternion.Euler(m_GravityNormalRotation[(int)m_CurrentGravityType]);
    }
}