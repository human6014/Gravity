using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Threading.Tasks;

public class SP1AnimationController : MonoBehaviour
{
    [SerializeField] Animator []rigAnimators;
    [SerializeField] RigBuilder []rigBuilders;

    private Animator m_Animator;

    private bool m_DoCrawsAttacking;
    private bool m_DoGrabAttackingReverse;
    private bool m_DoGrabAttacking;
    private bool m_DoJumpBiteAttacking;

    private bool m_DoHitting;
    private bool m_DoRoaring;
    
    #region Animation string
    private const string m_ClawsAttack = "ClawsAttack";
    private const string m_GrabAttack = "BiteAttack";

    private const string m_Walk = "Walk";
    private const string m_Roar = "Roar";
    private const string m_Hit = "GetHit";
    private const string m_Die = "Die";

    private const string m_StartJumpAttack = "StartJump";
    private const string m_EndJumpAttack = "EndJump";
    #endregion

    public System.Action DoDamageAction { get; set; }

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    public void SetIKEnable(bool _value)
    {
        //legController.SetIKEnable(_value);
        for(int i = 0; i < rigAnimators.Length; i++)
        {
            rigAnimators[i].enabled = _value;
            rigBuilders[i].enabled = _value;
        }
    }

    /*
    public void SetIKEnableExceptNeck(bool _value)
    {
        for (int i = 0; i < rigAnimators.Length - 1; i++)
        {
            rigAnimators[i].enabled = _value;
            rigBuilders[i].enabled = _value;
        }
    }
    */

    public void SetWalk(bool isActiveIK)
    {
        m_Animator.SetBool(m_Walk, isActiveIK);
        SetIKEnable(isActiveIK);
    }

    private void SetTriggerAnimation(string name)
    {
        SetWalk(false);
        m_Animator.SetTrigger(name);
    }

    public void SetDie()
    {
        SetTriggerAnimation(m_Die);
    }

    public void SetJumpBiteAttack()
    {
        m_DoJumpBiteAttacking = true;
        SetIKEnable(false);
        m_Animator.SetTrigger(m_StartJumpAttack);
    }

    public void EndJumpBiteAttack()
    {
        m_Animator.SetTrigger(m_EndJumpAttack);
        SetIKEnable(true);
        m_DoJumpBiteAttacking = false;
    }

    #region Using TCS
    public Task SetClawsAttack()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        m_DoCrawsAttacking = true;

        SetTriggerAnimation(m_ClawsAttack);
        StartCoroutine(CheckForEndClawsAttack(tcs));

        return tcs.Task;
    }

    public Task SetGrabAttack()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        m_DoGrabAttacking = true;
        m_DoGrabAttackingReverse = true;

        SetTriggerAnimation(m_GrabAttack);
        StartCoroutine(CheckForEndBiteAttack(tcs));

        return tcs.Task;
    }

    public Task SetHit()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        m_DoHitting = true;
        SetTriggerAnimation(m_Hit);
        StartCoroutine(CheckForEndHit(tcs));

        return tcs.Task;
    }

    public void SetRoar()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        m_DoRoaring = true;
        SetTriggerAnimation(m_Roar);
        StartCoroutine(CheckForEndRoar(tcs));
    }
    #endregion
    #region CheckForEndAnimation
    private IEnumerator CheckForEndClawsAttack(TaskCompletionSource<bool> tcs)
    {
        while (m_DoCrawsAttacking) yield return null;
        tcs.SetResult(true);
    }

    private IEnumerator CheckForEndBiteAttack(TaskCompletionSource<bool> tcs)
    {
        while (m_DoGrabAttackingReverse) yield return null;
        tcs.SetResult(true);
    }
    private IEnumerator CheckForEndHit(TaskCompletionSource<bool> tcs)
    {
        while (m_DoHitting) yield return null;
        tcs.SetResult(true);
    }

    private IEnumerator CheckForEndRoar(TaskCompletionSource<bool> tcs)
    {
        while (m_DoRoaring) yield return null;
        tcs.SetResult(true);
    }
    #endregion
    #region Animation End Event
#pragma warning disable IDE0051 // 사용되지 않는 private 멤버 제거
    private void DoDamage() => DoDamageAction?.Invoke();
    private void EndCrawsAttack() => m_DoCrawsAttacking = false;
    private void EndBiteAttack() => m_DoGrabAttacking = false;
    private void EndBiteAttackReverse() => m_DoGrabAttackingReverse = false;
    private void EndRoar() => m_DoRoaring = false;
    private void EndHit() => m_DoHitting = false;
#pragma warning restore IDE0051
    #endregion
}
