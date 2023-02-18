
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Contoller.Floor;
using EnumType;

namespace Manager.AI
{
    public class AIManager : MonoBehaviour
    {
        public static FloorController FloorDetector { get; set; }
        public static Transform PlayerTransfrom { get; set; }
        public static Vector3 PlayerRerversePosition { get; set; }

        /// <summary>
        /// 해당 navMeshAgent가 현재 중력에 맞는 지상을 밝고 있는지 구합니다
        /// </summary>
        /// <param name="navMeshAgent">판별할 navMeshAgent </param>
        /// <returns>true : 현재 중력에 맞는 땅, false : 다른 땅</returns>
        public static bool IsSameFloor(NavMeshAgent navMeshAgent)
        {
            if (!navMeshAgent.isOnNavMesh) return false;
            return navMeshAgent.navMeshOwner.name == FloorDetector.GetNowFloor().name;
        }

        /// <summary>
        /// 현재 중력의 방향 해당하는 축을 제외하고 2차원적 위치를 구합니다.
        /// </summary>
        /// <param name="transform">평면 위치에 해당하는 transform</param>
        /// <returns>값을 구할 수 있을 경우 유효한 Vector3 값, 오류일 경우 Vector3.zero</returns>
        public static Vector3 CurrentTargetPosition(Transform transform)
        {
            Vector3 playerPos = PlayerTransfrom.position;
            switch (GravitiesManager.gravityDirection)
            {
                case GravityDirection.X: return new(transform.position.x, playerPos.y, playerPos.z);
                case GravityDirection.Y: return new(playerPos.x, transform.position.y, playerPos.z);
                case GravityDirection.Z: return new(playerPos.x, playerPos.y, transform.position.z);
                default: break;
            }
            return Vector3.zero;
        }
    }
}
