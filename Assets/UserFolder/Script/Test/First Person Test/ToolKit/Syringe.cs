using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Syringe : MonoBehaviour
{
    [SerializeField] private Animator m_ArmAnimator;
    [SerializeField] private AnimatorOverrideController m_ArmOverrideController;

    private AudioSource m_AudioSource;
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

        await Task.Delay(2800);

        IsUsing = false;
        gameObject.SetActive(false);
    }

    /*
    public void TryHeal()
    {
        if (IsUsing) return;
        IsUsing = true;
        m_ArmAnimator.runtimeAnimatorController = m_ArmOverrideController;
        m_ArmAnimator.SetTrigger("Use");
        m_EquipmentAnimator.SetTrigger("Use");
        StartCoroutine(Heal());
    }
    
    private IEnumerator Heal()
    {

        yield return new WaitForSeconds(2);
        IsUsing = false;
    }
    */
}
