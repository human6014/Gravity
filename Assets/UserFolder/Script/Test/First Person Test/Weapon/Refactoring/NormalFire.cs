using Scriptable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Object;
using Contoller.Player;
using Manager;

[RequireComponent(typeof(Test.RangeWeapon))]
[System.Serializable]
public class NormalFire : MonoBehaviour, IFireable
{ 
    [SerializeField] private Transform m_CasingSpawnPos;
    private Transform m_MuzzlePos;
    private Camera mainCamera;

    private int m_CasingIndex = -1;

    private RangeWeaponSoundScriptable m_RangeWeaponSound;
    private RangeWeaponStatScriptable m_RangeWeaponStat;

    private FirstPersonController m_FirstPersonController;
    private AudioSource m_AudioSource;
    private SurfaceManager m_SurfaceManager;

    private ObjectPoolManager.PoolingObject[] m_BulletEffectPoolingObjects; //0 : Concrete, 1 : Metal, 2 : Wood
    private ObjectPoolManager.PoolingObject m_CasingPoolingObject;

    private float m_CurrentFireTime;

    private WaitForSeconds m_BurstFireTime;

    [SerializeField] private TestFireLight m_FireLight;         //ÃÑ±¸ È­¿° Á¦¾î ½ºÅ©¸³Æ®

    [System.Flags]
    private enum FireMode
    {
        Auto = 1,
        Semi = 2,
        Burst = 4,
    }

    private FireMode m_CurrentFireMode = FireMode.Auto;
    private int m_FireModeLength;
    private int m_FireModeIndex = 1;
    public void Setup()
    {

    }

    public void DoFire()
    {
        Debug.Log("DoFire");
    }

    

    private void TryAutoFire()
    {
        if (m_CurrentFireMode != FireMode.Auto) return;
        Fire();
    }

    private void TrySemiFire()
    {
        if (m_CurrentFireMode == FireMode.Auto) return;

        if (m_CurrentFireMode == FireMode.Semi) Fire();
        else if (m_CurrentFireMode == FireMode.Burst) StartCoroutine(BurstFire());

    }
    private IEnumerator BurstFire()
    {
        for (int i = 0; i < 3; i++)
        {
            Fire();
            yield return m_BurstFireTime;
        }
    }

    private void Fire()
    {
        m_CurrentFireTime = 0;

        //m_EquipmentAnimator.SetBool("Fire", true);
        //m_ArmAnimator.SetBool("Fire", true);

        FireRay();
        FireRecoil();
        m_FireLight.Play(false);
        InstanceBullet();
        Invoke(nameof(EndFire), 0.1f);
    }

    private void InstanceBullet()
    {
        if (m_CasingIndex == -1) return;
        Quaternion cassingSpawnRotation = Quaternion.Euler(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));

        DefaultPoolingScript casingPoolingObject = (DefaultPoolingScript)m_CasingPoolingObject.GetObject(false);

        //GameObject cassing = Instantiate(m_CasingObj, m_CasingSpawnPos.position, cassingSpawnRotation);
        casingPoolingObject.Init(m_CasingSpawnPos.position, cassingSpawnRotation, m_CasingPoolingObject);
        casingPoolingObject.gameObject.SetActive(true);
        Rigidbody cassingRB = casingPoolingObject.GetComponent<Rigidbody>();

        float spinValue = m_RangeWeaponStat.m_SpinValue;
        Vector3 randomForce = new Vector3(Random.Range(0.75f, 1.25f), Random.Range(0.75f, 1.25f), Random.Range(0.75f, 1.25f));
        Vector3 randomTorque = new Vector3(Random.Range(-spinValue, spinValue), Random.Range(-spinValue, spinValue), Random.Range(-spinValue, spinValue));

        cassingRB.velocity = m_CasingSpawnPos.right + randomForce * 0.5f;
        cassingRB.angularVelocity = randomTorque;
    }

    private void FireRay()
    {
        AudioClip audioClip = m_RangeWeaponSound.fireSound[Random.Range(0, m_RangeWeaponSound.fireSound.Length)];
        m_AudioSource.PlayOneShot(audioClip);

        ProcessingRaycast();

        audioClip = m_RangeWeaponSound.fireTailSound[Random.Range(0, m_RangeWeaponSound.fireTailSound.Length)];
        m_AudioSource.PlayOneShot(audioClip);
    }


    private void ProcessingRaycast()
    {
        if (Physics.Raycast(m_MuzzlePos.position, mainCamera.transform.forward, out RaycastHit hit, m_RangeWeaponStat.m_MaxRange, m_RangeWeaponStat.m_AttackableLayer, QueryTriggerInteraction.Ignore))
        {
            int fireEffectNumber;
            int hitLayer = hit.transform.gameObject.layer;
            if (hitLayer == 14) fireEffectNumber = 0;
            else if (hitLayer == 17) fireEffectNumber = 1;
            else
            {
                if (!hit.transform.TryGetComponent(out MeshRenderer meshRenderer)) return;
                if ((fireEffectNumber = m_SurfaceManager.IsInMaterial(meshRenderer.sharedMaterial)) == -1) return;
            }
            EffectSet(out AudioClip audioClip, out DefaultPoolingScript effectObj, fireEffectNumber);
            AudioSource.PlayClipAtPoint(audioClip, hit.point);
            effectObj.Init(hit.point, Quaternion.LookRotation(hit.normal), m_BulletEffectPoolingObjects[fireEffectNumber]);
            effectObj.gameObject.SetActive(true);
        }
    }

    private void EffectSet(out AudioClip audioClip, out DefaultPoolingScript effectObj, int fireEffectNumber)
    {
        AudioClip[] audioClips;

        effectObj = (DefaultPoolingScript)m_BulletEffectPoolingObjects[fireEffectNumber].GetObject(false);
        audioClips = m_SurfaceManager.GetBulletHitEffectSounds(fireEffectNumber);
        audioClip = audioClips[Random.Range(0, audioClips.Length)];
    }

    private void FireRecoil()
    {
        float upRandom = Random.Range(m_RangeWeaponStat.m_UpRandomRecoil.x, m_RangeWeaponStat.m_UpRandomRecoil.y);
        float rightRandom = Random.Range(m_RangeWeaponStat.m_RightRandomRecoil.x, m_RangeWeaponStat.m_RightRandomRecoil.y);

        upRandom += m_RangeWeaponStat.m_UpAxisRecoil;
        rightRandom += m_RangeWeaponStat.m_RightAxisRecoil;
        m_FirstPersonController.MouseLook.AddRecoil(upRandom * 0.2f, rightRandom * 0.2f);
    }

    private void EndFire()
    {
        m_FireLight.Stop(false);
    }
}
