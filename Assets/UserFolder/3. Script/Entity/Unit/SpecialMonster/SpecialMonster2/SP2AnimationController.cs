using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace Entity.Unit.Special
{
    public class SP2AnimationController : MonoBehaviour
    {
        private Animator m_Animator;

        private bool m_DoNormalAttacking;
        private bool m_DoCriticalHitting;

        #region AnimaionString
        private const string m_Walk = "Walk";
        private const string m_Roar = "Roar";
        private const string m_Death = "Death";
        private const string m_Grab = "Grab";

        private const string m_NormalAttack = "NormalAttack";
        private const string m_CriticalHit = "CriticalHit";

        private const string m_MovementSpeed = "MovementSpeed";
        private const string m_IdleSpeed = "IdleSpeed";
        #endregion

        public System.Action DoDamageAction { get; set; }

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        public void SetWalk(bool isActive)
        {
            m_Animator.SetBool(m_Walk, isActive);
        }

        public void SetRoar(bool isActive)
        {
            m_Animator.SetBool(m_Roar, isActive);
        }

        public void SetGrab(bool isActive)
        {
            m_Animator.SetBool(m_Grab, isActive);
        }

        public void SetDeath(bool isActive)
        {
            m_Animator.SetBool(m_Death, isActive);
        }

        public Task SetNormalAttack()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            m_DoNormalAttacking = true;
            m_Animator.SetTrigger(m_NormalAttack);
            StartCoroutine(CheckForEndNormalAttack(tcs));
            return tcs.Task;
        }

        public Task SetCriticalHit()
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            m_DoCriticalHitting = true;
            m_Animator.SetTrigger(m_CriticalHit);
            StartCoroutine(CheckForEndCriticalHit(tcs));

            return tcs.Task;
        }

        public void SetMovementSpeed(float value) 
            => m_Animator.SetFloat(m_MovementSpeed, value);

        public void SetIdleSpeed(float value) => m_Animator.SetFloat(m_IdleSpeed, value);
        
        private IEnumerator CheckForEndNormalAttack(TaskCompletionSource<bool> tcs)
        {
            while (m_DoNormalAttacking) yield return null;
            tcs.SetResult(true);
        }

        private IEnumerator CheckForEndCriticalHit(TaskCompletionSource<bool> tcs)
        {
            while (m_DoCriticalHitting) yield return null;
            tcs.SetResult(true);
        }

        #region Animation End Event
#pragma warning disable IDE0051 // 사용되지 않는 private 멤버 제거
        private void DoDamage() => DoDamageAction?.Invoke();
        private void EndNormalAttack() => m_DoNormalAttacking = false;
        private void EndCriticalHit() => m_DoCriticalHitting = false;
#pragma warning restore IDE0051
        #endregion
    }
}
