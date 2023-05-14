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
    private const string m_JumpAttack = "JumpBiteAttack";

    private const string m_Walk = "Walk";
    private const string m_Roar = "Roar";
    private const string m_Hit = "GetHit";
    private const string m_Die = "Die";
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

    public void SetIdle(bool isActiveIK)
    {
        m_Animator.SetBool(m_Walk, isActiveIK);
        SetIKEnable(isActiveIK);
    }

    private void SetTriggerAnimation(string name)
    {
        SetIdle(false);
        m_Animator.SetTrigger(name);
    }

    #region Animation Attack
    public void SetClawsAttack()
    {
        m_DoCrawsAttacking = true;
        SetTriggerAnimation(m_ClawsAttack);
    }

    public Task SetBiteAttack()
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

    public Task SetJumpBiteAttack()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

        m_DoJumpBiteAttacking = true;

        SetTriggerAnimation(m_JumpAttack);
        StartCoroutine(CheckForEndJumpBiteAttack(tcs));

        return tcs.Task;
    }

    private IEnumerator CheckForEndJumpBiteAttack(TaskCompletionSource<bool> tcs)
    {
        while (m_DoJumpBiteAttacking) yield return null;
        tcs.SetResult(true);
    }
    //위 4개 함수 통합

    public void SetSpitVenom()
    {
        m_DoSpiting = true;
        SetTriggerAnimation(m_SpitVenom);
    }

    #endregion
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

    #region Animation End Event
//#pragma warning disable IDE0051 // 사용되지 않는 private 멤버 제거
    private void EndCrawsAttack() => m_DoCrawsAttacking = false;
    private void EndBiteAttack() => m_DoGrabAttacking = false;
    private void EndBiteAttackReverse() => m_DoGrabAttackingReverse = false;
    private void EndJumpBiteAttack()
    {
        Debug.Log("EndJumpBiteAttack");
        m_DoJumpBiteAttacking = false;
    }
    private void EndSpitVenom() => m_DoSpiting = false;     //n
    private void EndRoar() => m_DoRoaring = false;
    private void EndHit() => m_DoHitting = false;   //n
    

//#pragma warning restore IDE0051
    #endregion
}
