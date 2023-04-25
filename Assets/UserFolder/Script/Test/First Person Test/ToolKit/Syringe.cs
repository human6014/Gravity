using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Syringe : MonoBehaviour
{
    [SerializeField] private Animator m_ArmAnimator;
    [SerializeField] private AnimatorOverrideController m_ArmOverrideController;
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_HealSound;
    
    private Animator m_EquipmentAnimator;

    public bool IsUsing { get; private set; }

    private void Awake()
    {
        m_EquipmentAnimator = GetComponent<Animator>();
    }
    
    public async Task TryHeal()
    {
        IsUsing = true;
        gameObject.SetActive(true);
        m_ArmAnimator.runtimeAnimatorController = m_ArmOverrideController;
        m_ArmAnimator.SetTrigger("Use");
        m_EquipmentAnimator.SetTrigger("Use");

        await Task.Delay(800);
        m_AudioSource.PlayOneShot(m_HealSound);
        await Task.Delay(2000);

        IsUsing = false;
        gameObject.SetActive(false);
    }
}
