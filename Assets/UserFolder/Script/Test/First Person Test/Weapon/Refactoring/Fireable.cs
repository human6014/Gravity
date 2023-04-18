using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity.Object;
using Contoller.Player;
using Manager;
using Scriptable;
using Contoller.Player.Utility;

[RequireComponent(typeof(Test.RangeWeapon))]
public abstract class Fireable : MonoBehaviour
{
    [Header("Fire light")]
    [Tooltip("총구 화염 제어 스크립트")]
    [SerializeField] protected TestFireLight m_FireLight;

    [Header("Casing")]
    [Tooltip("총알이 생성되는지")]
    [SerializeField] private bool m_HasBullet;

    [Tooltip("총알 생성 위치")]
    [SerializeField] private Transform m_CasingSpawnPos;

    [Tooltip("총알 오브젝트")]
    [SerializeField] private PoolableScript m_CasingObject;

    [Header("Pooling")]
    [Tooltip("풀링 오브젝트 하이라키 위치")]
    [SerializeField] private Transform m_ActiveObjectPool;

    [Tooltip("미리 생성할 개수")]
    [SerializeField] [Range(0,50)] private int m_PoolingCount;

    [Header("Timing")]
    [SerializeField] private bool m_IsDelayCasing;
    [SerializeField] private float m_LightOffTime = 0.3f;
    [SerializeField] private float m_InstanceCasingTime = 1f;

    protected Transform m_CameraTransform;
    protected RangeWeaponStatScriptable m_RangeWeaponStat;

    private WaitForSeconds m_LightOffSecond;
    private WaitForSeconds m_InstanceBulletSecond;

    private ObjectPoolManager.PoolingObject[] m_BulletEffectPoolingObjects; //0 : Concrete, 1 : Metal, 2 : Wood
    private ObjectPoolManager.PoolingObject m_CasingPoolingObject;

    private PlayerState m_PlayerState;
    private SurfaceManager m_SurfaceManager;
    private MouseLook m_MouseLook;
    private UI.Player.CrossHairDisplayer m_CrossHairController;
    private void Awake()
    {
        m_CameraTransform = Camera.main.transform;

        m_LightOffSecond = new WaitForSeconds(m_LightOffTime);
        m_InstanceBulletSecond = new WaitForSeconds(m_InstanceCasingTime);
    }

    public void Setup(RangeWeaponStatScriptable m_RangeWeaponStat, ObjectPoolManager.PoolingObject[] m_BulletEffectPoolingObjects, PlayerState m_PlayerState, Camera m_MainCamera)
    {
        this.m_RangeWeaponStat = m_RangeWeaponStat;
        this.m_BulletEffectPoolingObjects = m_BulletEffectPoolingObjects;
        this.m_PlayerState = m_PlayerState; 
        m_CameraTransform = m_MainCamera.transform;

        m_SurfaceManager = FindObjectOfType<SurfaceManager>();
        m_MouseLook = FindObjectOfType<FirstPersonController>().MouseLook;
        m_CrossHairController = FindObjectOfType<UI.Player.CrossHairDisplayer>();

        if (!m_HasBullet) return;
        m_CasingPoolingObject = ObjectPoolManager.Register(m_CasingObject, m_ActiveObjectPool);
        m_CasingPoolingObject.GenerateObj(m_PoolingCount);
    }

    public void DoFire()
    {
        FireRay();
        FireRecoil();
        m_FireLight.Play(false);
        StartCoroutine(EndFire());
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
        Vector3 randomForce = new Vector3(Random.Range(0.12f, 0.36f), Random.Range(0.12f, 0.36f), Random.Range(0.12f, 0.36f));
        Vector3 randomTorque = new Vector3(Random.Range(-spinValue, spinValue), Random.Range(-spinValue, spinValue), Random.Range(-spinValue, spinValue));

        cassingRB.velocity = m_CasingSpawnPos.right + randomForce;
        cassingRB.angularVelocity = randomTorque;
    }

    protected abstract void FireRay();

    protected void ProcessingRay(RaycastHit hit,int i)
    {
        if (hit.transform.TryGetComponent(out IDamageable damageable))
        {
            damageable.Hit(m_RangeWeaponStat.m_Damage);
            return;
        }

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

        if (i % 2 == 0) AudioSource.PlayClipAtPoint(audioClip, hit.point);

        effectObj.Init(hit.point, Quaternion.LookRotation(hit.normal), m_BulletEffectPoolingObjects[fireEffectNumber]);
        effectObj.gameObject.SetActive(true);
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
        m_MouseLook.AddRecoil(upRandom * 0.2f, rightRandom * 0.2f);
    }

    private IEnumerator EndFire()
    {
        if (!m_IsDelayCasing) InstanceBullet();

        yield return m_LightOffSecond;
        m_FireLight.Stop(false);

        if (!m_IsDelayCasing) yield break;

        yield return m_InstanceBulletSecond;
        InstanceBullet();
    }

    protected Vector3 GetCurrentAccuracy()
    {
        float playerPosAccuracy = m_CrossHairController.GetCurrentAccurancy();
        float weaponPosAccuracy = m_PlayerState.BeforePlayerWeaponState == PlayerWeaponState.Aiming ? 
            m_RangeWeaponStat.m_AimingAccuracy : m_RangeWeaponStat.m_IdleAccuracy;

        float minValue = Mathf.Min(-playerPosAccuracy - weaponPosAccuracy, 0);
        float maxValue = Mathf.Max( playerPosAccuracy + weaponPosAccuracy, 0);

        float xAccuracy = Random.Range(minValue, maxValue);
        float yAccuracy = Random.Range(minValue, maxValue);
        float zAccuracy = Random.Range(minValue, maxValue);

        return new Vector3(xAccuracy, yAccuracy, zAccuracy);
    }
}
