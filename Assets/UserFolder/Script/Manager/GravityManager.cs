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
        [SerializeField] private Transform m_RotateControllingObject;

        public List<Transform> SyncRotatingTransform { get; set; } = new List<Transform>();
        private const float m_RotateTime = 1;
        private bool m_IsGravityDupleicated;

        private GravityType m_BeforeGravityType { get; set; } = GravityType.yDown;
        public static GravityType m_CurrentGravityType { get; set; } = GravityType.yDown;
        public static GravityDirection m_CurrentGravityAxis { get; set; } = GravityDirection.Y;

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
        /// 중력 백터 일반화
        /// </summary>
        public static Vector3 GravityVector { get; private set; } = Vector3.down;

        /// <summary>
        /// 현재 중력에 해당하는 Area 각도
        /// </summary>
        private static readonly Vector3Int[] m_GravityRotation =
        {
            new Vector3Int(0, 0, -90)   , new Vector3Int(0, 0, 90),
            new Vector3Int(0, 0, 0)     , new Vector3Int(180, 0, 0),
            new Vector3Int(90, 0, 0)    , new Vector3Int(-90, 0, 0),
        };

        private static readonly Vector3Int[] m_GravityNormalDirection =
        {
            new Vector3Int(1,0,0), new Vector3Int(-1,0,0),
            new Vector3Int(0,1,0), new Vector3Int(0,-1,0),
            new Vector3Int(0,0,1), new Vector3Int(0,0,-1),
        };

        private void Awake()
        {
            GravityChangeAction = null;
            m_PlayerInputController.DoGravityChange += GravityChange;

            m_CurrentGravityType = GravityType.yDown;
            m_CurrentGravityAxis = GravityDirection.Y;
            GravityDirectionValue = -1;
            IsGravityChanging = false;
            GravityVector = Vector3.down;
        }

        public void GravityChange(int gravityKeyInput, float mouseScroll)
        {
            if (IsGravityChanging) return;

            m_CurrentGravityAxis = (GravityDirection)gravityKeyInput;
            GravityChange(Mathf.FloorToInt(mouseScroll * 10));
            if (!m_IsGravityDupleicated)
            {
                //StartCoroutine(GravityRotateTransform());
                StartCoroutine(GravityRotate(m_RotateControllingObject));
                StartCoroutine(GravityRotate(AI.AIManager.PlayerTransform));
            }
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

            if (m_BeforeGravityType == m_CurrentGravityType) m_IsGravityDupleicated = true;
            else
            {
                GravityChangeAction?.Invoke(m_CurrentGravityType);
                Physics.gravity = GravityVector * Physics.gravity.magnitude;

                IsGravityChanging = true;
                m_IsGravityDupleicated = false;
            }
        }

        private IEnumerator GravityRotateTransform()
        {
            Quaternion currentRotation = SyncRotatingTransform[0].rotation;
            Quaternion targetRotation = GetCurrentGravityRotation();
            float elapsedTime = 0;
            while (elapsedTime < m_RotateTime)
            {
                elapsedTime += Time.deltaTime;
                foreach(Transform t in SyncRotatingTransform)
                    t.rotation = Quaternion.Lerp(currentRotation, targetRotation, elapsedTime / m_RotateTime);
                yield return null;
            }
            IsGravityChanging = false;
        }

        private IEnumerator GravityRotate(Transform target)
        {
            Quaternion currentRotation = target.rotation;
            float t = 0;
            while (t < m_RotateTime)
            {
                t += Time.deltaTime / m_RotateTime;
                target.rotation = Quaternion.Lerp(currentRotation, GetCurrentGravityRotation(), t);
                yield return null;
            }
            IsGravityChanging = false;
        }

        public static Vector3Int GetCurrentGravityNormalDirection() => m_GravityNormalDirection[(int)m_CurrentGravityType];
        public static Vector3Int GetCurrentGravityDirection() => m_GravityRotation[(int)m_CurrentGravityType];

        public static Quaternion GetSpecificGravityRotation(int index) => Quaternion.Euler(m_GravityRotation[index]);
        
        public static Quaternion GetCurrentGravityRotation() => Quaternion.Euler(m_GravityRotation[(int)m_CurrentGravityType]);
    }
}