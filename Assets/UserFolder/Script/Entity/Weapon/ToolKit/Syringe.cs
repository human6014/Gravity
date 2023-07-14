using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Entity.Object.Util
{
    public class Syringe : MonoBehaviour
    {
        [SerializeField] private Animator m_ArmAnimator;
        [SerializeField] private AnimatorOverrideController m_ArmOverrideController;
        [SerializeField] private AudioSource m_AudioSource;
        [SerializeField] private AudioClip m_HealSound;
        [SerializeField] private ArmController m_ArmController;

        [SerializeField] private float m_SoundPlayTime = 0.8f;
        [SerializeField] private float m_HealEndTime = 2;

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
            m_ArmController.AppearArms(true);
            m_ArmAnimator.runtimeAnimatorController = m_ArmOverrideController;
            m_ArmAnimator.SetTrigger("Use");
            m_EquipmentAnimator.SetTrigger("Use");

            await WaitTime(m_SoundPlayTime);
            m_AudioSource.PlayOneShot(m_HealSound);
            await WaitTime(m_HealEndTime);

            gameObject.SetActive(false);
            IsUsing = false;
        }

        public async Task WaitTime(float waitTimeSeconds)
        {
            float elapsedTime = 0;
            while ((elapsedTime += Time.deltaTime) <= waitTimeSeconds)
                await Task.Yield();
        }
    }
}
