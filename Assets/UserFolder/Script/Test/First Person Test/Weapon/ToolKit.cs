using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolKit : MonoBehaviour
{
    [Header("Parent")]
    [Header("Index")]
    [SerializeField] protected int ItemIndex;

    [Header("Animation")]
    [SerializeField] protected Animator m_ArmAnimator; //팔 애니메이터
    [SerializeField] protected AnimatorOverrideController m_ArmOverrideController = null;

    [Header("Pos Change")]
    [SerializeField] private Transform m_Pivot;

    [Header("Player Data")]
    [SerializeField] protected PlayerData m_PlayerData;

    protected Animator m_Animator { get; private set; }
    protected AudioSource m_AudioSource { get; private set; }

    protected PlayerInputController m_PlayerInputController { get; private set; }

    protected virtual void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_AudioSource = transform.parent.GetComponent<AudioSource>();


    }



}
