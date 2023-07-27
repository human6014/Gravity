using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Entity.Unit.Special
{
    public class SP3AnimationController : MonoBehaviour
    {
        private Animator m_Animator;

        #region Animation string

        private const string m_Move = "Move";
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
    }
}
