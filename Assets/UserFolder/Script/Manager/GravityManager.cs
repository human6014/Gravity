using UnityEngine;
using System.Collections.Generic;
using System;
using EnumType;

namespace Manager
{
    public class GravityManager
    {
        public static GravityType currentGravityType = GravityType.yDown;
        public static GravityType beforeGravityType = GravityType.yDown;
        public static GravityDirection gravityDirection = GravityDirection.Y;

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
        private static readonly Vector3Int[] gravityRotation =
    {
            new Vector3Int(0, 0, -90)   , new Vector3Int(0, 0, 90),
            new Vector3Int(0, 0, 0)     , new Vector3Int(180, 0, 0),
            new Vector3Int(90, 0, 0)   , new Vector3Int(-90, 0, 0),
        };

        /// <summary>
        /// 중력 변경, 중력이 향하는 방향을 기록함
        /// </summary>
        /// <param name="direct"></param>
        public static void GravityChange(int direct)
        {
            beforeGravityType = currentGravityType;
            GravityDirectionValue = direct;
            switch (gravityDirection)
            {
                case GravityDirection.X:
                    currentGravityType = direct < 0 ? GravityType.xDown : GravityType.xUp;
                    GravityVector = new Vector3(direct, 0, 0);
                    break;
                case GravityDirection.Y:
                    currentGravityType = direct < 0 ? GravityType.yDown : GravityType.yUp;
                    GravityVector = new Vector3(0, direct, 0);
                    break;
                case GravityDirection.Z:
                    currentGravityType = direct < 0 ? GravityType.zDown : GravityType.zUp;
                    GravityVector = new Vector3(0, 0, direct);
                    break;
            }
            if (beforeGravityType == currentGravityType) IsGravityDupleicated = true;
            else
            {
                GravityChangeAction(currentGravityType);
                Physics.gravity = GravityVector * Physics.gravity.magnitude;

                IsGravityChanging = true;
                IsGravityDupleicated = false;
            }
        }

        public static void CompleteGravityChanging() => IsGravityChanging = false;

        public static Vector3Int GetCurrentGravityNoramlDirection() => gravityRotation[(int)currentGravityType];

        public static Vector3Int GetSpecificGravityNormalDirection(int index) => gravityRotation[index];
        
        public static Quaternion GetCurrentGravityNormalRotation() => Quaternion.Euler(gravityRotation[(int)currentGravityType]);
    }
}