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
        [SerializeField] private float m_SwingRadius;
        [SerializeField] private float m_MaxDistance;

        [SerializeField] private bool m_CanComboAttack;

        private Scriptable.MeleeWeaponSoundScripatble m_MeleeWeaponSound;
        private Scriptable.MeleeWeaponStatScriptable m_MeleeWeaponStat;

        private ObjectPoolManager.PoolingObject[] m_EffectPoolingObject;
        private SurfaceManager m_SurfaceManager;
        private Transform m_CameraTransform;
        private Coroutine m_RunningCoroutine;

        private Quaternion m_RunningPivotRotation;
        private float m_CurrentFireTime;
        private int m_SwingIndex;
        private bool m_IsLightAttacking;
        private bool m_IsHeavyAttacking;
        private bool m_IsAttacking;
        private bool m_IsRunning;
        
        public override bool CanChangeWeapon => base.CanChangeWeapon && !m_IsAttacking;
        private bool CanComboAttacking() => m_CanComboAttack && (m_IsLightAttacking && !m_IsHeavyAttacking);

        protected override void Awake()
        {
            base.Awake();

            m_MeleeWeaponSound = (Scriptable.MeleeWeaponSoundScripatble)base.m_WeaponSoundScriptable;
            m_MeleeWeaponStat = (Scriptable.MeleeWeaponStatScriptable)base.m_WeaponStatScriptable;

            m_SurfaceManager = FindObjectOfType<SurfaceManager>();
            m_CameraTransform = m_MainCamera.transform;

            m_RunningPivotRotation = Quaternion.Euler(m_MeleeWeaponStat.m_RunningPivotDirection);

            AssignPoolingObject();
        }

        private void AssignPoolingObject()
            => m_EffectPoolingObject = m_WeaponManager.m_EffectPoolingObjectArray;
        

        protected override void AssignKeyAction()
        {
            base.AssignKeyAction();
            m_PlayerInputController.SemiFire += TryLightAttack;
            m_PlayerInputController.HeavyFire += TryHeavyAttack;
        }

        private void Update()
        {
            m_CurrentFireTime += Time.deltaTime;
            if (m_PlayerData.m_PlayerState.PlayerBehaviorState == PlayerBehaviorState.Running)
            {
                if (!m_IsRunning)
                {
                    m_IsRunning = true;
                    if (m_RunningCoroutine != null) StopCoroutine(m_RunningCoroutine);
                    m_RunningCoroutine = StartCoroutine(PosChange(m_MeleeWeaponStat.m_RunningPivotPosition, m_RunningPivotRotation));
                }
            }
            else
            {
                if (m_IsRunning)
                {
                    m_IsRunning = false;
                    if (m_RunningCoroutine != null) StopCoroutine(m_RunningCoroutine);
                    m_RunningCoroutine = StartCoroutine(PosChange(m_WeaponManager.m_OriginalPivotPosition, m_WeaponManager.m_OriginalPivotRotation));
                }
                m_MainCamera.fieldOfView = Mathf.Lerp(m_MainCamera.fieldOfView, m_WeaponManager.m_OriginalFOV, m_MeleeWeaponStat.m_FOVMultiplier * Time.deltaTime);
            }
        }

        private IEnumerator PosChange(Vector3 EndPosition, Quaternion EndRotation)
        {
            float currentTime = 0;
            float elapsedTime;
            Vector3 startLocalPosition = m_Pivot.localPosition;
            Quaternion startLocalRotation = m_Pivot.localRotation;
            while (currentTime < m_MeleeWeaponStat.m_RunningPosTime)
            {
                currentTime += Time.deltaTime;

                elapsedTime = currentTime / m_MeleeWeaponStat.m_RunningPosTime;
                m_Pivot.localPosition = Vector3.Lerp(startLocalPosition, EndPosition, elapsedTime);
                m_Pivot.localRotation = Quaternion.Lerp(startLocalRotation, EndRotation, elapsedTime);

                yield return elapsedTime;
            }
        }

        private void TryLightAttack()
        {
            if (m_CurrentFireTime > m_MeleeWeaponStat.m_LightFireTime)
            {
                m_CurrentFireTime = 0;
                m_IsLightAttacking = true;
                Attack(0);
            }
        }

        private void TryHeavyAttack()
        {
            if ((m_CurrentFireTime > m_MeleeWeaponStat.m_HeavyFireTime) || CanComboAttacking())
            {
                m_CurrentFireTime = 0;
                m_IsHeavyAttacking = true;
                Attack(1);
            }
        }

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
            m_IsLightAttacking = false;
            m_IsHeavyAttacking = false;
            m_IsAttacking = false;
        }

        private void TestDamage()
        {
            if (Physics.SphereCast(m_CameraTransform.position, m_SwingRadius, m_CameraTransform.forward, out RaycastHit hit, m_MaxDistance, m_MeleeWeaponStat.m_AttackableLayer, QueryTriggerInteraction.Ignore))
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

        protected override void DischargeKeyAction()
        {
            base.DischargeKeyAction();
            m_PlayerInputController.SemiFire -= TryLightAttack;
            m_PlayerInputController.HeavyFire -= TryHeavyAttack;
        }
    }
}
