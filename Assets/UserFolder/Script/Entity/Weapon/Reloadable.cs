using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Manager;
using Scriptable.Equipment;

public enum Interactabe
{
    Non,
    Semi,
    Full
}
namespace Entity.Object.Weapon
{
    [RequireComponent(typeof(RangeWeapon))]
    public abstract class Reloadable : MonoBehaviour
    {
        [Tooltip("탄알집이 있는지")]
        [SerializeField] private bool m_HasMagazine;

        [Header("Magazine")]
        [Tooltip("탄알집 생성 위치")]
        [SerializeField] private Transform m_MagazineSpawnPos;

        [Tooltip("탄알집 오브젝트")]
        [SerializeField] private PoolableScript m_MagazineObject;

        [Header("Pooling")]
        [Tooltip("풀링 오브젝트 하이라키 위치")]
        [SerializeField] private Transform m_ActiveObjectPool;

        [Tooltip("미리 생성할 개수")]
        [SerializeField] [Range(0, 30)] private int m_PoolingCount;

        private ObjectPoolManager.PoolingObject m_MagazinePoolingObject;
        private AudioSource m_AudioSource;

        protected RangeWeaponSoundScriptable m_RangeWeaponSound;
        protected Animator m_ArmAnimator;
        protected Animator m_EquipmentAnimator;

        public bool m_IsReloading { get; protected set; }
        public bool m_IsNonEmptyReloading { get; protected set; }
        public bool m_IsEmptyReloading { get; protected set; }

        public Interactabe m_HowInteratable { get; protected set; }

        [SerializeField] private float m_TestAcceleration = 0;
        protected virtual void Awake() => m_AudioSource = GetComponentInParent<AudioSource>();

        public void Setup(RangeWeaponSoundScriptable m_RangeWeaponSound, Animator m_ArmAnimator)
        {
            this.m_RangeWeaponSound = m_RangeWeaponSound;
            this.m_ArmAnimator = m_ArmAnimator;
            m_EquipmentAnimator = GetComponent<Animator>();

            if (!m_HasMagazine) return;
            m_MagazinePoolingObject = ObjectPoolManager.Register(m_MagazineObject, m_ActiveObjectPool);
            m_MagazinePoolingObject.GenerateObj(m_PoolingCount);

            //this.m_EquipmentAnimator.speed += m_TestAcceleration / 100;
            //this.m_ArmAnimator.speed += m_TestAcceleration / 100;
        }

        public abstract void DoReload(bool m_IsEmpty, int difference);

        protected void InstanceMagazine()
        {
            if (!m_HasMagazine) return;
            Quaternion magazineSpawnRotation = Quaternion.Euler(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));
            DefaultPoolingScript magazinePoolingObject = (DefaultPoolingScript)m_MagazinePoolingObject.GetObject(false);
            magazinePoolingObject.Init(m_MagazineSpawnPos.position, magazineSpawnRotation, m_MagazinePoolingObject);
            magazinePoolingObject.gameObject.SetActive(true);
        }

        protected IEnumerator DelaySoundWithAnimation(WeaponSoundScriptable.DelaySoundClip[] reloadSoundClip, int playCount, float lastDelay = 0)
        {
            float delayTime;

            for (int j = 0; j < playCount; j++)
            {
                m_ArmAnimator.SetTrigger("Reload");
                m_EquipmentAnimator.SetTrigger("Reload");

                for (int i = 0; i < reloadSoundClip.Length; i++)
                {
                    delayTime = reloadSoundClip[i].delayTime;
                    //Debug.Log("기존 시간 : \t" + delayTime);
                    //delayTime -= delayTime * (m_TestAcceleration / 100);
                    //Debug.Log("가속된 시간 : \t" + delayTime);
                    yield return new WaitForSeconds(delayTime);

                    m_AudioSource.PlayOneShot(reloadSoundClip[i].audioClip);
                }
            }

            yield return new WaitForSeconds(lastDelay);

            m_ArmAnimator.SetTrigger("End Reload");
            m_EquipmentAnimator.SetTrigger("End Reload");
        }

        protected IEnumerator DelaySound(WeaponSoundScriptable.DelaySoundClip[] reloadSoundClip, int playCount, float lastDelay = 0)
        {
            float delayTime;

            for (int j = 0; j < playCount; j++)
            {
                for (int i = 0; i < reloadSoundClip.Length; i++)
                {
                    delayTime = reloadSoundClip[i].delayTime;
                    //Debug.Log("기존 시간 : \t" + delayTime);
                    //delayTime -= delayTime * (m_TestAcceleration / 100);
                    //Debug.Log("가속된 시간 : \t" + delayTime);
                    yield return new WaitForSeconds(delayTime);
                    m_AudioSource.PlayOneShot(reloadSoundClip[i].audioClip);
                }
            }
            yield return new WaitForSeconds(lastDelay);
        }

        public abstract void StopReload();

        public abstract bool CanFire();

        /*
         * 기본 1
         * 10% 가속
         * 총 110퍼
         * 
         * 0.5초
         * 0.45초
         */
    }
}
