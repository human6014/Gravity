using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumType;
using UnityEngine.Animations.Rigging;
using System.Threading.Tasks;

public class SP1AnimationController : MonoBehaviour
{
    [SerializeField] Animator []rigAnimators;
    [SerializeField] RigBuilder []rigBuilders;
    private LegController legController;
    private Animator m_Animator;


    private bool m_DoCrawsAttacking;
    private bool m_DoGrabAttacking;
    private bool m_DoGrabAttackingReverse;
    private bool m_DoJumpBiteAttacking;
    private bool m_DoSpiting;

    private bool m_DoHitting;
    private bool m_DoRoaring;
    
    #region Animation
    private const string m_ClawsAttack = "ClawsAttack";
    private const string m_GrabAttack = "BiteAttack";
    private const string m_SpitVenom = "SpitVenom";

    private const string m_Walk = "Walk";
    private const string m_Roar = "Roar";
    private const string m_Hit = "GetHit";
    private const string m_Die = "Die";

    private const string m_StartJumpAttack = "StartJump";
    private const string m_EndJumpAttack = "EndJump";
    #endregion

    public bool CanMoving() => !m_DoCrawsAttacking && !m_DoGrabAttacking && !m_DoGrabAttackingReverse &&
         !m_DoJumpBiteAttacking && !m_DoSpiting && !m_DoHitting && !m_DoRoaring;
    //

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        legController = FindObjectOfType<LegController>();
        //임시용
    }

    public void Init()
    {
        m_Animator = GetComponent<Animator>();
        legController = FindObjectOfType<LegController>();
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

    public void SetIKEnableExceptNeck(bool _value)
    {
        for (int i = 0; i < rigAnimators.Length - 1; i++)
        {
            rigAnimators[i].enabled = _value;
            rigBuilders[i].enabled = _value;
        }
    }

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

    #region Normal Attack Set
    public Task SetClawsAttack()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        m_DoCrawsAttacking = true;

        SetTriggerAnimation(m_ClawsAttack);
        StartCoroutine(CheckForEndClawsAttack(tcs));

        return tcs.Task;
    }

    private IEnumerator CheckForEndClawsAttack(TaskCompletionSource<bool> tcs)
    {
        while (m_DoCrawsAttacking) yield return null;
        tcs.SetResult(true);
    }

    public void SetSpitVenom()
    {
        m_DoSpiting = true;
        SetTriggerAnimation(m_SpitVenom);
    }
    #endregion
    #region Special Attack Set

    public Task SetGrabAttack()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        
        m_DoGrabAttacking = true;
        m_DoGrabAttackingReverse = true;

        SetTriggerAnimation(m_GrabAttack);
        StartCoroutine(CheckForEndBiteAttack(tcs));

        return tcs.Task;
    }

    private IEnumerator CheckForEndBiteAttack(TaskCompletionSource<bool> tcs)
    {
        while (m_DoGrabAttackingReverse) yield return null;
        tcs.SetResult(true);
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

    #endregion
    #region ETC
    public void SetRoar()
    {
        m_DoRoaring = true;
        SetTriggerAnimation(m_Roar);
    }

    public void SetHit()
    {
        m_DoHitting = true;
        SetTriggerAnimation(m_Hit);
    }

    public void SetDie()
    {
        SetTriggerAnimation(m_Die);
    }
    #endregion

    #region Animation End Event
#pragma warning disable IDE0051 // 사용되지 않는 private 멤버 제거
    private void EndCrawsAttack() => m_DoCrawsAttacking = false;
    private void EndBiteAttack() => m_DoGrabAttacking = false;
    private void EndBiteAttackReverse() => m_DoGrabAttackingReverse = false;
    private void EndSpitVenom() => m_DoSpiting = false;     //n
    private void EndRoar() => m_DoRoaring = false;
    private void EndHit() => m_DoHitting = false;   //n
    

#pragma warning restore IDE0051
    #endregion
}
