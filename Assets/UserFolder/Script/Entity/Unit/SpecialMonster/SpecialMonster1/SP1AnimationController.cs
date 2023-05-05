using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumType;
using UnityEngine.Animations.Rigging;

public class SP1AnimationController : MonoBehaviour
{
    [SerializeField] Animator []rigAnimators;
    [SerializeField] RigBuilder []rigBuilders;
    private LegController legController;
    private Animator animator;

    private bool m_DoBiteAttacking;
    private bool m_DoCrawsAttacking;
    private bool m_DoRoaring;
    private bool m_DoSpiting;

    public bool CanMoving() => !m_DoBiteAttacking && !m_DoCrawsAttacking && !m_DoRoaring && !m_DoSpiting;

    void Start()
    {
        animator = GetComponent<Animator>();
        legController = FindObjectOfType<LegController>();
        //SetIKEnable(false);
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

    /// <summary>
    /// None 상태 : IK만 동작함
    /// </summary>
    public void SetNone()
    {
        animator.SetBool("None", true);
        animator.SetBool("Walk", false);
        animator.SetBool("Idle", false);
        SetIKEnable(true);
    }

    public void SetIdle()
    {
        animator.SetBool("Idle", true);
        animator.SetBool("Walk", false);
        animator.SetBool("None", true);
        SetIKEnable(false);
    }

    public void SetWalk()
    {
        animator.SetBool("Walk", true);
        animator.SetBool("Idle", false);
        animator.SetBool("None", false);
        SetIKEnable(true);
    }

    public void SetBiteAttack()
    {
        m_DoBiteAttacking = true;
        SetIKEnable(false);
        animator.SetTrigger("BiteAttack");
    }

    public void SetClawsAttack()
    {
        m_DoCrawsAttacking = true;
        SetIKEnable(false);
        animator.SetTrigger("ClawsAttack");
    }

    public void SetRoar()
    {
        m_DoRoaring = true;
        SetIKEnable(false);
        animator.SetTrigger("Roar");
    }

    public void SetSpitVenom()
    {
        m_DoSpiting = true;
        SetIKEnable(false);
        animator.SetTrigger("SpitVenom");
    }


    #region Animation Event
    private void EndBiteAttack()
    {
        m_DoBiteAttacking = false;
        Debug.Log("EndBiteAttack");
    }

    private void EndCrawsAttack()
    {
        m_DoCrawsAttacking = false;
        Debug.Log("EndCrawsAttack");
    }

    private void EndRoar()
    {
        m_DoRoaring = false;
        Debug.Log("EndRoar");
    }

    private void EndSpitVenom()
    {
        m_DoSpiting = false;
        Debug.Log("EndSpitVenom");
    }
    #endregion
}
