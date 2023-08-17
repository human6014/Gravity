using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.AI;

namespace Entity.Unit.Special
{
    public class SpecialMonster : MonoBehaviour
    {
        protected bool m_IsAlive;

        protected int m_RealMaxHP;
        protected int m_RealDef;
        protected int m_RealDamage;
        protected int m_CurrentHP;

        public System.Action EndSpecialMonsterAction { get; set; }

        protected bool DetectObstacle(Vector3 pos, float dist, LayerMask obstacleLayer)
        {
            Vector3 dir = (AIManager.PlayerTransform.position - pos).normalized;

            bool isHit = Physics.Raycast(pos, dir, out RaycastHit hit, dist, obstacleLayer);
            if (!isHit) return true;
            return hit.transform.gameObject.layer != AIManager.PlayerLayerNum;
        }
    }
}
