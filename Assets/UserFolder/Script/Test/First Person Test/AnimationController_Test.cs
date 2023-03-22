using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController_Test : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private AnimatorOverrideController equipmentOverrideController = null;
    [SerializeField] private AnimatorOverrideController armOverrideController = null;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        

        
    }

    private void Start()
    {
        animator.runtimeAnimatorController = armOverrideController;
        
        animator.SetBool("Arms Are Visible", true);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift))
            animator.SetBool("Running", true);
        else
        {
        }
    }
}
