using Contoller.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test 
{
    public class MeleeWeapon : Weapon
    {
        [SerializeField] private float m_LightFireRatio = 1f;        //약공격 속도
        [SerializeField] private float m_HeavyFireRatio = 1.5f;         //강공격 속도
        //피격시 자국
        private Manager.SurfaceManager surfaceManager;

        private bool isEquip;
        private bool isRunning;
        private float currentFireRatio;

        private void Awake()
        {
            m_EquipmentAnimator = GetComponent<Animator>();
            surfaceManager = FindObjectOfType<Manager.SurfaceManager>();
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

            isEquip = true;
        }

        public override void Init()
        {
            base.Init();

            m_CrossHairController.SetCrossHair(0);

            isEquip = true;
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

                m_ArmAnimator.SetFloat("Swing Index",0);
                m_EquipmentAnimator.SetFloat("Swing Index", 0);

                m_ArmAnimator.SetTrigger("Swing");
                m_EquipmentAnimator.SetTrigger("Swing");
                Attack();
            }
        }

        private void TryHeavyAttack()
        {
            if (currentFireRatio > m_HeavyFireRatio)
            {
                currentFireRatio = 0;

                m_ArmAnimator.SetFloat("Swing Index", 1);
                m_EquipmentAnimator.SetFloat("Swing Index", 1);

                m_ArmAnimator.SetTrigger("Swing");
                m_EquipmentAnimator.SetTrigger("Swing");
                Attack();
            }
        }

        private void Attack()
        {

        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
