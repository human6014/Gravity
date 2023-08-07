using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit.Special
{
    public class SP2AnimationController : MonoBehaviour
    {
        private Animator m_Animator;

        #region AnimaionString
        private const string m_Walk = "Walk";
        private const string m_Die = "Die";
        #endregion

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }



    }
}
