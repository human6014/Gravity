using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager.AI;
using Scriptable.Monster;

namespace Entity.Unit.Special
{
    public class SpecialMonster : MonoBehaviour
    {
        [SerializeField] protected UnitScriptable m_UnitScriptable;
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

        protected void SetRealStat(float statMultiplier)
        {
            m_IsAlive = true;

            m_RealMaxHP = m_UnitScriptable.m_HP + (int)(statMultiplier * m_UnitScriptable.m_HPMultiplier);
            m_RealDef = m_UnitScriptable.m_Def + (int)(statMultiplier * m_UnitScriptable.m_DefMultiplier);
            m_RealDamage = (int)(statMultiplier * m_UnitScriptable.m_DamageMultiplier);

            m_CurrentHP = m_RealMaxHP;
        }
    }
}
