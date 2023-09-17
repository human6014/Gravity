using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using Scriptable.Equipment;

namespace Entity.Object.Weapon
{
    [RequireComponent(typeof(MeleeWeapon))]
    public abstract class Attackable : MonoBehaviour
    {
        [SerializeField] private Transform m_ForcePoint;
        [SerializeField] private int m_BloodEffectIndex;

        protected Transform m_CameraTransform;
        protected MeleeWeaponStatScriptable m_MeleeWeaponStat;

        private ObjectPoolManager.PoolingObject[] m_EffectPoolingObject;
        private SurfaceManager m_SurfaceManager;
        private AudioSource m_AudioSource;
        private AudioClip[] m_AudioClips;

        private int m_RealDamage;
        public void Setup(MeleeWeaponStatScriptable meleeWeaponStatScriptable, ObjectPoolManager.PoolingObject[] effectPoolingObject, Transform cameraTransform)
        {
            m_MeleeWeaponStat = meleeWeaponStatScriptable;
            m_EffectPoolingObject = effectPoolingObject;
            m_CameraTransform = cameraTransform;

            m_AudioSource = GetComponentInParent<AudioSource>();
            m_SurfaceManager = FindObjectOfType<SurfaceManager>();
        }

        protected bool ProcessEffect(ref RaycastHit hit, ref bool doEffect)
        {
            if (hit.transform.TryGetComponent(out IDamageable damageable))
            {
                m_SurfaceManager.InstanceBloodEffect(ref hit, m_BloodEffectIndex);
                Vector3 dir = (hit.point - m_ForcePoint.position).normalized * m_MeleeWeaponStat.m_AttackForce;
                damageable.Hit(m_RealDamage, m_MeleeWeaponStat.m_BulletType, dir);
                return true;
            }

            if (!doEffect)
            {
                int hitEffectNumber;
                int hitLayer = hit.transform.gameObject.layer;
                if (hitLayer == 14) hitEffectNumber = 0;
                else if (hitLayer == 17) hitEffectNumber = 1;
                else
                {
                    if (!hit.transform.TryGetComponent(out MeshRenderer meshRenderer)) return false;
                    if ((hitEffectNumber = m_SurfaceManager.IsInMaterial(meshRenderer.sharedMaterial)) == -1) return false;
                }
                hitEffectNumber += 3;
                EffectSet(out AudioClip audioClip, out DefaultPoolingScript effectObj, hitEffectNumber);

                m_AudioSource.PlayOneShot(audioClip);

                effectObj.Init(hit.point, Quaternion.LookRotation(hit.normal), m_EffectPoolingObject[hitEffectNumber]);
                effectObj.gameObject.SetActive(true);
                doEffect = true;
            }
            return false;
        }

        private void EffectSet(out AudioClip audioClip, out DefaultPoolingScript effectObj, int hitEffectNumber)
        {
            effectObj = (DefaultPoolingScript)m_EffectPoolingObject[hitEffectNumber].GetObject(false);
            m_AudioClips = m_SurfaceManager.GetSlashHitEffectSounds(hitEffectNumber - 3);
            audioClip = m_AudioClips[Random.Range(0, m_AudioClips.Length)];
        }

        public void SetDamageUpPercentage(float DamageUpPercentage)
        {
            m_RealDamage = (int)(m_MeleeWeaponStat.m_Damage * DamageUpPercentage);
        }

        public abstract bool SwingCast();
    }
}
