using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : PoolableScript
{
    private Manager.ObjectPoolManager.PoolingObject m_PoolingObject;

    private Rigidbody m_Rigidbody;
    private AudioSource m_AudioSource;

    [SerializeField] private bool m_IsBounce;
    [SerializeField] private GameObject[] m_ActivatingEffectObject;

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void Init(Manager.ObjectPoolManager.PoolingObject poolingObject)
    {
        m_PoolingObject = poolingObject;

        Invoke(nameof(Explosion),3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!m_IsBounce) m_Rigidbody.isKinematic = true;


    }

    private void Explosion()
    {
        for (int i = 0; i < m_ActivatingEffectObject.Length; i++)
        {
            m_ActivatingEffectObject[i].SetActive(true);
            m_ActivatingEffectObject[i].GetComponent<ParticleSystem>().Play();
        }
    }

    public override void ReturnObject() => m_PoolingObject.ReturnObject(this);
}
