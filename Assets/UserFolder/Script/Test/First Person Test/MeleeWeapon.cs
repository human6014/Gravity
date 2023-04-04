using Contoller.Player;
using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test 
{
    public class MeleeWeapon : Weapon
    {
        [Space(15)]
        [Header("Child")]
        [SerializeField] private Scriptable.MeleeWeaponSoundScripatble m_MeleeWeaponSound;
        [SerializeField] private Scriptable.MeleeWeaponStatScriptable m_MeleeWeaponStat;
        
        private bool isRunning;
        private float currentFireRatio;

        private ObjectPoolManager.PoolingObject m_EffectPoolingObject;

        protected override void Awake()
        {
            base.Awake();
            AssignKeyAction(); //AwakeÀÚ¸® ¾Æ´Ô
            AssignPoolingObject();
        }

        private void AssignPoolingObject()
        {
            m_EffectPoolingObject = m_WeaponManager.GetEffectPoolingObject(0);
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

            m_CrossHairController.SetCrossHair((int)m_MeleeWeaponStat.m_DefaultCrossHair);
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
            if (currentFireRatio > m_MeleeWeaponStat.m_LightFireTime)
            {
                currentFireRatio = 0;

                Attack(0);
            }
        }

        private void TryHeavyAttack()
        {
            if (currentFireRatio > m_MeleeWeaponStat.m_HeavyFireTime)
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

            if (Physics.CapsuleCast(transform.position + transform.up, transform.position - transform.up,3, Vector3.zero,5, m_MeleeWeaponStat.m_AttackableLayer))
            {

            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            
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
