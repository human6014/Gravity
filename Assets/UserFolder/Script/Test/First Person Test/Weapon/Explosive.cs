using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : PoolableScript
{
    [SerializeField] private GameObject[] m_ActivatingEffectObject;
    [SerializeField] private ParticleSystem[] m_ControllingParticle;

    [SerializeField] private float m_AutoExplosionTime = 3;
    [SerializeField] private float m_EffectDuration = 5;
    [SerializeField] private float m_StopDuration = 5;

    [SerializeField] private bool m_IsBounce;
    [SerializeField] private bool m_IsPersistencable;
    

    private Manager.ObjectPoolManager.PoolingObject m_PoolingObject;

    private Rigidbody m_Rigidbody;
    private MeshRenderer m_MeshRenderer;

    private WaitForSeconds m_AutoExplosionSecond;
    private WaitForSeconds m_DestroyObjectSecond;

    private AudioSource m_AudioSource;
    private Light m_Light;

    private float m_InitialAudioSourceVolume;
    private float m_InitialLightIntensity;
    
    private bool m_IsExploded;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_MeshRenderer = GetComponent<MeshRenderer>();

        if (m_IsPersistencable)
        {
            m_AudioSource = m_ControllingParticle[0].GetComponent<AudioSource>();
            m_Light = m_ControllingParticle[0].GetComponent<Light>();

            m_InitialAudioSourceVolume = m_AudioSource.volume;
            m_InitialLightIntensity = m_Light.intensity;
        }

        m_AutoExplosionSecond = new WaitForSeconds(m_AutoExplosionTime);
        m_DestroyObjectSecond = new WaitForSeconds(m_EffectDuration);
    }

    public void Init(Manager.ObjectPoolManager.PoolingObject poolingObject, Vector3 pos, Quaternion rot)
    {
        m_PoolingObject = poolingObject;
        m_IsExploded = m_Rigidbody.isKinematic = false;
        m_MeshRenderer.enabled = true;
        transform.SetPositionAndRotation(pos, rot);

        if (m_IsPersistencable)
        {
            m_AudioSource.volume = m_InitialAudioSourceVolume;
            m_Light.intensity = m_InitialLightIntensity;
        }

        if (m_IsBounce) StartCoroutine(Explosion());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_IsBounce) return;
        if (m_IsExploded) return;

        Vector3 surfaceNormal = collision.contacts[0].normal;
        transform.rotation = Quaternion.LookRotation(surfaceNormal);
        StartCoroutine(Explosion());
    }

    private IEnumerator Explosion()
    {
        yield return m_AutoExplosionSecond;
        m_IsExploded = m_Rigidbody.isKinematic = true;
        m_MeshRenderer.enabled = false;

        for (int i = 0; i < m_ActivatingEffectObject.Length; i++)
        {
            m_ActivatingEffectObject[i].SetActive(true);
        }
        yield return m_DestroyObjectSecond;

        if (m_IsPersistencable) yield return PersistencingDestroy();

        for (int i = 0; i < m_ActivatingEffectObject.Length; i++)
        {
            m_ActivatingEffectObject[i].SetActive(false);
        }

        ReturnObject();
    }

    private IEnumerator PersistencingDestroy()
    {
        for (int i = 0; i < m_ControllingParticle.Length; i++)
            m_ControllingParticle[i].Stop();

        float stopDuration = Time.time + m_StopDuration;

        while (stopDuration > Time.time)
        {
            m_AudioSource.volume -= Time.deltaTime * (1 / m_StopDuration);
            m_Light.intensity -= Time.deltaTime * (1 / m_StopDuration);

            yield return null;
        }
    }

    public override void ReturnObject() => m_PoolingObject.ReturnObject(this);
}
