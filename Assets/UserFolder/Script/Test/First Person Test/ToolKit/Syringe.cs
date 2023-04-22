using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syringe : MonoBehaviour
{
    [SerializeField] protected int ItemIndex;
    [SerializeField] private Animator m_Animator;
    [SerializeField] private AnimatorOverrideController m_AnimatorOverrideController;

    [SerializeField] private PlayerInputController m_PlayerInputController;
    [SerializeField] private WeaponSway m_WeaponSway;
    [SerializeField] protected PlayerData m_PlayerData;
    private AudioSource m_AudioSource;
    public bool IsEquiping { get; private set; }
    public bool IsUnequiping { get; private set; }
    private void Awake()
    {
        
    }

    private void AssignKetAction()
    {

    }

    private void TryHeal()
    {

    }
}
