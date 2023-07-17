using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit.Special
{
    public class SpecialMonster3 : MonoBehaviour, IMonster
    {
        [SerializeField] [Range(0, 2000)] private int m_BoidsInstacingCount = 500;

        private BoidsController m_BoidsController;
        private Animator m_Animator;

        private void Awake()
        {
            m_BoidsController = GetComponent<BoidsController>();
            //animator = GetComponent<Animator>();
        }

        void Start()
        {
            m_BoidsController.GenerateBoidMonster(m_BoidsInstacingCount);
        }

        public void Attack()
        {
            throw new System.NotImplementedException();
        }

        public void Die()
        {
            throw new System.NotImplementedException();
        }

        public void Hit(int damage, AttackType bulletType)
        {
            throw new System.NotImplementedException();
        }

        public void Move()
        {
            throw new System.NotImplementedException();
        }
    }
}
