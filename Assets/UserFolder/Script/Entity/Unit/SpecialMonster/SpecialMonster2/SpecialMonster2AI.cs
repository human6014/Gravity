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
        private float m_RotationSpeed = 45;

        public bool IsCloseToTarget { get; private set; }
        public bool CanMoveTarget { get; private set; }
        public float MovementSpeed { get => m_NavMeshAgent.speed; set => m_NavMeshAgent.speed = value; }
        public System.Action MoveCompToPos { get; set; }
        public System.Action RushCompToPos { get; set; }

        private void Awake() => m_NavMeshAgent = GetComponent<NavMeshAgent>();
        
        public void Init(float movementSpeed)
        {
            m_IsInit = true;
            m_NavMeshAgent.enabled = true;
            m_NavMeshAgent.speed = movementSpeed;
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
                    Debug.Log("PathPartical");
                    break;
                case NavMeshPathStatus.PathInvalid:
                    Debug.Log("PathInvalid");
                    break;
            }

            
            if (moveType == MoveType.RecoveryPos && Vector3.Distance(pos, transform.position) <= 3) MoveCompToPos?.Invoke();
            else if (moveType == MoveType.Rush && Vector3.Distance(pos,transform.position) <= 3) RushCompToPos?.Invoke();
        }

        public void RotateToPlayer()
        {
            Vector3 dir = (AIManager.PlayerTransform.position - transform.position);
            dir.y = 0;
            dir.Normalize();
            Quaternion targetRotation = Quaternion.LookRotation(dir);

            float step = m_RotationSpeed * Time.deltaTime; // 초당 회전할 각도
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        }

        public void Dispose()
        {
            m_IsInit = false;
            m_NavMeshAgent.enabled = false;
        }
    }
}
