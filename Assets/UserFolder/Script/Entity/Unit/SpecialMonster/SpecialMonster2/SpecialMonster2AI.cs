using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Manager.AI;

namespace Entity.Unit.Special
{
    public enum MoveType
    {
        ToPlayer,
        Self,
        RecoveryPos,
        Rush
    }
    public class SpecialMonster2AI : MonoBehaviour
    {
        private NavMeshAgent m_NavMeshAgent;

        private bool m_IsInit;

        public bool IsCloseToTarget { get; private set; }

        public bool CanMoveTarget { get; private set; }

        public System.Action MoveCompToPos { get; set; }

        private void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void Init()
        {
            m_IsInit = true;
        }

        public void OperateAIBehavior(Vector3 pos, MoveType moveType)
        {
            if (!m_IsInit) return;
            m_NavMeshAgent.SetDestination(pos);

            switch (m_NavMeshAgent.pathStatus)
            {
                case NavMeshPathStatus.PathComplete:
                    CanMoveTarget = true;
                    break;
                case NavMeshPathStatus.PathPartial:
                    CanMoveTarget = false;
                    break;
                case NavMeshPathStatus.PathInvalid:
                    Debug.Log("PathInvalid");
                    break;
            }

            if (moveType == MoveType.RecoveryPos && m_NavMeshAgent.remainingDistance <= 3) MoveCompToPos?.Invoke();
        }


        public void Dispose()
        {
            m_IsInit = false;
            m_NavMeshAgent.enabled = false;
        }
    }
}
