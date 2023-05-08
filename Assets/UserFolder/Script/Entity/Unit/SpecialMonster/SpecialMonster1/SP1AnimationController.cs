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

    private bool m_DoBiteAttacking;
    private bool m_DoCrawsAttacking;
    private bool m_DoRoaring;
    private bool m_DoSpiting;
    private bool m_DoBiteAttackingReverse;
    public bool CanMoving() => !m_DoBiteAttacking && !m_DoCrawsAttacking && !m_DoRoaring && !m_DoSpiting && !m_DoBiteAttackingReverse;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
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


    public void SetIdle(bool isActiveIK)
    {
        m_Animator.SetBool("Walk", false);
        SetIKEnable(isActiveIK);
    }

    public void SetWalk()
    {
        m_Animator.SetBool("Walk", true);
        SetIKEnable(true);
    }

    public Task SetBiteAttack()
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        
        m_DoBiteAttacking = true;
        m_DoBiteAttackingReverse = true;
        SetIdle(false);
        m_Animator.SetTrigger("BiteAttack");

        StartCoroutine(CheckForEndBiteAttack(tcs));

        return tcs.Task;
    }

    private IEnumerator CheckForEndBiteAttack(TaskCompletionSource<bool> tcs)
    {
        while (m_DoBiteAttackingReverse) yield return null;
        tcs.SetResult(true);
    }

    public void SetClawsAttack()
    {
        m_DoCrawsAttacking = true;
        SetIdle(false);
        m_Animator.SetTrigger("ClawsAttack");
    }

    public void SetRoar()
    {
        m_DoRoaring = true;
        SetIdle(false);
        m_Animator.SetTrigger("Roar");
    }

    public void SetSpitVenom()
    {
        m_DoSpiting = true;
        SetIdle(false);
        m_Animator.SetTrigger("SpitVenom");
    }

    public void SetDie()
    {
        m_Animator.SetTrigger("Die");
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

    private void EndBiteAttackReverse()
    {
        m_DoBiteAttackingReverse = false;
        Debug.Log("EndBiteAttackReverse");
    }
    #endregion
}
