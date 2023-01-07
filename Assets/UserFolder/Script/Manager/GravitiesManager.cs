using UnityEngine;
using System.Collections.Generic;
using System;
using EnumType;

namespace Manager
{
    public class GravitiesManager
    {
        public static GravitiesType currentGravityType = GravitiesType.yUp;
        public static GravitiesType beforeGravityType = GravitiesType.yUp;
        public static GravityDirection gravityDirection = GravityDirection.Y;
        /// <summary>
        /// 중력 방향 마우스 스크롤 아래  : -1 , 위 : 1
        /// </summary>
        public static float GravityDirectionValue { get; private set; } = -1;
        /// <summary>
        /// 중력 값 변경시 true, 플레이어 회전 끝나면 false
        /// </summary>
        public static bool IsGravityChange { get; private set; } = false;
        /// <summary>
        /// 중력 값 변경시 곂칠경우 true
        /// </summary>
        public static bool IsGravityDupleicated { get; private set; } = false;
        /// <summary>
        /// 중력 백터 일반화
        /// </summary>
        public static Vector3 GravityVector { get; private set; } = Vector3.down;
        
        /// <summary>
        /// 중력 변경, 중력이 향하는 방향이 아니라 반대임
        /// </summary>
        /// <param name="direct"></param>
        public static void GravityChange(int direct)
        {
            beforeGravityType = currentGravityType;
            GravityDirectionValue = direct;
            switch (gravityDirection)
            {
                case GravityDirection.X:
                    currentGravityType = direct < 0 ? GravitiesType.xUp : GravitiesType.xDown;
                    GravityVector = new Vector3(direct, 0, 0);
                    break;
                case GravityDirection.Y:
                    currentGravityType = direct < 0 ? GravitiesType.yUp : GravitiesType.yDown;
                    GravityVector = new Vector3(0, direct, 0);
                    break;
                case GravityDirection.Z:
                    currentGravityType = direct < 0 ? GravitiesType.zUp : GravitiesType.zDown;
                    GravityVector = new Vector3(0, 0, direct);
                    break;
            }
            if (beforeGravityType == currentGravityType) IsGravityDupleicated = true;
            else
            {
                Physics.gravity = GravityVector * Physics.gravity.magnitude;
                
                IsGravityChange = true;
                IsGravityDupleicated = false;
            }
            Debug.Log("current : " + currentGravityType);
        }

        public static void CompleteGravityChange() => IsGravityChange = false;
        
    }
}