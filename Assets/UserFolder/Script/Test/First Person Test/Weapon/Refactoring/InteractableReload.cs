using Entity.Object;
using Manager;
using Scriptable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Test.RangeWeapon))]
public class InteractableReload : MonoBehaviour, IReloadable
{
    [Header("Reload magazine")]
    [Tooltip("탄알집 생성 위치")]
    [SerializeField] private Transform m_MagazineSpawnPos;
    [SerializeField] private bool m_IsRepeatReloadMotion;

    private ObjectPoolManager.PoolingObject m_MagazinePoolingObject;
    private RangeWeaponSoundScriptable m_RangeWeaponSound;
    private AudioSource m_AudioSource;
    private Coroutine m_ReloadCoroutine;

    private Animator m_ArmAnimator;
    private Animator m_EquipmentAnimator;

    private bool m_HasMagazine;
    private bool m_IsNonEmptyReloading;
    private bool m_IsEmptyReloading;
    public bool GetIsNonEmptyReloading() => m_IsNonEmptyReloading;

    private void Awake()
    {
        m_AudioSource = GetComponentInParent<AudioSource>();
    }

    public void Setup(RangeWeaponSoundScriptable m_RangeWeaponSound, Animator m_ArmAnimator)
    {
        this.m_RangeWeaponSound = m_RangeWeaponSound;
        this.m_ArmAnimator = m_ArmAnimator;
        m_EquipmentAnimator = GetComponent<Animator>();
    }

    public void SetupMagazinePooling(ObjectPoolManager.PoolingObject m_MagazinePoolingObject)
    {
        this.m_MagazinePoolingObject = m_MagazinePoolingObject;
        m_HasMagazine = m_MagazinePoolingObject != null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(EmptyReload(m_RangeWeaponSound.reloadSoundClips));
        }
    }

    public void DoReload(bool m_IsEmpty)
    {
        WeaponSoundScriptable.DelaySoundClip[] soundClips;
        if (m_IsEmpty) soundClips = m_RangeWeaponSound.emptyReloadSoundClips;
        else soundClips = m_RangeWeaponSound.reloadSoundClips;

        m_ReloadCoroutine = StartCoroutine(NonEmptyReload(m_RangeWeaponSound.reloadSoundClips));
        
        //m_ReloadCoroutine = StartCoroutine(EmptyReload(soundClips));
    }

    #region R870 ver
    private IEnumerator NonEmptyReload(WeaponSoundScriptable.DelaySoundClip[] ReloadSoundClip)
    {
        m_IsNonEmptyReloading = true;
        if (!m_IsRepeatReloadMotion)
        {
            m_ArmAnimator.SetTrigger("Empty Reload");
            m_EquipmentAnimator.SetTrigger("Empty Reload");

            yield return new WaitForSeconds(0.8f);
        }
        float delayTime = 0;
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < ReloadSoundClip.Length; i++)
            {
                delayTime = ReloadSoundClip[i].delayTime;
                yield return new WaitForSeconds(delayTime);
                m_AudioSource.PlayOneShot(ReloadSoundClip[i].audioClip);
            }
            m_ArmAnimator.SetTrigger("Reload");
            m_EquipmentAnimator.SetTrigger("Reload");
        }

        yield return new WaitForSeconds(delayTime);
        m_ArmAnimator.SetTrigger("End Reload");
        m_EquipmentAnimator.SetTrigger("End Reload");

        m_IsNonEmptyReloading = false;
    }

    private IEnumerator EmptyReload(WeaponSoundScriptable.DelaySoundClip[] ReloadSoundClip)
    {
        m_IsEmptyReloading = true;
        m_ArmAnimator.SetTrigger("Empty Reload");
        m_EquipmentAnimator.SetTrigger("Empty Reload");

        float delayTime = 0;
        if (!m_IsRepeatReloadMotion)
        {
            for(int i=0;i< m_RangeWeaponSound.emptyReloadSoundClips.Length; i++)
            {
                delayTime = m_RangeWeaponSound.emptyReloadSoundClips[i].delayTime;
                yield return new WaitForSeconds(delayTime);
                m_AudioSource.PlayOneShot(m_RangeWeaponSound.emptyReloadSoundClips[i].audioClip);
            }

            InstanceMagazine();
            m_IsEmptyReloading = false;
            yield break;
        }
        yield return new WaitForSeconds(1.6f);
        m_ArmAnimator.SetTrigger("Start Reload");
        m_EquipmentAnimator.SetTrigger("Start Reload");

        yield return new WaitForSeconds(0.3f);
        

        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < ReloadSoundClip.Length; i++)
            {
                delayTime = ReloadSoundClip[i].delayTime;
                yield return new WaitForSeconds(delayTime);
                m_AudioSource.PlayOneShot(ReloadSoundClip[i].audioClip);
            }
            m_ArmAnimator.SetTrigger("Reload");
            m_EquipmentAnimator.SetTrigger("Reload");
        }
        yield return new WaitForSeconds(delayTime);

        m_ArmAnimator.SetTrigger("End Reload");
        m_EquipmentAnimator.SetTrigger("End Reload");
        m_IsEmptyReloading = false;
    }
    #endregion

    public void StopReload()
    {
        if(m_IsNonEmptyReloading) StopCoroutine(m_ReloadCoroutine);
    }

    private void InstanceMagazine()
    {
        if (!m_HasMagazine) return;
        Quaternion magazineSpawnRotation = Quaternion.Euler(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));
        DefaultPoolingScript magazinePoolingObject = (DefaultPoolingScript)m_MagazinePoolingObject.GetObject(false);
        magazinePoolingObject.Init(m_MagazineSpawnPos.position, magazineSpawnRotation, m_MagazinePoolingObject);
        magazinePoolingObject.gameObject.SetActive(true);
    }
}
