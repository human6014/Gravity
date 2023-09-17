using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Manager
{
    public enum GravityType
    {
        xDown = 0,
        xUp = 1,
        yDown = 2,
        yUp = 3,
        zDown = 4,
        zUp = 5
    }
    public enum GravityDirection
    {
        X, Y, Z
    }

    public class GravityManager : MonoBehaviour
    {
        [SerializeField] private Controller.PlayerInputController m_PlayerInputController;
        [SerializeField] private List<Transform> SyncRotatingTransform;

        private const float m_RotateTime = 1;
        private bool m_IsGravityDupleicated;

        private GravityType BeforeGravityType { get; set; } = GravityType.yDown;

        #region Static Field
        public static GravityType CurrentGravityType { get; set; } = GravityType.yDown;
        public static GravityDirection CurrentGravityAxis { get; set; } = GravityDirection.Y;

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
        /// 중력 변경 강제로 막을 때 사용
        /// 중력 변경을 할 수 없을 때 true
        /// </summary>
        public static bool CantGravityChange { get; set; } = false;

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

        public static Vector3Int GetCurrentGravityNormalDirection() 
            => m_GravityNormalDirection[(int)CurrentGravityType];

        //public static Vector3Int GetCurrentGravityDirection()
        //  => m_GravityRotation[(int)CurrentGravityType];

        public static Quaternion GetSpecificGravityRotation(int index) 
            => Quaternion.Euler(m_GravityRotation[index]);

        public static Quaternion GetCurrentGravityRotation() 
            => Quaternion.Euler(m_GravityRotation[(int)CurrentGravityType]);
        #endregion

        private void Awake()
        {
            GravityChangeAction = null;

            CurrentGravityType = GravityType.yDown;
            CurrentGravityAxis = GravityDirection.Y;
            GravityDirectionValue = -1;
            IsGravityChanging = false;
            GravityVector = Vector3.down;
        }

        public bool GravityChange(int gravityKeyInput, float mouseScroll)
        {
            if (IsGravityChanging) return true;
            if (CantGravityChange) return true;

            CurrentGravityAxis = (GravityDirection)gravityKeyInput;
            GravityChange(Mathf.FloorToInt(mouseScroll * 10));
            if (!m_IsGravityDupleicated)
                StartCoroutine(GravityRotateTransform());
            
            return m_IsGravityDupleicated;
        }

        /// <summary>
        /// 중력 변경, 중력이 향하는 방향을 기록함
        /// </summary>
        /// <param name="direct"></param>
        private void GravityChange(int direct)
        {
            BeforeGravityType = CurrentGravityType;
            GravityDirectionValue = direct;
            switch (CurrentGravityAxis)
            {
                case GravityDirection.X:
                    CurrentGravityType = direct < 0 ? GravityType.xDown : GravityType.xUp;
                    GravityVector = new Vector3(direct, 0, 0);
                    break;
                case GravityDirection.Y:
                    CurrentGravityType = direct < 0 ? GravityType.yDown : GravityType.yUp;
                    GravityVector = new Vector3(0, direct, 0);
                    break;
                case GravityDirection.Z:
                    CurrentGravityType = direct < 0 ? GravityType.zDown : GravityType.zUp;
                    GravityVector = new Vector3(0, 0, direct);
                    break;
            }

            if (BeforeGravityType == CurrentGravityType) m_IsGravityDupleicated = true;
            else
            {
                GravityChangeAction?.Invoke(CurrentGravityType);
                Physics.gravity = GravityVector * 9.81f;

                IsGravityChanging = true;
                m_IsGravityDupleicated = false;
            }
        }

        private IEnumerator GravityRotateTransform()
        {
            Quaternion currentRotation = SyncRotatingTransform[0].rotation;
            Quaternion targetRotation = GetCurrentGravityRotation();
            float elapsedTime = 0;
            float t;
            while (elapsedTime < m_RotateTime)
            {
                elapsedTime += Time.deltaTime;
                t = elapsedTime / m_RotateTime;
                foreach (Transform tf in SyncRotatingTransform)
                    tf.rotation = Quaternion.Lerp(currentRotation, targetRotation, t);
                yield return null;
            }
            IsGravityChanging = false;
        }
    }
}