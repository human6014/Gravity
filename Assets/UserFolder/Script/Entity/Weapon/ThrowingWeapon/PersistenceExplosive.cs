using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistenceExplosive : Explosible
{
    [SerializeField] private ParticleSystem[] m_ControllingParticle;

    private AudioSource m_AudioSource;
    private Light m_Light;
    private SphereCollider m_SphereCollider;
    private WaitForSeconds m_PersistenceExplosionTime;

    private const float m_DamageTick = 0.25f;
    private float m_InitialAudioSourceVolume;
    private float m_InitialLightIntensity;
    private float m_InitialLightRange;
    private float m_InitialColliderRadius;

    protected override void Awake()
    {
        base.Awake();

        m_AudioSource = m_ControllingParticle[0].GetComponent<AudioSource>();
        m_Light = m_ControllingParticle[0].GetComponent<Light>();
        m_SphereCollider = m_ControllingParticle[0].GetComponent<SphereCollider>();
        m_PersistenceExplosionTime = new WaitForSeconds(m_DamageTick);

        m_InitialAudioSourceVolume = m_AudioSource.volume;
        m_InitialLightIntensity = m_Light.intensity;
        m_InitialLightRange = m_Light.range;
        m_InitialColliderRadius = m_SphereCollider.radius;
    }

    public override void Init(Manager.ObjectPoolManager.PoolingObject poolingObject, Vector3 pos, Quaternion rot, AttackType bulletType)
    {
        base.Init(poolingObject, pos, rot, bulletType);

        m_AudioSource.volume = m_InitialAudioSourceVolume;
        m_Light.intensity = m_InitialLightIntensity;
        m_Light.range = m_InitialLightRange;
        m_SphereCollider.radius = m_InitialColliderRadius;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (m_IsExploded) return;

        Vector3 surfaceNormal = collision.contacts[0].normal;
        transform.rotation = Quaternion.LookRotation(surfaceNormal);
        StartCoroutine(PersistenceExplosion());
    }

    private IEnumerator PersistenceExplosion()
    {
        yield return base.Explosion();

        float elapsedTime = 0;
        while(elapsedTime < m_EffectDuration)
        {
            elapsedTime += m_DamageTick;
            base.Damage(false);
            yield return m_PersistenceExplosionTime;
        }

        yield return PersistencingDestroy();

        base.EndExplosion();
        base.ReturnObject();
    }

    private IEnumerator PersistencingDestroy()
    {
        for (int i = 0; i < m_ControllingParticle.Length; i++)
            m_ControllingParticle[i].Stop();

        float elapsedTime = 0;
        float t;
        while (elapsedTime < m_StopDuration)
        {
            elapsedTime += m_DamageTick;
            if (elapsedTime * 2 < m_StopDuration) base.Damage(false);
            t = elapsedTime / m_StopDuration; 

            m_AudioSource.volume = Mathf.Lerp(m_InitialAudioSourceVolume, 0, t);
            m_Light.intensity = Mathf.Lerp(m_InitialLightIntensity, 0, t);
            m_Light.range = Mathf.Lerp(m_InitialLightRange, 0, t);
            m_SphereCollider.radius = Mathf.Lerp(m_InitialColliderRadius, 0, t);

            yield return m_PersistenceExplosionTime;
        }
    }
}
