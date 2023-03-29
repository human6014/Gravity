using Contoller.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test 
{
    public class MeleeWeapon : Weapon
    {
        [Space(15)]
        [Header("Child")]
        [Header("Attack Ratio")]
        [SerializeField] private float m_LightFireRatio = 1f;        //약공격 속도
        [SerializeField] private float m_HeavyFireRatio = 1.5f;         //강공격 속도
        //피격시 자국
        
        
        private bool isRunning;
        private float currentFireRatio;

        protected override void Awake()
        {
            base.Awake();
            AssignKeyAction(); //Awake자리 아님
        }

        private void AssignKeyAction()
        {
            m_PlayerInputController.SemiFire += TryLightAttack;
            m_PlayerInputController.HeavyFire += TryHeavyAttack;
        }

        private void Start()
        {
            m_ArmAnimator.runtimeAnimatorController = m_ArmOverrideController;

            m_EquipmentAnimator.SetTrigger("Equip");
            m_ArmAnimator.SetTrigger("Equip");

            m_CrossHairController.SetCrossHair(0);

            m_IsEquip = true;
        }

        public override void Init()
        {
            base.Init();

            m_CrossHairController.SetCrossHair(0);
            AssignKeyAction();
            m_IsEquip = true;
        }

        private void Update()
        {
            currentFireRatio += Time.deltaTime;

            //if (!firstPersonController.m_IsWalking)
            //{
            //    if (!isRunning)
            //    {
            //        isRunning = true;
            //        if (runningCoroutine != null) StopCoroutine(runningCoroutine);
            //        runningCoroutine = StartCoroutine(RunningPos());
            //    }
            //}
            //else if (isRunning)
            //{
            //    isRunning = false;
            //    if (runningCoroutine != null) StopCoroutine(runningCoroutine);
            //}
        }

        private void TryLightAttack()
        {
            if (currentFireRatio > m_LightFireRatio)
            {
                currentFireRatio = 0;

                Attack(0);
            }
        }

        private void TryHeavyAttack()
        {
            if (currentFireRatio > m_HeavyFireRatio)
            {
                currentFireRatio = 0;

                Attack(1);
            }
        }

        private void Attack(int swingIndex)
        {
            m_ArmAnimator.SetFloat("Swing Index", swingIndex);
            m_EquipmentAnimator.SetFloat("Swing Index", swingIndex);

            m_ArmAnimator.SetTrigger("Swing");
            m_EquipmentAnimator.SetTrigger("Swing");
        }

        public override void Dispose()
        {
            base.Dispose();
            DischargeKeyAction();
        }

        private void DischargeKeyAction()
        {
            m_PlayerInputController.SemiFire -= TryLightAttack;
            m_PlayerInputController.HeavyFire -= TryHeavyAttack;
        }
    }
}
