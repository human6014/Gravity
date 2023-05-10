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
        protected Transform m_CameraTransform;
        protected MeleeWeaponStatScriptable m_MeleeWeaponStat;

        private ObjectPoolManager.PoolingObject[] m_EffectPoolingObject;
        private SurfaceManager m_SurfaceManager;
        private AudioSource m_AudioSource;

        private AudioClip[] audioClips;
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
                damageable.Hit(m_MeleeWeaponStat.m_Damage, m_MeleeWeaponStat.m_BulletType);
                //m_PlayerData.HitEnemy();
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
            audioClips = m_SurfaceManager.GetSlashHitEffectSounds(hitEffectNumber - 3);
            audioClip = audioClips[Random.Range(0, audioClips.Length)];
        }

        public abstract bool SwingCast();
    }
}
