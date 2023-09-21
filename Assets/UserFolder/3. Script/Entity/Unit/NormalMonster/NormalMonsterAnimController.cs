using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Unit.Normal
{
    public class NormalMonsterAnimController : MonoBehaviour
    {
        protected Animator m_Animator;

        #region Animation string
        private const string m_Walking = "Walking";
        private const string m_Running = "Running";
        private const string m_CrawlMoving = "CrawlMoving";
        private const string m_Attack = "Attack";
        private const string m_GettingUp = "GettingUp";
        private const string m_GettingUpSpeed = "GettingUpSpeed";
        #endregion
        protected string m_BeforeState;

        private bool IsEndAttack;
        private bool IsEndGettingUp;

        public System.Action DoDamageAction { get; set; }

        private void Awake() => m_Animator = GetComponent<Animator>();

        public void Init()
        {
            m_BeforeState = m_Walking;

            IsEndAttack = true;
            IsEndGettingUp = true;
        }

        public void PlayIdle()
        {
            m_Animator.SetBool(m_BeforeState, false);
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

        #region Using tcs
        public Task PlayAttack()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            m_Animator.SetTrigger(m_Attack);
            IsEndAttack = false;
            StartCoroutine(CheckForEndAttack(tcs));
            return tcs.Task;
        }

        public Task PlayGettingUp()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            m_Animator.SetFloat(m_GettingUpSpeed, Random.Range(0.75f, 1.25f));
            m_Animator.SetTrigger(m_GettingUp);
            IsEndGettingUp = false;
            StartCoroutine(CheckForEndGettingUp(tcs));
            return tcs.Task;
        }
        #endregion

        #region CheckForEndAnimation
        private IEnumerator CheckForEndAttack(TaskCompletionSource<bool> tcs)
        {
            while (!IsEndAttack) yield return null;
            tcs.SetResult(true);
        }

        private IEnumerator CheckForEndGettingUp(TaskCompletionSource<bool> tcs)
        {
            while (!IsEndGettingUp) yield return null;
            m_Animator.ResetTrigger(m_GettingUp);
            tcs.SetResult(true);
        }
        #endregion

        #region Animation Event
        private void DoDamage() => DoDamageAction?.Invoke();
        
        private void EndAttack() => IsEndAttack = true;
        
        private void EndGettingUp() => IsEndGettingUp = true;
        #endregion
    }
}
