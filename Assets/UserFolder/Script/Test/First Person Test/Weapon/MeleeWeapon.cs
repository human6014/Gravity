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
        private Scriptable.MeleeWeaponSoundScripatble m_MeleeWeaponSound;
        private Scriptable.MeleeWeaponStatScriptable m_MeleeWeaponStat;
 
        private bool isRunning;
        private float currentFireRatio;

        private ObjectPoolManager.PoolingObject[] m_EffectPoolingObject;
        private SurfaceManager m_SurfaceManager;

        [SerializeField] private Transform m_MainCamera;
        [SerializeField] private float m_SwingRadius;
        [SerializeField] private float m_MaxDistance;
        
        public bool m_IsAttacking { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            m_MeleeWeaponSound = (Scriptable.MeleeWeaponSoundScripatble)base.m_WeaponSoundScriptable;
            m_MeleeWeaponStat = (Scriptable.MeleeWeaponStatScriptable)base.m_WeaponStatScriptable;
            m_SurfaceManager = FindObjectOfType<SurfaceManager>();

            AssignPoolingObject();
        }

        private void AssignPoolingObject()
        {
            m_EffectPoolingObject = m_WeaponManager.m_EffectPoolingObjectArray;
        }

        protected override void AssignKeyAction()
        {
            m_PlayerInputController.SemiFire += TryLightAttack;
            m_PlayerInputController.HeavyFire += TryHeavyAttack;
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

        private void TestDamage()
        {
            if (Physics.SphereCast(m_MainCamera.position, m_SwingRadius, m_MainCamera.forward, out RaycastHit hit, m_MaxDistance, m_MeleeWeaponStat.m_AttackableLayer, QueryTriggerInteraction.Ignore))
            {
                // Apply an impact impulse
                //if (hitInfo.rigidbody != null)
                //    hitInfo.rigidbody.AddForceAtPosition(itemUseRays.direction * swing.HitImpact, hitInfo.point, ForceMode.Impulse);

                if (hit.transform.TryGetComponent(out IDamageable damageable))
                {
                    damageable.Hit(m_MeleeWeaponStat.m_Damage);
                    return;
                }

                int hitEffectNumber;
                int hitLayer = hit.transform.gameObject.layer;
                if (hitLayer == 14) hitEffectNumber = 0;
                else if (hitLayer == 17) hitEffectNumber = 1;
                else
                {
                    if (!hit.transform.TryGetComponent(out MeshRenderer meshRenderer)) return;
                    if ((hitEffectNumber = m_SurfaceManager.IsInMaterial(meshRenderer.sharedMaterial)) == -1) return;
                }
                hitEffectNumber += 3;
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
            audioClips = m_SurfaceManager.GetSlashHitEffectSounds(hitEffectNumber - 3);
            audioClip = audioClips[Random.Range(0, audioClips.Length)];
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(m_MainCamera.position, m_SwingRadius);
            Gizmos.DrawWireSphere(m_MainCamera.position + m_MainCamera.forward * 0.3f + m_MainCamera.forward * m_MaxDistance, m_SwingRadius);
        }

        protected override void DischargeKeyAction()
        {
            m_PlayerInputController.SemiFire -= TryLightAttack;
            m_PlayerInputController.HeavyFire -= TryHeavyAttack;
        }
    }
}
