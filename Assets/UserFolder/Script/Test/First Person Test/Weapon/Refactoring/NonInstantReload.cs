using Entity.Object;
using Manager;
using Scriptable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Test.RangeWeapon))]
public class NonInstantReload : MonoBehaviour, IReloadable
{
    [Header("Reload magazine")]
    [Tooltip("탄알집 생성 위치")]
    [SerializeField] private Transform m_MagazineSpawnPos;

    private ObjectPoolManager.PoolingObject m_MagazinePoolingObject;
    private RangeWeaponSoundScriptable m_RangeWeaponSound;
    private AudioSource m_AudioSource;

    private bool m_HasMagazine;
    private bool m_IsReloading;

    public bool GetIsReloading() => m_IsReloading;

    private void Awake()
    {
        m_AudioSource = GetComponentInParent<AudioSource>();
    }

    public void Setup(RangeWeaponSoundScriptable m_RangeWeaponSound)
    {
        this.m_RangeWeaponSound = m_RangeWeaponSound;
    }

    public void SetupMagazinePooling(ObjectPoolManager.PoolingObject m_MagazinePoolingObject)
    {
        this.m_MagazinePoolingObject = m_MagazinePoolingObject;
        m_HasMagazine = m_MagazinePoolingObject != null;
    }

    public void DoReload(bool m_IsEmpty)
    {
        m_IsReloading = true;

        WeaponSoundScriptable.DelaySoundClip[] soundClips;
        if (m_IsEmpty) soundClips = m_RangeWeaponSound.emptyReloadSoundClips;
        else soundClips = m_RangeWeaponSound.reloadSoundClips;

        StartCoroutine(Reload(soundClips));
    }

    private IEnumerator Reload(RangeWeaponSoundScriptable.DelaySoundClip[] ReloadSoundClip)
    {
        int magazineSpawnTiming = (int)(ReloadSoundClip.Length * 0.5f);
        float delayTime = 0;

        for (int i = 0; i < ReloadSoundClip.Length; i++)
        {
            delayTime = ReloadSoundClip[i].delayTime;
            yield return new WaitForSeconds(delayTime);
            m_AudioSource.PlayOneShot(ReloadSoundClip[i].audioClip);
            if (i == magazineSpawnTiming) InstanceMagazine();
        }
        yield return new WaitForSeconds(delayTime);
        m_IsReloading = false;
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
