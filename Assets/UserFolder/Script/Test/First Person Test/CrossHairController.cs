using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scriptable;

public enum BehaviorState
{
    Idle,
    Walk,
    Run,
    Crouch,
    Jump
}

public enum WeaponState
{
    Idle,
    Aim
}
public class CrossHairController : MonoBehaviour
{
    //[SerializeField] private AnimatorOverrideController[] m_AnimatorController;
    [SerializeField] private CrossHairScripatble[] crossHairInfo;

    private CrossHairScripatble m_CurrentCrossHairScripatble;
    private Animator m_Animator;
    private Image [] crossHairImage = new Image[5];
    private Color color;


    #region Test
    private const string m_RunState = "IsRunning";
    private const string m_WalkState = "IsWalking";
    private const string m_IdleState = "IsIdle";
    private const string m_CrouchState = "IsCrouching";
    private const string m_JumpState = "IsJumping";
    private const string m_AimState = "IsAiming";

    private string m_CurrentState;
    #endregion
    public CrossHairScripatble GetCrossHairInfo(int index) => crossHairInfo[index];

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        crossHairImage = GetComponentsInChildren<Image>();
    }


    /// <summary>
    /// 총기 종류에 따라 크로스 해어 설정
    /// </summary>
    /// <param name="index">0 : 없음, 1: 점, 2: 십자선, 3 : 원형</param>
    public void SetCrossHair(int index)
    {
        m_CurrentCrossHairScripatble = crossHairInfo[index];
        m_Animator.runtimeAnimatorController = m_CurrentCrossHairScripatble.m_AnimatorController;
        
        for (int i = 0; i < crossHairImage.Length; i++)
        {
            if (m_CurrentCrossHairScripatble.crossHairSprite[i] == null)
            {
                crossHairImage[i].enabled = false;
                continue;
            }
            crossHairImage[i].sprite = m_CurrentCrossHairScripatble.crossHairSprite[i];
            crossHairImage[i].enabled = true;
        }
    }

    //temp
    public void CrossHairSetTrigger(string state)
    {
        m_Animator.SetTrigger(state);   
    }

    public void CrossHairSetBool(string state, bool active)
    {
        m_CurrentState = state;
        if (state == m_WalkState && !active) m_CurrentState = m_IdleState;
        m_Animator.SetBool(state, active);
    }

    public float GetCurrentAccurancy()
    {
        float currentAccurancy = 0;
        switch (m_CurrentState)
        {
            case m_CrouchState:
                currentAccurancy = m_CurrentCrossHairScripatble.m_CrouchAccurancy;
                break;
            case m_JumpState:
                currentAccurancy = m_CurrentCrossHairScripatble.m_JumpAccuracy;
                break;
            case m_WalkState:
                currentAccurancy = m_CurrentCrossHairScripatble.m_WalkAccuracy;
                break;
            case m_IdleState:
                currentAccurancy = m_CurrentCrossHairScripatble.m_IdleAccuracy;
                break;
        }

        return currentAccurancy;
    }
}
