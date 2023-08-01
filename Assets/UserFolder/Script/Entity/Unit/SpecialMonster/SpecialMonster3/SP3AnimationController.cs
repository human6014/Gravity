using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Entity.Unit.Special
{
    public class SP3AnimationController : MonoBehaviour
    {
        private Animator m_Animator;

        public System.Action EndDieHitGroundAnimation { get; set; }

        #region Animation string
        private const string m_Attack = "Attacking";
        private const string m_Die = "Die";
        private const string m_DieGround = "DieGround";
        #endregion

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        public void Init()
        {

        }

        private void SetTrigger(string name)
        {
            m_Animator.SetTrigger(name);
        }

        private void SetBool(string name, bool isActive)
        {
            m_Animator.SetBool(name, isActive);
        }

        public void Attack()
        {
            SetTrigger(m_Attack);
        }

        public void Die()
        {
            SetBool(m_Die, true);
        }

        public void DieHitGround()
        {
            SetBool(m_DieGround, true);
        }

        public void EndDieHitGround()
        {
            EndDieHitGroundAnimation?.Invoke();
        }
    }
}
