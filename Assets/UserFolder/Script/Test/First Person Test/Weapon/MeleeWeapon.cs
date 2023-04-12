using Contoller.Player;
using Entity.Object;
using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

        private ObjectPoolManager.PoolingObject[] m_EffectPoolingObject;

        [SerializeField] private Transform m_MainCamera;
        [SerializeField] private float m_SwingRadius;
        [SerializeField] private float m_MaxDistance;
        
        public bool m_IsAttacking { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            AssignKeyAction(); //AwakeÀÚ¸® ¾Æ´Ô
            AssignPoolingObject();
        }

        private void AssignPoolingObject()
        {
            m_EffectPoolingObject = m_WeaponManager.m_EffectPoolingObjectArray;
        }

        private void AssignKeyAction()
        {
            m_PlayerInputController.SemiFire += TryLightAttack;
            m_PlayerInputController.HeavyFire += TryHeavyAttack;
        }

        public override void Init()
        {
            base.Init();
            m_AudioSource.PlayOneShot(m_MeleeWeaponSound.equipSound[Random.Range(0, m_MeleeWeaponSound.equipSound.Length)]);
            m_CrossHairController.SetCrossHair((int)m_MeleeWeaponStat.m_DefaultCrossHair);
            AssignKeyAction();
            m_IsEquip = true;
        }

        private void Update()
        {
            currentFireRatio += Time.deltaTime;

            //if (m_IsAttacking) TestDamage();
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

        private int m_SwingIndex;
        private void Attack(int swingIndex)
        {
            //m_IsAttacking = true;
            m_SwingIndex = swingIndex;
            m_ArmAnimator.SetFloat("Swing Index", swingIndex);
            m_EquipmentAnimator.SetFloat("Swing Index", swingIndex);

            m_ArmAnimator.SetTrigger("Swing");
            m_EquipmentAnimator.SetTrigger("Swing");
        }

        //Animation Event
        private void StartAnimation()
        {
            m_IsAttacking = true;

            AudioClip[] playingAudio = m_SwingIndex == 1 ? m_MeleeWeaponSound.m_HeavyAttackSound : m_MeleeWeaponSound.m_LightAttackSound;

            m_AudioSource.PlayOneShot(playingAudio[Random.Range(0, playingAudio.Length)]);
            TestDamage();
        }

        //Animation Event
        private void EndAnimation()
        {
            m_IsAttacking = false;
        }

        //Vector3.Angle(mainCamera.forward, colls[i].transform.position - cachedTransform.position) 
        //    <= settings.FOVAngleVector3.Angle(cachedTransform.forward, colls[i].transform.position - cachedTransform.position) 
        //    <= settings.FOVAngle

        private void TestDamage()
        {
            if (Physics.SphereCast(m_MainCamera.position, m_SwingRadius, m_MainCamera.forward, out RaycastHit hit, m_MaxDistance, m_AttackableLayer, QueryTriggerInteraction.Ignore))
            {
                // Apply an impact impulse
                //if (hitInfo.rigidbody != null)
                //    hitInfo.rigidbody.AddForceAtPosition(itemUseRays.direction * swing.HitImpact, hitInfo.point, ForceMode.Impulse);

                int hitEffectNumber;
                int hitLayer = hit.transform.gameObject.layer;

                if (hitLayer == 14) hitEffectNumber = 0;
                else if (hitLayer == 17) hitEffectNumber = 1;
                else
                {
                    if (!hit.transform.TryGetComponent(out MeshRenderer meshRenderer)) return;
                    if ((hitEffectNumber = m_SurfaceManager.IsInMaterial(meshRenderer.sharedMaterial)) == -1) return;
                }
                EffectSet(out AudioClip audioClip, out DefaultPoolingScript effectObj, hitEffectNumber);

                m_AudioSource.PlayOneShot(audioClip);

                effectObj.Init(hit.point, Quaternion.LookRotation(hit.normal), m_EffectPoolingObject[hitEffectNumber]);
                effectObj.gameObject.SetActive(true);
            }
        }

        private void EffectSet(out AudioClip audioClip, out DefaultPoolingScript effectObj, int hitEffectNumber)
        {
            AudioClip[] audioClips;

            effectObj = (DefaultPoolingScript)m_EffectPoolingObject[hitEffectNumber].GetObject(false);
            audioClips = m_SurfaceManager.GetSlashHitEffectSounds(hitEffectNumber);
            audioClip = audioClips[Random.Range(0, audioClips.Length)];
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(m_MainCamera.position, m_SwingRadius);
            Gizmos.DrawWireSphere(m_MainCamera.position + m_MainCamera.forward * 0.3f + m_MainCamera.forward * m_MaxDistance, m_SwingRadius);
        }

        public override void Dispose()
        {
            m_AudioSource.PlayOneShot(m_MeleeWeaponSound.unequipSound[Random.Range(0, m_MeleeWeaponSound.unequipSound.Length)]);
            DischargeKeyAction();
            base.Dispose();
        }

        private void DischargeKeyAction()
        {
            m_PlayerInputController.SemiFire -= TryLightAttack;
            m_PlayerInputController.HeavyFire -= TryHeavyAttack;
        }
    }
}
