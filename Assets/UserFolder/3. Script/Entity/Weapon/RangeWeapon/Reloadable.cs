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
        [Tooltip("ź������ �ִ���")]
        [SerializeField] private bool m_HasMagazine;

        [Header("Magazine")]
        [Tooltip("ź���� ���� ��ġ")]
        [SerializeField] private Transform m_MagazineSpawnPos;

        [Tooltip("ź���� ������Ʈ")]
        [SerializeField] private PoolableScript m_MagazineObject;

        [Header("Pooling")]
        [Tooltip("Ǯ�� ������Ʈ ���̶�Ű ��ġ")]
        [SerializeField] private Transform m_ActiveObjectPool;

        [Tooltip("�̸� ������ ����")]
        [SerializeField] [Range(0, 30)] private int m_PoolingCount;

        private ObjectPoolManager.PoolingObject m_MagazinePoolingObject;
        private AudioSource m_AudioSource;

        protected PlayerData m_PlayerData;
        protected RangeWeaponSoundScriptable m_RangeWeaponSound;
        protected Animator m_ArmAnimator;
        protected Animator m_EquipmentAnimator;

        private float m_ReloadSpeedUpPercentage;

        public bool IsReloading { get; protected set; }
        public bool IsNonEmptyReloading { get; protected set; }
        public bool IsEmptyReloading { get; protected set; }
        public Interactabe HowInteratable { get; protected set; }


        protected virtual void Awake() => m_AudioSource = GetComponentInParent<AudioSource>();

        public void Setup(RangeWeaponSoundScriptable m_RangeWeaponSound, Animator m_ArmAnimator)
        {
            this.m_RangeWeaponSound = m_RangeWeaponSound;
            this.m_ArmAnimator = m_ArmAnimator;
            m_EquipmentAnimator = GetComponent<Animator>();
            m_PlayerData = FindObjectOfType<PlayerData>();

            if (!m_HasMagazine) return;
            m_MagazinePoolingObject = ObjectPoolManager.Register(m_MagazineObject, m_ActiveObjectPool);
            m_MagazinePoolingObject.GenerateObj(m_PoolingCount);
        }

        public void SetReloadSpeedPercentage(float ReloadSpeedUpPercentage)
        {
            m_ReloadSpeedUpPercentage = ReloadSpeedUpPercentage;

            m_EquipmentAnimator.SetFloat("Reload Speed", ReloadSpeedUpPercentage);
            m_EquipmentAnimator.SetFloat("Empty Reload Speed", ReloadSpeedUpPercentage);

            m_ArmAnimator.SetFloat("Reload Speed", ReloadSpeedUpPercentage);
            m_ArmAnimator.SetFloat("Empty Reload Speed", ReloadSpeedUpPercentage);
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
                    delayTime = Mathf.Max(delayTime - (delayTime * (m_ReloadSpeedUpPercentage - 1)), 0.05f);
                    yield return new WaitForSeconds(delayTime);

                    m_AudioSource.PlayOneShot(reloadSoundClip[i].audioClip);
                }
                m_PlayerData.RangeWeaponCountingReload();
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
                    delayTime = Mathf.Max(delayTime - (delayTime * (m_ReloadSpeedUpPercentage - 1)), 0.05f);
                    yield return new WaitForSeconds(delayTime);

                    m_AudioSource.PlayOneShot(reloadSoundClip[i].audioClip);
                }
            }
            
            yield return new WaitForSeconds(lastDelay);
        }

        public virtual void StopReload() { }

        public abstract bool CanFire();

        /*
         * �⺻ 1
         * 10% ����
         * �� 110��
         * 
         * 0.5��
         * 0.45��
         */
    }
}
