
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
        /// �ش� navMeshAgent�� ���� �߷¿� �´� ������ ��� �ִ��� ���մϴ�
        /// </summary>
        /// <param name="navMeshAgent">�Ǻ��� navMeshAgent </param>
        /// <returns>true : ���� �߷¿� �´� ��, false : �ٸ� ��</returns>
        public static bool IsSameFloor(NavMeshAgent navMeshAgent)
        {
            if (!navMeshAgent.isOnNavMesh) return false;
            return navMeshAgent.navMeshOwner.name == FloorDetector.GetNowFloor().name;
        }

        /// <summary>
        /// ���� �߷��� ���� �ش��ϴ� ���� �����ϰ� 2������ ��ġ�� ���մϴ�.
        /// </summary>
        /// <param name="transform">��� ��ġ�� �ش��ϴ� transform</param>
        /// <returns>���� ���� �� ���� ��� ��ȿ�� Vector3 ��, ������ ��� Vector3.zero</returns>
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
