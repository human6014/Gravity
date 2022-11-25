using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumType;
using UnityEngine.Animations.Rigging;

public class AnimationController : MonoBehaviour
{
    private LegController legController;
    private Animator animator;
    AnimationStates animationState = AnimationStates.Idle;
    [SerializeField] Animator []rigAnimators;
    [SerializeField] RigBuilder []rigBuilders;
    void Start()
    {
        animator = GetComponent<Animator>();
        legController = FindObjectOfType<LegController>();
        //SetIKEnable(false);
    }

    private void FixedUpdate()
    {
        if(animationState == AnimationStates.None)
        {

        }
        else
        {
            
        }
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
        animationState = AnimationStates.None;
        //animator.SetBool("None", true);
        //animator.SetBool("Walk", false);
        //animator.SetBool("Idle", false);
        SetIKEnable(true);
    }

    public void SetIdle()
    {
        animationState = AnimationStates.Idle;
        animator.SetBool("Idle", true);
        animator.SetBool("Walk", false);
        animator.SetBool("None", false);
        SetIKEnable(false);
    }

    public void SetWalk()
    {
        animationState = AnimationStates.WalkForward;
        animator.SetBool("Walk", true);
        animator.SetBool("Idle", false);
        animator.SetBool("None", false);
        SetIKEnable(true);
    }
    public IEnumerator SetAnimationEnd(string animName, bool _value, float _time)
    {
        yield return new WaitForSeconds(_time);
        animator.SetBool(animName, _value);
    }
    public void SetBiteAttack()
    {

    }

    public void SetClawsAttack()
    {

    }

    public void SetRoar(bool _value)
    {
        SetIdle();
        animator.SetBool("Roar", _value);
        StartCoroutine(SetAnimationEnd("Roar", false, 0.8f));
    }

    public void SetSplitVenom()
    {
        animator.SetBool("SplitVenom", true);
    }
}
