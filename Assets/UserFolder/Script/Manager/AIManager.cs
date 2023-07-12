
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
        public static Transform PlayerTransform { get; set; }
        public static Vector3 PlayerRerversePosition { get; set; }
        public static Transform PlayerSupportTargetTransform { get; set; }
        public static bool PlayerIsGround { get; set; }

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
        public static Vector3 CurrentTargetPosition(Vector3 direction)
        {
            switch (GravityManager.m_CurrentGravityAxis)
            {
                case GravityDirection.X:
                    direction.x = 0;
                    break;
                case GravityDirection.Y:
                    direction.y = 0;
                    break;
                case GravityDirection.Z:
                    direction.z = 0;
                    break;
            }
            return direction;
        }
    }
}
