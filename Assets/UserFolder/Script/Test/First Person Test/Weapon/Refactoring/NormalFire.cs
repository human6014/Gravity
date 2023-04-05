using Scriptable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Object;
using Contoller.Player;
using Manager;

[RequireComponent(typeof(Test.RangeWeapon))]
public class NormalFire : MonoBehaviour, IFireable
{
    [Header("Fire light")]
    [Tooltip("ÃÑ±¸ È­¿° Á¦¾î ½ºÅ©¸³Æ®")]
    [SerializeField] private TestFireLight m_FireLight;

    [Header("Casing")]
    [Tooltip("ÃÑ¾Ë »ý¼º À§Ä¡")]
    [SerializeField] private Transform m_CasingSpawnPos;

    private Transform m_MuzzlePos;             //ÃÑ±¸ À§Ä¡
    private Camera mainCamera;

    private RangeWeaponStatScriptable m_RangeWeaponStat;
    private ObjectPoolManager.PoolingObject[] m_BulletEffectPoolingObjects; //0 : Concrete, 1 : Metal, 2 : Wood
    private ObjectPoolManager.PoolingObject m_CasingPoolingObject;

    private SurfaceManager m_SurfaceManager;
    private FirstPersonController m_FirstPersonController;

    private bool m_HasBullet;


    private void Awake()
    {
        m_MuzzlePos = m_FireLight.transform;
        mainCamera = Camera.main;
    }

    public void Setup(RangeWeaponStatScriptable m_RangeWeaponStat,ObjectPoolManager.PoolingObject[] m_BulletEffectPoolingObjects,
                    SurfaceManager m_SurfaceManager, FirstPersonController m_FirstPersonController)
    {
        this.m_RangeWeaponStat = m_RangeWeaponStat;
        this.m_BulletEffectPoolingObjects = m_BulletEffectPoolingObjects;
        this.m_SurfaceManager = m_SurfaceManager;
        this.m_FirstPersonController = m_FirstPersonController;
    }

    public void SetupCasingPooling(ObjectPoolManager.PoolingObject m_CasingPoolingObject)
    {
        this.m_CasingPoolingObject = m_CasingPoolingObject;
        m_HasBullet = m_CasingPoolingObject != null;
    }

    public void DoFire()
    {
        FireRay();
        FireRecoil();
        m_FireLight.Play(false);
        InstanceBullet();
        Invoke(nameof(EndFire), 0.1f);
    }

    private void InstanceBullet()
    {
        if (!m_HasBullet) return;
        Quaternion cassingSpawnRotation = Quaternion.Euler(Random.Range(-30, 30), Random.Range(-30, 30), Random.Range(-30, 30));

        DefaultPoolingScript casingPoolingObject = (DefaultPoolingScript)m_CasingPoolingObject.GetObject(false);

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
