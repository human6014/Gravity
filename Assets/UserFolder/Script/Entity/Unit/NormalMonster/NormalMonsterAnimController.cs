using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entity.Unit.Normal
{
    public class NormalMonsterAnimController : MonoBehaviour
    {
        protected Animator m_Animator;

        protected const string m_Walking = "Walking";
        protected const string m_Running = "Running";
        private const string m_CrawlMoving = "CrawlMoving";
        private const string m_Attack = "Attack";
        private const string m_GettingUp = "GettingUp";

        protected string m_BeforeState;

        private void Awake() => m_Animator = GetComponentInChildren<Animator>();

        public void Init()
        {
            m_BeforeState = m_Walking;
        }

        public virtual void PlayWalk(bool active)
        {
            m_Animator.SetBool(m_BeforeState,!active);
            m_Animator.SetBool(m_Walking, active);
            m_BeforeState = m_Walking;
        }

        public virtual void PlayRun(bool active)
        {
            m_Animator.SetBool(m_BeforeState, !active);
            m_Animator.SetBool(m_Running, active);
            m_BeforeState = m_Running;
        }

        public void PlayCrawl(bool active)
        {
            m_Animator.SetBool(m_BeforeState, !active);
            m_Animator.SetBool(m_CrawlMoving, active);
            m_BeforeState = m_CrawlMoving;
        }

        public void PlayAttack()
        {
            m_Animator.SetTrigger(m_Attack);
        }

        public void PlayGettingUp()
        {
            m_Animator.SetTrigger(m_GettingUp);
        }
    }
}
